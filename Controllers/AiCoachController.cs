using GymTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GymTrack.Controllers
{
    [Authorize]
    public class AiCoachController : Controller
    {
        private readonly OpenAiWorkoutService _ai;

        public AiCoachController(OpenAiWorkoutService ai)
        {
            _ai = ai;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new WorkoutAiRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(WorkoutAiRequest model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _ai.GeneratePlanAsync(model, ct);

            if (result.Ok && !string.IsNullOrWhiteSpace(result.Json))
            {
                // JSON'u pretty yap
                try
                {
                    using var doc = JsonDocument.Parse(result.Json);
                    ViewBag.PrettyJson = JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });

                    // meta ve notları çıkarmaya çalış
                    if (doc.RootElement.TryGetProperty("meta", out var meta))
                        ViewBag.Meta = meta;

                    if (doc.RootElement.TryGetProperty("notlar", out var notes) && notes.ValueKind == JsonValueKind.Array)
                        ViewBag.Notes = notes;
                }
                catch
                {
                    ViewBag.PrettyJson = result.Json;
                }
            }

            ViewBag.Ok = result.Ok;
            ViewBag.Error = result.Error;
            ViewBag.RawJson = result.Json;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DownloadJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return RedirectToAction(nameof(Index));

            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", "workout-plan.json");
        }
    }
}
