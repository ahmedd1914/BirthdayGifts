using System.ComponentModel.DataAnnotations;

namespace BirthdayGifts.Web.Models.ViewModels.Gift
{
    public class GiftFilterViewModel
    {
        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Min Price")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00")]
        [DataType(DataType.Currency)]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Max Price")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00")]
        [DataType(DataType.Currency)]
        public decimal? MaxPrice { get; set; }
    }
} 