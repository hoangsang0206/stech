using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STech.Data.Models;

namespace STech.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly StechDbContext dbContext;
        public MenuViewComponent(StechDbContext context) => dbContext = context;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Menu> menu = await dbContext.Menus
                .Include(m => m.MenuLevel1s)
                .ThenInclude(m => m.MenuLevel2s)
                .ToListAsync();
            return View(menu);
        }
    }
}
