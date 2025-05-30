using System.ComponentModel.DataAnnotations;
using BirthdayGifts.Web.Models.ViewModels.Gift;

namespace BirthdayGifts.Web.Models.ViewModels.Voting
{
    public class CreateVoteViewModel
    {
        public int VotingSessionId { get; set; }

        [Required(ErrorMessage = "Please select a gift")]
        [Display(Name = "Select Gift")]
        public int GiftId { get; set; }

        public List<GiftViewModel> AvailableGifts { get; set; } = new();

        // Navigation properties for display
        [Display(Name = "Gift Name")]
        public string? GiftName { get; set; }

        [Display(Name = "Gift Description")]
        public string? GiftDescription { get; set; }

        [Display(Name = "Gift Price")]
        [DataType(DataType.Currency)]
        public decimal? GiftPrice { get; set; }
    }
} 