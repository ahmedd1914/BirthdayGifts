using System;

namespace BirthdayGifts.Services.DTOs.Gift
{
    public class GiftUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
    }
} 