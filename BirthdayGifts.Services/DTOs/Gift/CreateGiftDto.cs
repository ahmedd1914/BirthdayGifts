using System;

namespace BirthdayGifts.Services.DTOs.Gift
{
    public class CreateGiftDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
} 