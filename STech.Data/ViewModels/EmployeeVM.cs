using System.ComponentModel.DataAnnotations;

namespace STech.Data.ViewModels
{
    public class EmployeeVM
    {
        [Required]
        public string EmployeeName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime? Dob { get; set; }

        public string Gender { get; set; } = null!;

        public string CitizenId { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string WardCode { get; set; } = null!;

        public string DistrictCode { get; set; } = null!;

        public string ProvinceCode { get; set; } = null!;

        [Required]
        public string LoginPassword { get; set; } = null!;

        public int? GroupId { get; set; }
    }
}
