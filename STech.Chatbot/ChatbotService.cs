using STech.Chatbot.Models;
using System.Text;
using System.Text.Json;

namespace STech.Chatbot
{
    public class ChatbotService : IChatbotService
    {
        private readonly string _parseApiUrl = "http://localhost:5005/model/parse";
        private readonly string _messageApiUrl = "http://localhost:5005/webhooks/rest/webhook";

        private readonly HttpClient _httpClient;

        public ChatbotService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ChatbotParseResponse?> GetParseResponse(string message)
        {
            var requestData = JsonSerializer.Serialize(new
            {
                text = message
            });

            var content = new StringContent(requestData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(_parseApiUrl, content);

            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ChatbotParseResponse>(json);

                return data;
            }

            return null;
        }

        public async Task<ChatbotMessageResponse?> GetMessageResponse(string message)
        {
            var requestData = JsonSerializer.Serialize(new
            {
                sender = "user",
                message = message
            });

            var content = new StringContent(requestData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(_messageApiUrl, content);

            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<ChatbotMessageResponse>>(json);

                return data?.FirstOrDefault();
            }

            return null;
        }
    }
}
