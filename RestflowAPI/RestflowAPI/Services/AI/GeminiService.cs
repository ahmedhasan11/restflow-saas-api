using RestflowAPI.ServiceInterfaces.AI;
using RestflowAPI.Settings;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;

namespace RestflowAPI.Services.AI
{
    public class GeminiService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;

        public GeminiService(
            HttpClient httpClient,
            IOptions<GeminiSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<string> GenerateAsync(
            string systemPrompt,
            string userPrompt,
            CancellationToken cancellationToken)
        {
            var endpoint =
                $"{_settings.BaseUrl}models/{_settings.Model}:generateContent?key={_settings.ApiKey}";

            var body = new
            {
                contents = new[]
                {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text =
                                $"{systemPrompt}\n\n{userPrompt}"
                        }
                    }
                }
            }
            };

            var json =
                JsonSerializer.Serialize(body);

            var response =
                await _httpClient.PostAsync(
                    endpoint,
                    new StringContent(json, Encoding.UTF8, "application/json"),
                    cancellationToken);

            response.EnsureSuccessStatusCode();

            var content =
                await response.Content.ReadAsStringAsync(cancellationToken);

            using JsonDocument doc =
                JsonDocument.Parse(content);

            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString()!;
        }
    }
}