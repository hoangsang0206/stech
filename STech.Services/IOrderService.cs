using STech.Data.Models;
using STech.Data.ViewModels;

namespace STech.Services
{
    public interface IOrderService
    {
        Task<int> GetTotalOrders();
        Task<decimal> GetTotalRevenue();
        Task<int> GetMonthOrders(int month);
        Task<decimal> GetMonthRevenue(int month);

        Task<bool> CreateInvoice(Invoice invoice);
        Task<Invoice?> GetInvoice(string invoiceId);
        Task<Invoice?> GetInvoiceWithDetails(string invoiceId);
        Task<Invoice?> GetInvoiceWithDetails(string invoiceId, string phone);
        Task<Invoice?> GetUserInvoiceWithDetails(string invoiceId, string userId);
        Task<IEnumerable<Invoice>> GetUserInvoices(string userId);
        Task<IEnumerable<Invoice>> GetRecentOrders(int numToTake);

        Task<bool> CheckIsPurchased(string userId, string productId);
        Task<bool> CheckIsPurchased(string email, string? phone, string productId);
        Task<bool> UpdateInvoice(Invoice invoice);
        Task<bool> AddInvoiceStatus(InvoiceStatus invoiceStatus);
        Task<bool> UpdateInvoiceStatus(InvoiceStatus invoiceStatus);
        Task<PagedList<Invoice>> GetInvoices(int page, int itemsPerPage, string? filterBy, string? sortBy);
        Task<PagedList<Invoice>> SearchInvoices(string query, int page, int itemsPerPage);
    }
}
