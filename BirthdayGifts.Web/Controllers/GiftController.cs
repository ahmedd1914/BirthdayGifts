using Microsoft.AspNetCore.Mvc;
using BirthdayGifts.Services.Interfaces.Gift;
using BirthdayGifts.Services.DTOs.Gift;
using BirthdayGifts.Web.Models.ViewModels.Gift;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using BirthdayGifts.Web.Attributes;
namespace BirthdayGifts.Web.Controllers
{
    [Authorize]
    public class GiftController : Controller
    {
        private readonly IGiftService _giftService;
        private readonly IMapper _mapper;

        public GiftController(IGiftService giftService, IMapper mapper)
        {
            _giftService = giftService;
            _mapper = mapper;
        }

        // GET: Gift
        public async Task<IActionResult> Index()
        {
            var gifts = await _giftService.GetAllGiftsAsync();
            var viewModels = _mapper.Map<IEnumerable<GiftViewModel>>(gifts);
            return View(viewModels);
        }

        // GET: Gift/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var gift = await _giftService.GetGiftByIdAsync(id);
            if (gift == null)
            {
                return NotFound();
            }
            var viewModel = _mapper.Map<GiftViewModel>(gift);
            return View(viewModel);
        }

        // GET: Gift/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Gift/Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateGiftViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var createDto = _mapper.Map<CreateGiftDto>(viewModel);
                    await _giftService.CreateGiftAsync(createDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(viewModel);
        }

        // GET: Gift/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var gift = await _giftService.GetGiftByIdAsync(id);
            if (gift == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<EditGiftViewModel>(gift);
            return View(viewModel);
        }

        // POST: Gift/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditGiftViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var updateDto = _mapper.Map<GiftUpdateDto>(viewModel);
                    await _giftService.UpdateGiftAsync(id, updateDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(viewModel);
        }

        // GET: Gift/Search
        public async Task<IActionResult> Search(GiftFilterViewModel filter)
        {
            if (filter == null)
            {
                filter = new GiftFilterViewModel();
            }

            var filterDto = _mapper.Map<GiftFilterDto>(filter);
            var gifts = await _giftService.GetFilteredGiftsAsync(filterDto);
            var viewModels = _mapper.Map<IEnumerable<GiftViewModel>>(gifts);
            return View("Index", viewModels);
        }

        // GET: Gift/PriceRange
        public async Task<IActionResult> PriceRange(decimal minPrice, decimal maxPrice)
        {
            var gifts = await _giftService.GetGiftsByPriceRangeAsync(minPrice, maxPrice);
            var viewModels = _mapper.Map<IEnumerable<GiftViewModel>>(gifts);
            return View("Index", viewModels);
        }
    }
}
