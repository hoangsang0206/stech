using STech.Data.Models;

namespace STech.Services
{
    public interface IOrderService
    {
        Task<bool> CreateInvoice(Invoice invoice);
        Task<Invoice?> GetInvoice(string invoiceId);
        Task<bool> UpdateInvoice(Invoice invoice);
    }
}
