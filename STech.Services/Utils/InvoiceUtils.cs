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

        public static IEnumerable<Invoice> Paginate(this IEnumerable<Invoice> invoices, int page, int numToTake)
        {
            if(page <= 0)
            {
                page = 1;
            }

            int numToSkip = (page - 1) * numToTake;

            return invoices.Skip(numToSkip).Take(numToTake).ToList(); ;
        }

        public static IEnumerable<Invoice> FilterBy(this IEnumerable<Invoice> invoices, string? filterBy)
        {
            if (filterBy == null)
            {
                return invoices;
            }

            switch (filterBy)
            {
                case "paid":
                    return invoices.Where(i => i.PaymentStatus == PaymentContants.Paid)
                        .OrderByDescending(i => i.OrderDate).ToList();

                case "unpaid":
                    return invoices.Where(i => i.PaymentStatus == PaymentContants.UnPaid)
                        .OrderByDescending(i => i.OrderDate).ToList();

                case "accepted":
                    return invoices.Where(i => i.IsAccepted && !i.IsCancelled && !i.IsCompleted)
                        .OrderByDescending(i => i.OrderDate).ToList();

                case "unaccepted":
                    return invoices
                        .Where(i => !i.IsAccepted && !i.IsCancelled && !i.IsCompleted)
                        .OrderByDescending(i => i.OrderDate).ToList();

                case "completed":
                    return invoices.Where(i => i.IsCompleted)
                        .OrderByDescending(i => i.OrderDate).ToList();

                case "cancelled":
                    return invoices.Where(i => i.IsCancelled)
                        .OrderByDescending(i => i.OrderDate).ToList();

                default:
                    return invoices;
            }
        }

        public static IEnumerable<Invoice> SortBy(this IEnumerable<Invoice> invoices, string? sortBy)
        {
            if (sortBy == null)
            {
                return invoices;
            }

            switch (sortBy)
            {
                case "date-asc":
                    return invoices.OrderBy(i => i.OrderDate).ToList();
                case "date-desc":
                    return invoices.OrderByDescending(i => i.OrderDate).ToList();
                case "price-asc":
                    return invoices.OrderBy(i => i.Total).ToList();
                case "price-desc":
                    return invoices.OrderByDescending(i => i.Total).ToList();
                default:
                    return invoices;
            }
        }
    }
}
