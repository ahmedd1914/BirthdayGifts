using System.ComponentModel.DataAnnotations;
using BirthdayGifts.Web.Models.ViewModels.Gift;

namespace BirthdayGifts.Web.Models.ViewModels.Voting
{
    public class VoteViewModel
    {
        public int VoteId { get; set; }

        [Required]
        [Display(Name = "Voting Session")]
        public int VotingSessionId { get; set; }

        [Required]
        [Display(Name = "Voter")]
        public int VoterId { get; set; }

        [Required]
        [Display(Name = "Gift")]
        public int GiftId { get; set; }

        [Required]
        [Display(Name = "Voted At")]
        [DataType(DataType.DateTime)]
        public DateTime VotedAt { get; set; }

        // Navigation properties for display
        [Display(Name = "Voter Name")]
        public string? VoterName { get; set; }

        [Display(Name = "Gift Name")]
        public string? GiftName { get; set; }
        public string BirthdayPersonName { get; set; }
        public List<GiftViewModel> AvailableGifts { get; set; }
        public int SelectedGiftId { get; set; }
    }
} 