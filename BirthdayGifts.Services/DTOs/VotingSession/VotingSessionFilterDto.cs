using System;

namespace BirthdayGifts.Services.DTOs.VotingSession
{
    public class VotingSessionFilterDto
    {
        public int? VoteSessionCreatorId { get; set; }
        public int? BirthdayPersonId { get; set; }
        public bool? IsActive { get; set; }
        public int? Year { get; set; }
    }
} 