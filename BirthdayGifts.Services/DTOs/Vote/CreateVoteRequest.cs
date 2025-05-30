namespace BirthdayGifts.Services.DTOs.Vote
{
    public class CreateVoteRequest
    {
        public int VotingSessionId { get; set; }
        public int GiftId { get; set; }
        public int VoterId { get; set; }
    }
}