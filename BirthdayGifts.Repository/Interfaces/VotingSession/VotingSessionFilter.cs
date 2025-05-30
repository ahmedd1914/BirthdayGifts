using System;

namespace BirthdayGifts.Repository.Interfaces
{
    public class VotingSessionFilter
    {
        public int? VoteSessionCreatorId { get; set; }
        public int? BirthdayPersonId { get; set; }
        public bool? IsActive { get; set; }
        public int? Year { get; set; }
    }
} 