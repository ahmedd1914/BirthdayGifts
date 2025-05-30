using System.ComponentModel.DataAnnotations;

namespace BirthdayGifts.Web.Models.ViewModels.Employee
{
    public class CreateEmployeeViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters and numbers.")]
        [Display(Name = "Username")]
        [DataType(DataType.Text)]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password cannot be longer than 100 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{6,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Full name cannot be longer than 100 characters.")]
        [Display(Name = "Full Name")]
        [DataType(DataType.Text)]
        [MinLength(3, ErrorMessage = "Full name must be at least 3 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [Range(typeof(DateTime), "1/1/1900", "12/31/2025", ErrorMessage = "Date of birth must be between 1900 and 2025.")]
        public DateTime DateOfBirth { get; set; }
    }
} 