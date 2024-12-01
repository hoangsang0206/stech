using STech.Data.Models;

namespace STech.Data.ViewModels
{
    public class AdminHomePageData
    {
        public ProductStatistic ProductStatistic { get; set; } = new ProductStatistic();
        public OrderStatistic OrderStatistic { get; set; } = new OrderStatistic();
        public IEnumerable<User> TopUsers { get; set; } = new List<User>();
        public IEnumerable<Product> TopSellingProducts { get; set; } = new List<Product>();
        public IEnumerable<Invoice> RecentOrders { get; set; } = new List<Invoice>();
    }

    public class ProductStatistic
    {
        public int TotalProducts { get; set; }
        public int CurrentMonthAdded { get; set; }
    }

    public class OrderStatistic
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int LastMonthOrders { get; set; }
        public int CurrentMonthOrders { get; set; }
        public decimal LastMonthRevenue { get; set; }
        public decimal CurrentMonthRevenue { get; set; }
    }
}
