using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly StechDbContext _context;

        private readonly int NumOfInvoicePerPage = 20;

        public OrderService(StechDbContext context)
        {
            _context = context;
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
                invoice.InvoiceDetails = invoice.InvoiceDetails.Select(d => new InvoiceDetail
                {
                    InvoiceId = d.InvoiceId,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    Cost = d.Cost,
                    Product = new Product
                    {
                        ProductId = d.Product.ProductId,
                        ProductName = d.Product.ProductName,
                        Warranty = d.Product.Warranty,
                        Price = d.Product.Price,
                        ProductImages = d.Product.ProductImages.OrderBy(t => t.Id).Take(1).ToList(),
                    }
                }).ToList();
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
                invoice.InvoiceDetails = invoice.InvoiceDetails.Select(d => new InvoiceDetail
                {
                    InvoiceId = d.InvoiceId,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    Cost = d.Cost,
                    Product = new Product
                    {
                        ProductId = d.Product.ProductId,
                        ProductName = d.Product.ProductName,
                        Warranty = d.Product.Warranty,
                        Price = d.Product.Price,
                        ProductImages = d.Product.ProductImages.OrderBy(t => t.Id).Take(1).ToList(),
                    }
                }).ToList();
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
                invoice.InvoiceDetails = invoice.InvoiceDetails.Select(d => new InvoiceDetail
                {
                    InvoiceId = d.InvoiceId,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    Cost = d.Cost,
                    Product = new Product
                    {
                        ProductId = d.Product.ProductId,
                        ProductName = d.Product.ProductName,
                        Warranty = d.Product.Warranty,
                        Price = d.Product.Price,
                        ProductImages = d.Product.ProductImages.OrderBy(t => t.Id).Take(1).ToList(),
                    }
                }).ToList();
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

        public async Task<(IEnumerable<Invoice>, int)> GetInvoices(int page, string? filterBy, string? sortBy)
        {
            IEnumerable<Invoice> invoices = await _context.Invoices
                .Include(i => i.InvoiceStatuses)
                .Include(i => i.InvoiceDetails)
                .Include(i => i.PaymentMed)
                .ToListAsync();

            invoices = invoices.FilterBy(filterBy);

            int totalPage = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(invoices.Count()) / Convert.ToDouble(NumOfInvoicePerPage)));

            return (invoices.Paginate(page, NumOfInvoicePerPage).SortBy(sortBy), totalPage);
        }

        
    }
}
