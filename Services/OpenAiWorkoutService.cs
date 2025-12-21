using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymTrack.Services
{
    public class OpenAiWorkoutService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAiOptions _options;

        public OpenAiWorkoutService(HttpClient httpClient, IOptions<OpenAiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            if (!string.IsNullOrEmpty(_options.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            }
        }

        public async Task<string> GetWorkoutPlanAsync(string prompt)
        {
            if (string.IsNullOrEmpty(_options.ApiKey))
            {
                return "⚠ API Anahtarı tanımlanmamış.";
            }

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Sen profesyonel bir fitness koçusun. Türkçe cevap ver." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 1000
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseString);
                    return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                }
                return $"Hata: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                return $"Bağlantı hatası: {ex.Message}";
            }
        }

        
        public async Task<string> GenerateImageAsync(string imagePrompt)
        {
            if (string.IsNullOrEmpty(_options.ApiKey)) return null;

            
            var requestBody = new
            {
                model = "dall-e-3", 
                prompt = imagePrompt,
                n = 1, 
                size = "1024x1024",
                quality = "standard",
                response_format = "url"
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/images/generations", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseString);
                    
                    string imageUrl = doc.RootElement.GetProperty("data")[0].GetProperty("url").GetString();
                    return imageUrl;
                }
                else
                {
                    
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"DALL-E Hatası: {errorResponse}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DALL-E Bağlantı Hatası: {ex.Message}");
                return null;
            }
        }
    }
}