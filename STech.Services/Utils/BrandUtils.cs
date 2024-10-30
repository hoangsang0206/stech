using STech.Data.Models;

namespace STech.Services.Utils
{
    public static class BrandUtils
    {
        public static IQueryable<Brand> Sort(this IQueryable<Brand> brands, string? sort_by)
        {
            if(sort_by == "name_desc")
            {
                return brands.OrderByDescending(b => b.BrandName);
            }

            return brands.OrderBy(b => b.BrandName);
        }
    }
}
