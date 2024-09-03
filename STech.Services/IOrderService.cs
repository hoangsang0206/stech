using STech.Data.Models;

namespace STech.Services
{
    public interface IOrderService
    {
        Task<bool> CreateInvoice(Invoice invoice);
        Task<Invoice?> GetInvoice(string invoiceId);
        Task<Invoice?> GetInvoiceWithDetails(string invoiceId);
        Task<Invoice?> GetInvoiceWithDetails(string invoiceId, string phone);
        Task<Invoice?> GetUserInvoiceWithDetails(string invoiceId, string userId);
        Task<IEnumerable<Invoice>> GetUserInvoices(string userId);
        Task<bool> CheckIsPurchased(string userId, string productId);
        Task<bool> CheckIsPurchased(string email, string? phone, string productId);
        Task<bool> UpdateInvoice(Invoice invoice);
        Task<bool> AddInvoiceStatus(InvoiceStatus invoiceStatus);
        Task<bool> UpdateInvoiceStatus(InvoiceStatus invoiceStatus);
        Task<(IEnumerable<Invoice>, int)> GetInvoices(int page, string? filterBy, string? sortBy);
        Task<IEnumerable<Invoice>> SearchInvoices(string query);
    }
}
