using STech.Data.Models;
using STech.Services.Constants;

namespace STech.Services.Utils
{
    public static class InvoiceUtils
    {
        public static IEnumerable<InvoiceDetail> SelectDetail(this IEnumerable<InvoiceDetail> details)
        {
            return details.Select(d => new InvoiceDetail
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
            });
        }

        public static IQueryable<Invoice> FilterBy(this IQueryable<Invoice> invoices, string? filterBy)
        {
            if (filterBy == null)
            {
                return invoices;
            }

            switch (filterBy)
            {
                case "paid":
                    return invoices.Where(i => i.PaymentStatus == PaymentContants.Paid)
                        .OrderByDescending(i => i.OrderDate);

                case "unpaid":
                    return invoices.Where(i => i.PaymentStatus == PaymentContants.UnPaid)
                        .OrderByDescending(i => i.OrderDate);

                case "accepted":
                    return invoices.Where(i => i.IsAccepted && !i.IsCancelled && !i.IsCompleted)
                        .OrderByDescending(i => i.OrderDate);

                case "unaccepted":
                    return invoices
                        .Where(i => !i.IsAccepted && !i.IsCancelled && !i.IsCompleted)
                        .OrderByDescending(i => i.OrderDate);

                case "completed":
                    return invoices.Where(i => i.IsCompleted)
                        .OrderByDescending(i => i.OrderDate);

                case "cancelled":
                    return invoices.Where(i => i.IsCancelled)
                        .OrderByDescending(i => i.OrderDate);

                default:
                    return invoices;
            }
        }

        public static IQueryable<Invoice> SortBy(this IQueryable<Invoice> invoices, string? sortBy)
        {
            if (sortBy == null)
            {
                return invoices;
            }

            switch (sortBy)
            {
                case "date-asc":
                    return invoices.OrderBy(i => i.OrderDate);
                case "date-desc":
                    return invoices.OrderByDescending(i => i.OrderDate);
                case "price-asc":
                    return invoices.OrderBy(i => i.Total);
                case "price-desc":
                    return invoices.OrderByDescending(i => i.Total);
                default:
                    return invoices;
            }
        }
    }
}
