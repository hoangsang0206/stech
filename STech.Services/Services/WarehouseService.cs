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
    }
}
