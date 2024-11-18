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
            return await _context.PaymentMethods
                .OrderBy(p => p.Sort)
                .ToListAsync();
        }

        public async Task<PaymentMethod?> GetPaymentMethod(string id)
        {
            return await _context.PaymentMethods
                .Where(p => p.PaymentMedId == id).
                FirstOrDefaultAsync();
        }

        public async Task<bool> CreatePaymentMethod(PaymentMethod payment)
        {
            await _context.PaymentMethods.AddAsync(payment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ActivePaymentMethod(string paymentId)
        {
            PaymentMethod? payment = await _context.PaymentMethods
                .Where(p => p.PaymentMedId == paymentId)
                .FirstOrDefaultAsync();
            if(payment != null) {
                payment.IsActive = true;

                _context.PaymentMethods.Update(payment);
                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> DeActivePaymentMethod(string paymentId)
        {
            PaymentMethod? payment = await _context.PaymentMethods
                .Where(p => p.PaymentMedId == paymentId)
                .FirstOrDefaultAsync();
            if(payment != null) {
                payment.IsActive = false;

                _context.PaymentMethods.Update(payment);
                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> DeletePaymentMethod(string paymentId)
        {
            PaymentMethod? payment = await _context.PaymentMethods
                .Where(p => p.PaymentMedId == paymentId)
                .FirstOrDefaultAsync();
            if(payment != null) {
                _context.PaymentMethods.Remove(payment);
                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

    }
}
