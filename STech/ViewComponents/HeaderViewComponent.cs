using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STech.Data.Models;

namespace STech.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly StechDbContext _dbContext;
        public HeaderViewComponent(StechDbContext dbContext) => _dbContext = dbContext;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<SubHeader> subHeader = await _dbContext.SubHeaders.ToListAsync();
            return View("Header", subHeader);
        }
    }
}
