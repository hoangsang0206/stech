using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Data.ViewModels
{
    public class AddAddressVM
    {
        [Required(ErrorMessage = "* Địa chỉ không để trống")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "* Phường/xã không để trống")]
        public string WardCode { get; set; } = null!;

        [Required(ErrorMessage = "* Quận/huyện không để trống")]
        public string DistrictCode { get; set; } = null!;

        [Required(ErrorMessage = "* Tỉnh/thành phố không để trống")]
        public string CityCode { get; set; } = null!;

        [Required(ErrorMessage = "* Loại địa chỉ không để trống")]
        public string Type { get; set; } = "home";

        [Required(ErrorMessage = "* Tên người nhận không để trống")]
        public string Recipient { get; set; } = null!;

        [Required(ErrorMessage = "* Số điện thoại không để trống")]
        [RegularExpression(@"^(0[1-9]\d{8,9})$|^(84[1-9]\d{8,9})$|^\+84[1-9]\d{8,9}$", ErrorMessage = "* Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = null!;
    }
}
