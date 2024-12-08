using System.Text.Json.Serialization;

namespace STech.Chatbot.Models
{
    public class ChatbotParseResponse
    {
        public string text { get; set; } = null!;
        public Intent intent { get; set; } = new Intent();
        public List<Entity> entities { get; set; } = new List<Entity>();
        public List<List<int>> text_tokens { get; set; } = new List<List<int>>();
        public List<Intent> intent_ranking { get; set; } = new List<Intent>();

        [JsonIgnore]
        public ResponseSelector response_selector { get; set; } = new ResponseSelector();
    }

    public class Intent
    {
        public string name { get; set; } = null!;
        public double confidence { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; } = null!;
        public int start { get; set; }
        public int end { get; set; }
        public double confidence_entity { get; set; }
        public string value { get; set; } = null!;
        public string extractor { get; set; } = null!;
    }

    public class ResponseSelector
    {
        public Dictionary<string, object> all_retrieval_intents { get; set; } = null!;
        public DefaultResponse Default { get; set; } = new DefaultResponse();
    }

    public class DefaultResponse
    {
        public Response response { get; set; } = new Response();
        public List<object> ranking { get; set; } = new List<object>();
    }

    public class Response
    {
        public object responses { get; set; } = null!;
        public double confidence { get; set; }
        public string intent_response_key { get; set; } = null!;
        public string utter_action { get; set; } = null!;
    }

}
