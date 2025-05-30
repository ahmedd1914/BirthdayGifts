// Models/ViewModels/Voting/VoteResultViewModel.cs
namespace BirthdayGifts.Web.Models.ViewModels.Voting
{
    public class VoteResultViewModel
    {
        public int GiftId { get; set; }
        public string GiftName { get; set; } = string.Empty;
        public int VoteCount { get; set; }
        public double Percentage { get; set; }
    }


public class SessionResultsViewModel
{
    public int SessionId { get; set; }
    public string BirthdayPersonName { get; set; }
    public string CreatedByName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalVotes { get; set; }
    public List<VoteResultViewModel> VoteResults { get; set; }
}
}
