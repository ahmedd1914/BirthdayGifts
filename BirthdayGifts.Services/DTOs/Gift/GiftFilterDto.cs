using System;

namespace BirthdayGifts.Services.DTOs.Gift
{
    public class GiftFilterDto
    {
        public string? Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
} 