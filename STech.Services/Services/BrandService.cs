using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Services.Utils;
using STech.Data.ViewModels;

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

        public async Task<PagedList<Brand>> GetAll(string? sort_by, int page, int itemsPerPage)
        
        {
            IQueryable<Brand> brands = _context.Brands.OrderBy(b => b.BrandName);
            brands = brands.Sort(sort_by);

            return await brands.ToPagedListAsync(page, itemsPerPage);
        }

        public async Task<Brand?> GetById(string id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task<bool> Create(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(Brand brand)
        {
            _context.Brands.Update(brand);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(string id)
        {
            Brand? brand = await GetById(id);
            if(brand == null)
            {
                return false;
            }

            _context.Brands.Remove(brand);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
