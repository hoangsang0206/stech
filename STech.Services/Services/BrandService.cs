using Microsoft.EntityFrameworkCore;
using STech.Data.Models;

namespace STech.Services.Services
{
    public class BrandService : IBrandService
    {
        private readonly StechDbContext _context;
        public BrandService(StechDbContext context) => _context = context;

        public async Task<IEnumerable<Brand>> GetAll(bool isExcept)
        {
            if(isExcept)
            {
                return await _context.Brands.Where(b => b.BrandId != "khac").OrderBy(b => b.BrandName).ToListAsync();
            }

            return await _context.Brands.OrderBy(b => b.BrandName).ToListAsync();
        }

        public async Task<Brand?> GetById(string id)
        {
            return await _context.Brands.FindAsync(id);
        }
    }
}
