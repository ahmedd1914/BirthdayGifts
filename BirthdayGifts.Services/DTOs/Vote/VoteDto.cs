namespace BirthdayGifts.Services.DTOs.Vote
{
    public class VoteDto
    {
        public int VoteId { get; set; }
        public int VoteSessionId { get; set; }
        public int GiftId { get; set; }
        public int VoterId { get; set; }
    }
}