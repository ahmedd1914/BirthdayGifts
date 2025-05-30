using BirthdayGifts.Services.DTOs.Gift;

namespace BirthdayGifts.Services.Interfaces.Gift
{
    public interface IGiftService
    {
        Task<GiftDto?> GetGiftByIdAsync(int giftId);
        Task<IEnumerable<GiftDto>> GetAllGiftsAsync();
        Task<IEnumerable<GiftDto>> GetFilteredGiftsAsync(GiftFilterDto filter);
        Task<GiftDto> CreateGiftAsync(CreateGiftDto giftDto);
        Task<GiftDto> UpdateGiftAsync(int giftId, GiftUpdateDto giftDto);
        Task<bool> DeleteGiftAsync(int giftId);
        Task<IEnumerable<GiftDto>> GetGiftsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
} 