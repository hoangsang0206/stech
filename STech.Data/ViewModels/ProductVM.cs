using System.ComponentModel.DataAnnotations;
using STech.Data.Validations;

namespace STech.Data.ViewModels
{
    public class ProductVM
    {
        [Required(ErrorMessage = "Nhập mã sản phẩm")]
        [RegularExpression(@"^[a-zA-Z0-9-_]*$", ErrorMessage = "Mã sản phẩm không chứa kí tự đặc biệt (ngoại trừ -, _) và khoảng trống")]
        public string ProductId { get; set; } = null!;

        [Required(ErrorMessage = "Nhập tên sản phẩm")]
        public string ProductName { get; set; } = null!;

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public int? ManufacturedYear { get; set; }

        public decimal? OriginalPrice { get; set; }

        [Required(ErrorMessage = "Nhập giá sản phẩm")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        public decimal Price { get; set; }

        public int? Warranty { get; set; } = 0;

        public string? CategoryId { get; set; }

        public string? BrandId { get; set; }

        public List<Image>? Images { get; set; }

        public class Image
        {
            [Required(ErrorMessage = "Nhập đường dẫn ảnh")]
            public string ImageUrl { get; set; } = null!;
        }
    }
}
