using System;

namespace BirthdayGifts.Repository.Interfaces
{
    public class VoteFilter
    {
        public int? VoteSessionId { get; set; }
        public int? VoterId { get; set; }
        public int? GiftId { get; set; }
    }
} 