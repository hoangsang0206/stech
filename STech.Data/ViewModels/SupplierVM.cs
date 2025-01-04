using System.ComponentModel.DataAnnotations;

namespace STech.Data.ViewModels;

public class SupplierVM
{
    [Required(ErrorMessage = "Mã nhà cung cấp không được để trống")]
    [RegularExpression(@"^[a-zA-Z0-9-_]*$", ErrorMessage = "Mã nhà cung cấp không chứa kí tự đặc biệt (ngoại trừ -, _) và khoảng trống")]
    public string SupplierId { get; set; } = null!;

    [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
    public string SupplierName { get; set; } = null!;

    [Required(ErrorMessage = "Địa chỉ không được để trống")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [RegularExpression(@"^(0[1-9]\d{8,9})$|^(84[1-9]\d{8,9})$|^\+84[1-9]\d{8,9}$", ErrorMessage = "* Số điện thoại không hợp lệ")]
    public string Phone { get; set; } = null!;
}