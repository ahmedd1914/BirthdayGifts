namespace BirthdayGifts.Services.DTOs.VotingSession
{
    public class VotingSessionDto
    {
        public int VoteSessionId { get; set; }
        public int VoteSessionCreatorId { get; set; }
        public int BirthdayPersonId { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int Year { get; set; }
    }
}