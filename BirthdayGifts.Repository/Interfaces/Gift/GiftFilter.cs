using System;

namespace BirthdayGifts.Repository.Interfaces
{
    public class GiftFilter
    {
        public string? Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
} 