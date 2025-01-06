using STech.Data.Models;

namespace STech.Data.ViewModels;

public class CollectionFilter
{
    public IEnumerable<Brand> Brands { get; set; } = new List<Brand>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    
    // Specs
}