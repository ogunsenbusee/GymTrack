using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace GymTrack.Services
{
    public class OpenAiWorkoutService
    {
        private readonly HttpClient _http;
        private readonly OpenAiOptions _opt;

        public OpenAiWorkoutService(HttpClient http, IOptions<OpenAiOptions> opt)
        {
            _http = http;
            _opt = opt.Value;
        }

        public async Task<WorkoutAiResult> GeneratePlanAsync(WorkoutAiRequest req, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_opt.ApiKey))
            {
                return new WorkoutAiResult
                {
                    Ok = false,
                    Error = "OpenAI API anahtarı yok. user-secrets içine ekle."
                };
            }

            var prompt =
$@"Kullanıcı:
- Boy: {req.HeightCm} cm
- Kilo: {req.WeightKg} kg
- Hedef: {req.Goal}
- Seviye: {req.Level}
- Haftada: {req.DaysPerWeek} gün
- Ekipman: {req.Equipment}

4 haftalık antrenman planı üret.
SADECE JSON döndür (json_object).";

            var payload = new
            {
                model = _opt.Model,
                input = prompt,
                text = new { format = new { type = "json_object" } }
            };

            using var msg = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
            msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _opt.ApiKey);
            msg.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var resp = await _http.SendAsync(msg, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
            {
                return new WorkoutAiResult
                {
                    Ok = false,
                    Error = $"OpenAI hata: {resp.StatusCode} | {body}"
                };
            }

            try
            {
                using var doc = JsonDocument.Parse(body);

                string? jsonText = null;

                if (doc.RootElement.TryGetProperty("output_text", out var outputText))
                    jsonText = outputText.GetString();

                if (string.IsNullOrWhiteSpace(jsonText) &&
                    doc.RootElement.TryGetProperty("output", out var outputArr) &&
                    outputArr.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in outputArr.EnumerateArray())
                    {
                        if (item.TryGetProperty("content", out var contentArr) &&
                            contentArr.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var c in contentArr.EnumerateArray())
                            {
                                if (c.TryGetProperty("text", out var t))
                                {
                                    jsonText = t.GetString();
                                    if (!string.IsNullOrWhiteSpace(jsonText))
                                        break;
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(jsonText))
                            break;
                    }
                }

                if (string.IsNullOrWhiteSpace(jsonText))
                {
                    return new WorkoutAiResult
                    {
                        Ok = false,
                        Error = "AI cevabı çözümlenemedi.",
                        Raw = body
                    };
                }

                return new WorkoutAiResult
                {
                    Ok = true,
                    Json = jsonText,
                    Raw = body
                };
            }
            catch (Exception ex)
            {
                return new WorkoutAiResult
                {
                    Ok = false,
                    Error = "AI parse hata: " + ex.Message,
                    Raw = body
                };
            }
        }
    }

    public class WorkoutAiRequest
    {
        public int HeightCm { get; set; }
        public int WeightKg { get; set; }
        public string Goal { get; set; } = "Kilo verme";
        public string Level { get; set; } = "Başlangıç";
        public int DaysPerWeek { get; set; } = 3;
        public string Equipment { get; set; } = "Salonda temel ekipman";
    }

    public class WorkoutAiResult
    {
        public bool Ok { get; set; }
        public string? Json { get; set; }
        public string? Raw { get; set; }
        public string? Error { get; set; }
    }
}
