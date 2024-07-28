using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STech.Data.Models;

namespace STech.ViewComponents
{
    public class DeliveryViewComponent : ViewComponent
    {
        private readonly StechDbContext _context;

        public DeliveryViewComponent(StechDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<DeliveryMethod> methods = await _context.DeliveryMethods.ToListAsync();

            return View(methods);
        }
    }
}
