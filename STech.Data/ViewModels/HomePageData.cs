using STech.Data.Models;

namespace STech.Data.ViewModels
{
    public class HomePageData
    {
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Brand> Brands { get; set; } = new List<Brand>();
        public IEnumerable<Category> RandomCategories { get; set; } = new List<Category>();
        public IEnumerable<Slider> Sliders { get; set; } = new List<Slider>();
        public IEnumerable<Product> BestSellingProducts { get; set; } = new List<Product>();
        public IEnumerable<Product> NewProducts { get; set; } = new List<Product>();
        public IEnumerable<ProductGroup> ProductGroups { get; set; } = new List<ProductGroup>();
        public IEnumerable<Sale> Sales { get; set; } = new List<Sale>();
    }
}
