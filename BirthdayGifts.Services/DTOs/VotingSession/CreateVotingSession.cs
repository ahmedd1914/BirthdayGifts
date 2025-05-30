namespace BirthdayGifts.Services.DTOs.VotingSession
{
    public class CreateVotingSession
    {
        public int VoteSessionCreatorId { get; set; }
        public int BirthdayPersonId { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}