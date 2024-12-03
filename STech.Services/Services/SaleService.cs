using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class SaleService : ISaleService
    {
        private readonly StechDbContext _context;

        public SaleService(StechDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sale>> GetActiveSales()
        {
            return await _context.Sales
                .Where(s => s.IsActive == true && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                .SelectSale()
                .ToListAsync();
        }

        public async Task<Sale?> GetSale(string id, string? sortProducts)
        {
            return await _context.Sales
                .Where(s => s.SaleId == id)
                .SelectSale(sortProducts)
                .FirstOrDefaultAsync();
        }
    }
}
