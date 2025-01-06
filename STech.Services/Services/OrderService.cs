using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services.Constants;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly StechDbContext _context;
        
        public OrderService(StechDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalOrders()
        {
            return await _context.Invoices.CountAsync();
        }

        public async Task<decimal> GetTotalRevenue()
        {
            return await _context.Invoices
                .SumAsync(i => i.Total);
        }

        public async Task<int> GetMonthOrders(int month, int year)
        {
            DateTime startOfMonth = new DateTime(year, month, 1);
            DateTime startOfNextMonth = startOfMonth.AddMonths(1);

            return await _context.Invoices
                .Where(i => i.OrderDate >= startOfMonth && i.OrderDate < startOfNextMonth)
                .CountAsync();
        }

        public async Task<decimal> GetMonthRevenue(int month, int year)
        {
            DateTime startOfMonth = new DateTime(year, month, 1);
            DateTime startOfNextMonth = startOfMonth.AddMonths(1);

            return await _context.Invoices
                .Where(i => i.OrderDate >= startOfMonth && i.OrderDate < startOfNextMonth)
                .SumAsync(i => i.Total);
        }

        public async Task<IEnumerable<MonthlyOrderSummary>> GetLastSixMonthSummary()
        {
            DateTime now = DateTime.Now;
            DateTime sixMonthAgo = new DateTime(now.Year, now.Month, 1).AddMonths(-5);

            var sixMonths = Enumerable.Range(0, 6)
                .Select(i => sixMonthAgo.AddMonths(i))
                .Select(i => new { i.Year, i.Month})
                .ToList();

            var data = await _context.Invoices
                .Where(i => i.OrderDate >= sixMonthAgo)
                .GroupBy(i => new { i.OrderDate.Value.Year, i.OrderDate.Value.Month })
                .Select(i => new MonthlyOrderSummary
                {
                    Month = i.Key.Month,
                    Year = i.Key.Year,
                    TotalOrders = i.Count(),
                    TotalRevenue = i.Sum(i => i.Total)
                })
                .OrderBy(i => i.Year).ThenBy(i => i.Month)
                .ToListAsync();

            return sixMonths
                .GroupJoin(data,
                    m => new { m.Year, m.Month },
                    q => new { q.Year, q.Month },
                    (m, q) => new MonthlyOrderSummary
                    {
                        Year = m.Year,
                        Month = m.Month,
                        TotalOrders = q.FirstOrDefault()?.TotalOrders ?? 0,
                        TotalRevenue = q.FirstOrDefault()?.TotalRevenue ?? 0
                    })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();  
        }

        public async Task<bool> CreateInvoice(Invoice invoice)
        {
            IEnumerable<InvoiceDetail> invoiceDetails = invoice.InvoiceDetails;
            IEnumerable<InvoiceStatus> invoiceStatuses = invoice.InvoiceStatuses;
            PackingSlip? packingSlip = invoice.PackingSlip;

            invoice.InvoiceDetails = new List<InvoiceDetail>();
            invoice.InvoiceStatuses = new List<InvoiceStatus>();
            invoice.PackingSlip = null;
            invoice.WarehouseExports = new List<WarehouseExport>();

            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();

            await _context.InvoiceStatuses.AddRangeAsync(invoiceStatuses);
            await _context.InvoiceDetails.AddRangeAsync(invoiceDetails);

            if (packingSlip != null)
            {
                await _context.PackingSlips.AddAsync(packingSlip);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Invoice?> GetInvoice(string invoiceId)
        {
            return await _context.Invoices
                .Where(i => i.InvoiceId == invoiceId)
                .FirstOrDefaultAsync();
        }

        public async Task<Invoice?> GetUserInvoice(string invoiceId, string userId)
        {
            return await _context.Invoices
                .Where(i => i.InvoiceId == invoiceId && i.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<Invoice?> GetUserCurrentOrder(string userId)
        {
            return await _context.Invoices
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.OrderDate)
                .FirstOrDefaultAsync();
        }

        public async Task<Invoice?> GetUserInvoiceWithDetails(string invoiceId, string userId)
        {
            Invoice? invoice = await _context.Invoices
                .Where(i => i.InvoiceId == invoiceId && i.UserId == userId)
                .Include(i => i.PaymentMed)
                .Include(i => i.InvoiceStatuses)
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .ThenInclude(d => d.ProductImages)
                .Include(i => i.PackingSlip)
                .FirstOrDefaultAsync();

            if (invoice != null)
            {
                invoice.InvoiceDetails = invoice.InvoiceDetails
                    .SelectDetail()
                    .ToList();
            }

            return invoice;
        }

        public async Task<Invoice?> GetInvoiceWithDetails(string invoiceId, string phone)
        {
            Invoice? invoice = await _context.Invoices
                .Where(i => i.InvoiceId == invoiceId && i.RecipientPhone == phone)
                .Include(i => i.PaymentMed)
                .Include(i => i.InvoiceStatuses)
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .ThenInclude(d => d.ProductImages)
                .Include(i => i.PackingSlip)
                .FirstOrDefaultAsync();

            if(invoice != null)
            {
                invoice.InvoiceDetails = invoice.InvoiceDetails
                    .SelectDetail()
                    .ToList();
            }

            return invoice;
        }

        public async Task<Invoice?> GetInvoiceWithDetails(string invoiceId)
        {
            Invoice? invoice = await _context.Invoices
                .Where(i => i.InvoiceId == invoiceId)
                .Include(i => i.PaymentMed)
                .Include(i => i.InvoiceStatuses)
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .ThenInclude(d => d.ProductImages)
                .Include(i => i.PackingSlip)
                .FirstOrDefaultAsync();

            if (invoice != null)
            {
                invoice.InvoiceDetails = invoice.InvoiceDetails
                    .SelectDetail()
                    .ToList();
            }

            return invoice;
        }

        public async Task<Invoice?> GetInvoiceWithAllDetails(string invoiceId)
        {
            Invoice? invoice = await _context.Invoices
                .Where(i => i.InvoiceId == invoiceId)
                .Include(i => i.PaymentMed)
                .Include(i => i.InvoiceStatuses)
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .ThenInclude(d => d.ProductImages)
                .Include(i => i.PackingSlip)
                .Include(i => i.User)
                .Include(i => i.Employee)
                .Include(i => i.Customer)
                .FirstOrDefaultAsync();

            if (invoice != null)
            {
                invoice.InvoiceDetails = invoice.InvoiceDetails
                    .SelectDetail()
                    .ToList();
            }

            return invoice;
        }

        public async Task<IEnumerable<Invoice>> GetUserInvoices(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<Invoice>();
            }

            return await _context.Invoices
                .Where(i => i.UserId == userId)
                .Include(i => i.InvoiceStatuses)
                .Include(i => i.InvoiceDetails)
                .OrderByDescending(i => i.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetRecentOrders(int numToTake)
        {
            return await _context.Invoices
                .Where(i => !i.IsAccepted)
                .Include(i => i.InvoiceStatuses)
                .Include(i => i.InvoiceDetails)
                .OrderByDescending(i => i.OrderDate)
                .Take(numToTake)
                .ToListAsync();
        }

        public async Task<bool> CheckIsPurchased(string userId, string productId)
        {
            Invoice? invoice = await _context.Invoices
                .Where(i => i.UserId == userId && i.IsCompleted && i.InvoiceDetails.Any(d => d.ProductId == productId))
                .FirstOrDefaultAsync();

            return invoice != null;
        }

        public async Task<bool> CheckIsPurchased(string email, string? phone, string productId)
        {
            Invoice? invoice = await _context.Invoices
                .Where(i => i.InvoiceDetails.Any(d => d.ProductId == productId) 
                        && (i.RecipientPhone == phone || (i.User != null && i.User.Email == email)))
                .FirstOrDefaultAsync();

            return invoice != null;
        }

        public async Task<bool> UpdateInvoice(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddInvoiceStatus(InvoiceStatus invoiceStatus)
        {
            await _context.InvoiceStatuses.AddAsync(invoiceStatus);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateInvoiceStatus(InvoiceStatus invoiceStatus)
        {
            _context.InvoiceStatuses.Update(invoiceStatus);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CancelOrder(string userId, string invoiceId)
        {
            Invoice? invoice = await _context.Invoices
                .Where(i => i.InvoiceId == invoiceId && i.UserId == userId)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return false;
            }

            if(invoice.IsAccepted || invoice.IsCompleted || invoice.PaymentStatus == PaymentContants.Paid)
            {
                return false;
            }

            invoice.IsCancelled = true;
            invoice.CancelledDate = DateTime.Now;

            _context.Invoices.Update(invoice);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CancelOrder(string invoiceId)
        {
            Invoice? invoice = await _context.Invoices
                .Where(i => i.InvoiceId == invoiceId)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return false;
            }

            if (invoice.IsCompleted)
            {
                return false;
            }

            invoice.IsCancelled = true;
            invoice.CancelledDate = DateTime.Now;

            _context.Invoices.Update(invoice);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CompleteOrder(string invoiceId)
        {
            Invoice? invoice = await _context.Invoices
                .Where(i => i.InvoiceId == invoiceId)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return false;
            }

            invoice.PaymentStatus = PaymentContants.Paid;
            invoice.IsCompleted = true;
            invoice.CompletedDate = DateTime.Now;

            if (invoice.PackingSlip != null)
            {
                invoice.PackingSlip.IsCompleted = true;
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedList<Invoice>> GetInvoices(int page, int itemsPerPage, string? filterBy, string? sortBy)
        {
            IQueryable<Invoice> invoices = _context.Invoices
                .Include(i => i.InvoiceStatuses)
                .Include(i => i.InvoiceDetails)
                .Include(i => i.PaymentMed);

            invoices = invoices.SortBy(sortBy).FilterBy(filterBy);
            return await invoices.ToPagedListAsync(page, itemsPerPage);
        }

        public async Task<PagedList<Invoice>> SearchInvoices(string query, int page, int itemsPerPage)
        {
            IQueryable<Invoice> invoices = _context.Invoices
                .Include(i => i.InvoiceStatuses)
                .Include(i => i.InvoiceDetails)
                .Include(i => i.PaymentMed)
                .Where(i => i.InvoiceId.Contains(query) || i.RecipientPhone.Contains(query));

            return await invoices.ToPagedListAsync(page, itemsPerPage);
        }
    }
}
