using Microsoft.EntityFrameworkCore;
using STech.Data.Models;

namespace STech.Services.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly StechDbContext _context;

        public SupplierService(StechDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Supplier>> GetSuppliers()
        {
            return await _context
                .Suppliers
                .ToListAsync();
        }
    }
}
