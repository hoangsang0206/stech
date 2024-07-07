using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Data.ViewModels
{
    public class LoginVM
    {
        [Key]
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "* Tên đăng nhập không để trống")]
        public string UserName { get; set; } = null!;

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "* Mật khẩu không để trống")]
        public string Password { get; set; } = null!;
    }
}
