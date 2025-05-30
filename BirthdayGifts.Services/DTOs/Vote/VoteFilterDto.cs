using System;

namespace BirthdayGifts.Services.DTOs.Vote
{
    public class VoteFilterDto
    {
        public int? VoteSessionId { get; set; }
        public int? VoterId { get; set; }
        public int? GiftId { get; set; }
    }
} 