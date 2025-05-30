using BirthdayGifts.Models;
using BirthdayGifts.Repository.Interfaces;
using BirthdayGifts.Services.DTOs.Gift;
using BirthdayGifts.Services.Interfaces.Gift;
using AutoMapper;

namespace BirthdayGifts.Services.Implementations.Gift
{
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _giftRepository;
        private readonly IMapper _mapper;

        public GiftService(IGiftRepository giftRepository, IMapper mapper)
        {
            _giftRepository = giftRepository;
            _mapper = mapper;
        }

        public async Task<GiftDto?> GetGiftByIdAsync(int giftId)
        {
            var gift = await _giftRepository.RetrieveByIdAsync(giftId);
            return _mapper.Map<GiftDto>(gift);
        }

        public async Task<IEnumerable<GiftDto>> GetAllGiftsAsync()
        {
            var gifts = await _giftRepository.RetrieveAllAsync();
            return _mapper.Map<IEnumerable<GiftDto>>(gifts);
        }

        public async Task<IEnumerable<GiftDto>> GetFilteredGiftsAsync(GiftFilterDto filter)
        {
            var filterModel = _mapper.Map<GiftFilter>(filter);
            var gifts = await _giftRepository.GetFilteredAsync(filterModel);
            return _mapper.Map<IEnumerable<GiftDto>>(gifts);
        }

        public async Task<GiftDto> CreateGiftAsync(CreateGiftDto giftDto)
        {
            var gift = _mapper.Map<Models.Gift>(giftDto);
            var id = await _giftRepository.CreateAsync(gift);
            gift.GiftId = id;
            return _mapper.Map<GiftDto>(gift);
        }

        public async Task<GiftDto> UpdateGiftAsync(int giftId, GiftUpdateDto giftDto)
        {
            var gift = await _giftRepository.RetrieveByIdAsync(giftId);
            if (gift == null)
                throw new KeyNotFoundException($"Gift with ID {giftId} not found");

            _mapper.Map(giftDto, gift);
            await _giftRepository.UpdateAsync(gift);
            return _mapper.Map<GiftDto>(gift);
        }

        public async Task<bool> DeleteGiftAsync(int giftId)
        {
            return await _giftRepository.DeleteAsync(giftId);
        }

        public async Task<IEnumerable<GiftDto>> GetGiftsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var filter = new GiftFilterDto { MinPrice = minPrice, MaxPrice = maxPrice };
            return await GetFilteredGiftsAsync(filter);
        }
    }
}
