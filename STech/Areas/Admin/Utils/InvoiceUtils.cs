using STech.Data.Models;

namespace STech.Areas.Admin.Utils
{
    public static class InvoiceUtils
    {
        public static IEnumerable<Invoice> Paginate(this IEnumerable<Invoice> invoices, int page, int numToTake)
        {
            if(page <= 0)
            {
                page = 1;
            }

            int numToSkip = (page - 1) * 10;

            return invoices.Skip(numToSkip).Take(numToTake).ToList(); ;
        }

        public static IEnumerable<Invoice> FilterBy(this IEnumerable<Invoice> invoices, string filterBy)
        {
            if (filterBy == null)
            {
                return invoices;
            }

            switch (filterBy)
            {
                case "newest":
                    return invoices
                        .Where(i => i.IsAccepted == false && i.IsCancelled == false && i.IsCompleted == false)
                        .OrderByDescending(i => i.OrderDate).ToList();
                case "paid":
                    return invoices.Where(i => i.PaymentMed != null).ToList();
                case "unpaid":
                    return invoices.Where(i => i.PaymentMed == null).ToList();
                default:
                    break;
            }

            return invoices.Where(i => i.InvoiceId.Contains(filterBy)).ToList();
        }
    }
}
