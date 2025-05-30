using System.ComponentModel.DataAnnotations;

namespace BirthdayGifts.Web.Models.ViewModels.Employee
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Username")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters and numbers.")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessage = "Full name cannot be longer than 100 characters.")]
        [MinLength(3, ErrorMessage = "Full name must be at least 3 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1900", "12/31/2025", ErrorMessage = "Date of birth must be between 1900 and 2025.")]
        public DateTime DateOfBirth { get; set; }

        public int DaysToNextBirthday { get; set; }
    }
    public class EmployeeListViewModel
    {
        public List<EmployeeViewModel> Employees { get; set; }
        public int TotalCount { get; set; }
        public List<EmployeeViewModel> UpcomingBirthdays { get; set; }
    }
} 