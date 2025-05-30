using System.ComponentModel.DataAnnotations;

namespace BirthdayGifts.Web.Models.ViewModels.Employee
{
    public class EmployeeFilterViewModel
    {
        [Display(Name = "Username")]
        public string? Username { get; set; }

        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
} 