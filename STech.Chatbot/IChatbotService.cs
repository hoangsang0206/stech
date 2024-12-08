using STech.Chatbot.Models;

namespace STech.Chatbot
{
    public interface IChatbotService
    {
        Task<ChatbotParseResponse?> GetParseResponse(string message);
        Task<ChatbotMessageResponse?> GetMessageResponse(string message);
    }
}
