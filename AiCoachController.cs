using Microsoft.AspNetCore.Mvc;
using GymTrack.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GymTrack.Controllers
{
    public class AiCoachController : Controller
    {
        private readonly OpenAiWorkoutService _workoutService;

        public AiCoachController(OpenAiWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePlan(
            int age,
            int height,
            int weight,
            string gender,
            string target,
            string level,
            int days,
            string equipment,
            IFormFile? bodyImage)
        {
            
            var textPrompt = $"Bana 4 haftalık detaylı bir antrenman programı hazırla. " +
                             $"Bilgilerim: Yaş: {age}, Cinsiyet: {gender}, Boy: {height}cm, Kilo: {weight}kg. " +
                             $"Hedefim: {target}. Seviyem: {level}. " +
                             $"Haftada {days} gün çalışacağım. Ekipman durumum: {equipment}. " +
                             "Programı haftalara böl ve Türkçe yaz.";

            var plan = await _workoutService.GetWorkoutPlanAsync(textPrompt);
            ViewBag.Plan = plan;

            
            string englishTarget = target switch
            {
                "Yağ Yakımı" => "very lean, defined abs, athletic physique",
                "Kas Kazanımı" => "muscular, ripped physique, broad shoulders",
                "Güç Artışı" => "strong, powerful build, powerlifter physique",
                "Dayanıklılık" => "lean runner's physique, very fit",
                _ => "fit and healthy physique"
            };

            string englishGender = gender == "Erkek" ? "man" : "woman";

            
            string imagePrompt = $"A high quality, realistic photo of a {englishGender}, approximately {age} years old, with a {englishTarget}. " +
                                 $"They are standing in a modern gym setting with good lighting, looking confident and fit after a successful workout program. " +
                                 $"Fitness photography style.";

            
            string generatedImageUrl = await _workoutService.GenerateImageAsync(imagePrompt);
            ViewBag.GeneratedImageUrl = generatedImageUrl;

            return View("Index");
        }
    }
}