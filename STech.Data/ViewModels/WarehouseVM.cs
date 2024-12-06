using System.ComponentModel.DataAnnotations;

namespace STech.Data.ViewModels
{
    public class WarehouseVM
    {
        [Required(ErrorMessage = "Mã kho không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9-_]*$", ErrorMessage = "Mã kho không chứa kí tự đặc biệt (ngoại trừ -, _) và khoảng trống")]
        public string WarehouseId { get; set; } = null!;

        [Required(ErrorMessage = "Tên kho không được để trống")]
        public string WarehouseName { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; } = null!;

        [Required]
        public string WardCode { get; set; } = null!;

        [Required]
        public string DistrictCode { get; set; } = null!;

        [Required]
        public string ProvinceCode { get; set; } = null!;
    }
}
