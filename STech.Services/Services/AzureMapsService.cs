using System.Text.Json;
using System.Text.Json.Serialization;

namespace STech.Services.Services
{
    public class AzureMapsService : IAzureMapsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;
        private readonly string _subscriptionKey;

        public AzureMapsService(HttpClient httpClient, string baseUri, string subscriptionKey)
        {
            _baseUri = baseUri;
            _httpClient = httpClient;
            _subscriptionKey = subscriptionKey;
        }

        public async Task<(double? Latitude, double? Longtitude)> GetLocation(string city, string district, string ward)
        {
            string address = $"{ward}, {district}, {city}, Vietnam";
            return await FetchLocation(address);
        }

        public async Task<(double? Latitude, double? Longtitude)> GetLocation(string address)
        {
            return await FetchLocation(address);
        }

        private async Task<(double? Latitude, double? Longtitude)> FetchLocation(string query)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_baseUri}?query={query}&subscription-key={_subscriptionKey}");
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<AzureMapsResponse>(json);

                if (data != null && data.Results?.Count() > 0)
                {
                    Position? position = data.Results.ElementAt(0).Position;
                    return (position?.Latitude, position?.Longitude);
                }
            }

            return (null, null);
        }

        public class AzureMapsResponse
        {
            [JsonPropertyName("results")]
            public List<SearchResult>? Results { get; set; }
        }

        public class SearchResult
        {
            [JsonPropertyName("position")]
            public Position? Position { get; set; }
        }

        public class Position
        {
            [JsonPropertyName("lat")]
            public double Latitude { get; set; }

            [JsonPropertyName("lon")]
            public double Longitude { get; set; }
        }
    }
}
