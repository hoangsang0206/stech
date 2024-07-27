using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly StechDbContext _context;

        public PaymentService(StechDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods()
        {
            return await _context.PaymentMethods.OrderBy(p => p.Sort).ToListAsync();
        }

        public async Task<PaymentMethod?> GetPaymentMethod(string id)
        {
            return await _context.PaymentMethods.Where(p => p.PaymentMedId == id).FirstOrDefaultAsync();
        }
    }
}
