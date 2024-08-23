using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STech.Data.Models;

namespace STech.Areas.Admin.ViewComponents
{
    public class SelectWarehousesViewComponent : ViewComponent
    {
        private readonly StechDbContext _context;

        public SelectWarehousesViewComponent(StechDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Warehouse> warehouses = await _context.Warehouses.ToListAsync();

            return View(warehouses);
        }
    }
}
