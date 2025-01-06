using STech.Data.Models;

namespace STech.Data.ViewModels;

public class CollectionPageData
{
    public PagedList<Product> Products { get; set; } = new PagedList<Product>();
    public IEnumerable<Breadcrumb> Breadcrumbs { get; set; } = new List<Breadcrumb>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public IEnumerable<Brand> Brands { get; set; } = new List<Brand>();
}