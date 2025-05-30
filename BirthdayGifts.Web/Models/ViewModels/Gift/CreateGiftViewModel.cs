using System.ComponentModel.DataAnnotations;

namespace BirthdayGifts.Web.Models.ViewModels.Gift
{
    public class CreateGiftViewModel
    {
        [Required]
        [Display(Name = "Name")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Price")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
} 