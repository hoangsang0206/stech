using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly StechDbContext _context;

        public DeliveryService(StechDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryMethod?> GetDeliveryMethodById(string id)
        {
            return await _context.DeliveryMethods.FindAsync(id);
        }
    }
}
