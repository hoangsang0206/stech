using STech.Data.Models;

namespace STech.Services
{
    public interface IOrderService
    {
        Task<bool> CreateInvoice(Invoice invoice);
        Task<Invoice?> GetInvoice(string invoiceId);
        Task<Invoice?> GetInvoice(string invoiceId, string phone);
        Task<bool> UpdateInvoice(Invoice invoice);
    }
}
