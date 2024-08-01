using STech.Data.Models;

namespace STech.Services
{
    public interface IOrderService
    {
        Task<bool> CreateInvoice(Invoice invoice);
        Task<Invoice?> GetInvoice(string invoiceId);
        Task<Invoice?> GetInvoiceWithDetails(string invoiceId, string phone);
        Task<Invoice?> GetUserInvoiceWithDetails(string invoiceId, string userId);
        Task<IEnumerable<Invoice>> GetUserInvoices(string userId);
        Task<bool> UpdateInvoice(Invoice invoice);
    }
}
