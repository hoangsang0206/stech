using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STech.Data.Models;

namespace STech.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly StechDbContext _dbContext;
        public HeaderViewComponent(StechDbContext dbContext) => _dbContext = dbContext;

        public async Task<IViewComponentResult> InvokeAsync(string type = "")
        {
            switch(type)
            {
                case "subheader":
                    IEnumerable<SubHeader> subHeader = await _dbContext.SubHeaders.ToListAsync();
                    return View(subHeader);
                case "mobile-subheader":
                    IEnumerable<Category> categories = await _dbContext.Categories
                    .Where(c => c.CategoryId != "khac")
                    .Select(c => new Category()
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                    })
                    .ToListAsync();
                    return View("MobileSubHeader", categories);
            }

            return View();
        }
    }
}
