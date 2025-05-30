using System.ComponentModel.DataAnnotations;

namespace BirthdayGifts.Web.Models.ViewModels.Voting
{
    public class CreateVotingSessionViewModel
    {
        [Required]
        [Display(Name = "Birthday Person")]
        public int BirthdayPersonId { get; set; }

        // Navigation property for display
        [Display(Name = "Birthday Person Name")]
        public string? BirthdayPersonName { get; set; }
    }
} 