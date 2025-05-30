using System.ComponentModel.DataAnnotations;

namespace BirthdayGifts.Web.Models.ViewModels.Voting
{
    public class VotingSessionViewModel
    {
        public int VotingSessionId { get; set; }

        [Required]
        [Display(Name = "Creator")]
        public int VoteSessionCreatorId { get; set; }

        [Required]
        [Display(Name = "Birthday Person")]
        public int BirthdayPersonId { get; set; }

        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [Display(Name = "Started At")]
        [DataType(DataType.DateTime)]
        public DateTime StartedAt { get; set; }

        [Display(Name = "Ended At")]
        [DataType(DataType.DateTime)]
        public DateTime? EndedAt { get; set; }

        [Required]
        [Display(Name = "Year")]
        public int Year { get; set; }

        // Navigation properties for display
        [Display(Name = "Creator Name")]
        public string? CreatorName { get; set; }

        [Display(Name = "Birthday Person Name")]
        public string? BirthdayPersonName { get; set; }
        public int TotalVotes { get; set; }
        public bool IsCreator { get; set; }
            public bool HasUserVoted { get; set; }

        // Collection of votes for this session
        public IEnumerable<VoteViewModel>? Votes { get; set; }

        // Vote results
        public List<VoteResultViewModel> VoteResults { get; set; } = new();
    }
    public class VotingSessionListViewModel
    {
        public List<VotingSessionViewModel> ActiveSessions { get; set; } = new();
        public int TotalCount { get; set; }
    }

}