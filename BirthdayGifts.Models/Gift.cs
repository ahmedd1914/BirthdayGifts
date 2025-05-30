using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayGifts.Models
{
    public class Gift
    {
        [Key]
        public int GiftId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [Display(Name = "Gift Name")]
        [DataType(DataType.Text)]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?]+$", ErrorMessage = "Name can only contain letters, numbers, and basic punctuation.")]
        public string Name { get; set; }
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        [Display(Name = "Gift Description")]
        [DataType(DataType.MultilineText)]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?]+$", ErrorMessage = "Description can only contain letters, numbers, and basic punctuation.")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Gift Price")]
        [DataType(DataType.Currency)]
        [Range(0.01, 10000.00, ErrorMessage = "Gift price must be between 0.01 and 10,000.00.")]
        public decimal Price { get; set; }       
    }
}
