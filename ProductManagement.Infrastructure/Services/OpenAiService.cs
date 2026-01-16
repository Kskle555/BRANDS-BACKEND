using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductManagement.Infrastructure.Services
{
    public class OpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"];
        }

        public async Task<string> GenerateProductDescription(string productName)
        {
            if (string.IsNullOrEmpty(_apiKey)) return "API Key eksik, AI çalışmadı.";

            var requestBody = new
            {
                model = "gpt-3.5-turbo", // veya "gpt-4"
                messages = new[]
                {
                    new { role = "system", content = "Sen profesyonel bir e-ticaret editörüsün. Ürünler için kısa, etkileyici, pazarlama odaklı, Türkçe ve maksimum 2 cümlelik açıklamalar yazıyorsun." },
                    new { role = "user", content = $"Şu ürün için açıklama yaz: {productName}" }
                },
                max_tokens = 100
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);

            if (!response.IsSuccessStatusCode) return "AI servisine ulaşılamadı.";

            var responseString = await response.Content.ReadAsStringAsync();

            // JSON Parse işlemi (Hızlıca sonuca ulaşmak için dynamic kullanıyoruz)
            using var doc = JsonDocument.Parse(responseString);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return content;
        }

        public async Task<string> GeneratePricePrediction(string productName)
        {
            if (string.IsNullOrEmpty(_apiKey)) return "0";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
            new { role = "system", content = "Sen bir fiyat uzmanısın. Sana verilen ürünün Türkiye piyasasındaki ortalama fiyatını (TL) tahmin edeceksin. Cevap olarak SADECE ve SADECE sayıyı ver (nokta veya para birimi işareti kullanma, düz tamsayı ver). Örn: 15000" },
            new { role = "user", content = $"Şu ürünün ortalama fiyatı ne kadar: {productName}" }
        },
                max_tokens = 10
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);

            if (!response.IsSuccessStatusCode) return "0";

            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            // Gelen cevabın sadece sayı olduğundan emin olalım (temizlik)
            return new string(content.Where(char.IsDigit).ToArray());
        }
    }
}