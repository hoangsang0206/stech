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

        public async Task<Supplier?> GetSupplierById(string id)
        {
            return await _context.Suppliers.FindAsync(id);
        }
        
        public async Task<Supplier?> GetSupplierByIdWithImports(string id)
        {
            return await _context.Suppliers
                .Where(s => s.SupplierId == id)
                .Include(s => s.WarehouseImports)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateSupplier(Supplier supplier)
        {
            Supplier? existingSupplier = await GetSupplierById(supplier.SupplierId);
            
            if (existingSupplier != null)
            {
                return false;
            }
            
            _context.Suppliers.Add(supplier);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateSupplier(Supplier supplier)
        {
            Supplier? existingSupplier = await GetSupplierById(supplier.SupplierId);
            
            if (existingSupplier == null)
            {
                return false;
            }
            
            existingSupplier.SupplierName = supplier.SupplierName;
            existingSupplier.Address = supplier.Address;
            existingSupplier.Phone = supplier.Phone;
            
            _context.Suppliers.Update(existingSupplier);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteSupplier(string id)
        {
            Supplier? supplier = await GetSupplierByIdWithImports(id);
            
            if (supplier == null)
            {
                return false;
            }

            if (supplier.WarehouseImports.Any())
            {
                return false;
            }
            
            _context.Suppliers.Remove(supplier);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
