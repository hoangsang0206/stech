using Microsoft.EntityFrameworkCore;
using STech.Data.ViewModels;

namespace STech.Services.Utils;

public static class PaginationUtils
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> superset, int page, int itemsPerPage)
    {
        if (page <= 0)
            page = 1;
        if (itemsPerPage <= 0)
            itemsPerPage = 1;
        
        return new PagedList<T>
        {
            Items = await superset.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync(),
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(superset.Count() / (double)itemsPerPage),
            PageSize = itemsPerPage,
            TotalItems = superset.Count(),
            RemaingItems = superset.Count() - (page * itemsPerPage)
        }; 
    }
}