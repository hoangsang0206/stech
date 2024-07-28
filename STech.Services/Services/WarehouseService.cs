using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly StechDbContext _context;

        public WarehouseService(StechDbContext context)
        {
            _context = context;
        }

        public async Task<Warehouse?> GetOnlineWarehouse()
        {
            return await _context.Warehouses.Where(t => t.Type == "online").FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<WarehouseProduct>> GetWarehouseProducts(string productId)
        {
            return await _context.WarehouseProducts
                .Where(p => p.ProductId == productId)
                .Include(p => p.Warehouse)
                .ToListAsync();
        }

        public async Task<bool> SubtractProductQuantity(string warehouseId, string productId, int quantity)
        {
            WarehouseProduct? whP = await _context.WarehouseProducts
                .Where(wp => wp.ProductId == productId && wp.WarehouseId == warehouseId).FirstOrDefaultAsync();
            if (whP != null)
            {
                whP.Quantity -= quantity;
                _context.WarehouseProducts.Update(whP);
                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> SubtractProductQuantity(Invoice invoice)
        {
            if (invoice == null)
            {
                return false;
            }

            foreach (InvoiceDetail detail in invoice.InvoiceDetails)
            {
                
            }

            return true;
        }
    }
}
