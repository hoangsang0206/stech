using STech.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace STech.Data.ViewModels
{
    public class WarehouseImportVM
    {
        public string? Note { get; set; }

        public string? Status { get; set; }

        [Required(ErrorMessage = "Mã kho không để trống")]
        public string WarehouseId { get; set; } = null!;

        [Required(ErrorMessage = "Mã nhà cung cấp không để trống")]
        public string SupplierId { get; set; } = null!;

        public virtual List<WarehouseImportDetailVM> WarehouseImportDetails { get; set; } = new List<WarehouseImportDetailVM>();
    }

    public class WarehouseImportDetailVM
    {
        [Required(ErrorMessage = "Mã sản phẩm không để trống")]
        public string ProductId { get; set; } = null!;
        [Required(ErrorMessage = "Số lượng không để trống")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Giá nhập không để trống")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Số lô không để trống")]
        public string BatchNumber { get; set; } = null!;
    }
}
