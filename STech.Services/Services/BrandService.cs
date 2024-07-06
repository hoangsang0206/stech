using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return await _context.Brands.Where(b => b.BrandId != "khac").ToListAsync();
            }

            return await _context.Brands.ToListAsync();
        }
    }
}
