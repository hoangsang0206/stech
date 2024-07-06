using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STech.Data.Models;

namespace STech.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly StechDbContext _context;
        public FooterViewComponent(StechDbContext context) => _context = context;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<DeliveryUnit> deliveryUnits = await _context.DeliveryUnits.ToListAsync();
            return View(deliveryUnits);
        }
    }
}
