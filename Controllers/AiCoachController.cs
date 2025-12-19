using GymTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            if (!ModelState.IsValid)
                return View(model);

            var result = await _ai.GeneratePlanAsync(model, ct);

            ViewBag.Ok = result.Ok;
            ViewBag.Error = result.Error;
            ViewBag.Json = result.Json;

            return View(model);
        }
    }
}
