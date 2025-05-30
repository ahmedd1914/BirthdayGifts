using System;

namespace BirthdayGifts.Repository.Interfaces
{
    public class VotingSessionUpdate
    {
        public int? VoteSessionCreatorId { get; set; }
        public int? BirthdayPersonId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int? Year { get; set; }
    }
} 