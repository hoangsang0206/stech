using Microsoft.EntityFrameworkCore;
using STech.Data.Models;

namespace STech.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly StechDbContext _context;

        public OrderService(StechDbContext context)
        {
            _context = context;
        }

        
    }
}
