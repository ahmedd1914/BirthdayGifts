namespace BirthdayGifts.Services.DTOs.Vote
{
    public class CreateVoteDto
    {
        public int VoteSessionId { get; set; }
        public int GiftId { get; set; }
        public int VoterId { get; set; }
    }
} 