using System;

namespace BirthdayGifts.Services.DTOs.Vote
{
    public class VoteUpdateDto
    {
        public int? VoteSessionId { get; set; }
        public int? VoterId { get; set; }
        public int? GiftId { get; set; }
    }
} 