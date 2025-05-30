using Microsoft.AspNetCore.Mvc;
using BirthdayGifts.Services.Interfaces.VotingSession;
using BirthdayGifts.Services.Interfaces.Vote;
using BirthdayGifts.Services.DTOs.VotingSession;
using BirthdayGifts.Services.DTOs.Vote;
using BirthdayGifts.Web.Models.ViewModels.Voting;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using BirthdayGifts.Web.Attributes;
using BirthdayGifts.Services.Interfaces.Employee;
using System.Linq;
using BirthdayGifts.Services.Interfaces.Gift;
using System.Security.Claims;
using AuthorizeAttribute = BirthdayGifts.Web.Attributes.AuthorizeAttribute;
using BirthdayGifts.Web.Models.ViewModels.Gift;
using Microsoft.Extensions.Logging;

namespace BirthdayGifts.Web.Controllers
{
    [Authorize]
    public class VotingController : Controller
    {
        private readonly IVotingSessionService _votingSessionService;
        private readonly IVoteService _voteService;
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;
        private readonly IGiftService _giftService;
        private readonly ILogger<VotingController> _logger;

        public VotingController(
            IVotingSessionService votingSessionService,
            IVoteService voteService,
            IMapper mapper,
            IEmployeeService employeeService,
            IGiftService giftService,
            ILogger<VotingController> logger)
        {
            _votingSessionService = votingSessionService;
            _voteService = voteService;
            _mapper = mapper;
            _employeeService = employeeService;
            _giftService = giftService;
            _logger = logger;
        }

        // GET: Voting
        public async Task<IActionResult> Index()
        {
            var sessions = await _votingSessionService.GetActiveSessionsAsync();
            var viewModels = _mapper.Map<List<VotingSessionViewModel>>(sessions);
            var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            // Filter out sessions where the current user is the birthday person
            viewModels = viewModels.Where(vm => vm.BirthdayPersonId != currentUserId).ToList();
            foreach (var viewModel in viewModels)
            {
                viewModel.IsCreator = viewModel.VoteSessionCreatorId == currentUserId;
                var votes = await _voteService.GetVotesBySessionIdAsync(viewModel.VotingSessionId);
                viewModel.HasUserVoted = votes.Any(v => v.VoterId == currentUserId);
                viewModel.TotalVotes = votes.Count();
                var creator = await _employeeService.GetEmployeeByIdAsync(viewModel.VoteSessionCreatorId);
                var birthdayPerson = await _employeeService.GetEmployeeByIdAsync(viewModel.BirthdayPersonId);
                viewModel.CreatorName = creator?.FullName;
                viewModel.BirthdayPersonName = birthdayPerson?.FullName;
            }
            return View(viewModels);
        }

        // GET: Voting/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var session = await _votingSessionService.GetSessionByIdAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (session.BirthdayPersonId == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot view details of your own birthday session.";
                return RedirectToAction("Index");
            }
            var viewModel = _mapper.Map<VotingSessionViewModel>(session);
            var votes = await _voteService.GetVotesBySessionIdAsync(id);
            viewModel.Votes = _mapper.Map<IEnumerable<VoteViewModel>>(votes);
            viewModel.TotalVotes = votes.Count();
            var creator = await _employeeService.GetEmployeeByIdAsync(viewModel.VoteSessionCreatorId);
            var birthdayPerson = await _employeeService.GetEmployeeByIdAsync(viewModel.BirthdayPersonId);
            viewModel.CreatorName = creator?.FullName;
            viewModel.BirthdayPersonName = birthdayPerson?.FullName;
            viewModel.IsCreator = viewModel.VoteSessionCreatorId == currentUserId;
            viewModel.HasUserVoted = votes.Any(v => v.VoterId == currentUserId);

            // Populate VoteResults
            var gifts = await _giftService.GetAllGiftsAsync();
            var giftDict = gifts.ToDictionary(g => g.GiftId, g => g.Name);
            var voteResults = votes
                .GroupBy(v => v.GiftId)
                .Select(g => new VoteResultViewModel
                {
                    GiftId = g.Key,
                    GiftName = giftDict.ContainsKey(g.Key) ? giftDict[g.Key] : "Unknown Gift",
                    VoteCount = g.Count(),
                    Percentage = (double)g.Count() / votes.Count() * 100
                })
                .OrderByDescending(r => r.VoteCount)
                .ToList();
            viewModel.VoteResults = voteResults;

            return View(viewModel);
        }

        // GET: Voting/Create
        public async Task<IActionResult> Create(int? birthdayPersonId)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var model = new CreateVotingSessionViewModel();
            var employees = await _employeeService.GetAllEmployeesAsync();
            ViewBag.Employees = employees;
            if (birthdayPersonId.HasValue)
            {
                if (birthdayPersonId.Value == currentUserId)
                {
                    TempData["ErrorMessage"] = "You cannot start a voting session for yourself.";
                    return RedirectToAction("Index");
                }
                model.BirthdayPersonId = birthdayPersonId.Value;
                var selected = employees.FirstOrDefault(e => e.EmployeeId == birthdayPersonId.Value);
                if (selected != null)
                    model.BirthdayPersonName = selected.FullName;
            }
            return View(model);
        }

        // POST: Voting/Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateVotingSessionViewModel viewModel)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (viewModel.BirthdayPersonId == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot start a voting session for yourself.";
                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid)
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                ViewBag.Employees = employees;
                return View(viewModel);
            }
            try
            {
                var createDto = _mapper.Map<CreateVotingSessionDto>(viewModel);
                createDto.VoteSessionCreatorId = currentUserId;
                createDto.IsActive = true;
                await _votingSessionService.CreateSessionAsync(createDto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var employees = await _employeeService.GetAllEmployeesAsync();
                ViewBag.Employees = employees;
                return View(viewModel);
            }
        }

        // GET: Voting/Vote/5
        public async Task<IActionResult> Vote(int id)
        {
            var session = await _votingSessionService.GetSessionByIdAsync(id);
            if (session == null || !session.IsActive)
            {
                return NotFound();
            }
            var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (session.BirthdayPersonId == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot vote in your own birthday session.";
                return RedirectToAction("Index");
            }
            var votes = await _voteService.GetVotesBySessionIdAsync(id);
            if (votes.Any(v => v.VoterId == currentUserId))
            {
                TempData["ErrorMessage"] = "You have already voted in this session.";
                return RedirectToAction(nameof(Details), new { id });
            }
            var gifts = await _giftService.GetAllGiftsAsync();
            var viewModel = new CreateVoteViewModel
            {
                VotingSessionId = id,
                AvailableGifts = _mapper.Map<List<GiftViewModel>>(gifts)
            };
            return View(viewModel);
        }

        // POST: Voting/Vote/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int id, CreateVoteViewModel viewModel)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null || currentUserId == 0)
            {
                TempData["ErrorMessage"] = "You must be logged in to vote.";
                return RedirectToAction("Login", "Account");
            }
            var session = await _votingSessionService.GetSessionByIdAsync(id);
            if (session.BirthdayPersonId == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot vote in your own birthday session.";
                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid)
            {
                var gifts = await _giftService.GetAllGiftsAsync();
                viewModel.AvailableGifts = _mapper.Map<List<GiftViewModel>>(gifts);
                return View(viewModel);
            }
            try
            {
                var createDto = new CreateVoteDto
                {
                    VoteSessionId = id,
                    GiftId = viewModel.GiftId,
                    VoterId = currentUserId.Value
                };
                await _voteService.CreateVoteAsync(createDto);
                TempData["SuccessMessage"] = "Your vote has been recorded successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var gifts = await _giftService.GetAllGiftsAsync();
                viewModel.AvailableGifts = _mapper.Map<List<GiftViewModel>>(gifts);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your vote. Please try again.");
                var gifts = await _giftService.GetAllGiftsAsync();
                viewModel.AvailableGifts = _mapper.Map<List<GiftViewModel>>(gifts);
                return View(viewModel);
            }
        }

        // POST: Voting/End/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> End(int id)
        {
            try
            {
                var session = await _votingSessionService.GetSessionByIdAsync(id);
                if (session == null)
                {
                    return NotFound();
                }

                // Verify current user is the creator
                var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                if (session.VoteSessionCreatorId != currentUserId)
                {
                    TempData["ErrorMessage"] = "You are not authorized to end this voting session.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var updateDto = new VotingSessionUpdateDto
                {
                    IsActive = false,
                    EndedAt = DateTime.Now
                };

                await _votingSessionService.UpdateSessionAsync(id, updateDto);
                TempData["SuccessMessage"] = "Voting session has been ended successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending voting session {SessionId}", id);
                TempData["ErrorMessage"] = $"An error occurred while ending the session: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Voting/Results/5
        public async Task<IActionResult> Results(int id)
        {
            var session = await _votingSessionService.GetSessionByIdAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (session.BirthdayPersonId == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot view results of your own birthday session.";
                return RedirectToAction("Index");
            }
            var viewModel = _mapper.Map<VotingSessionViewModel>(session);
            var votes = await _voteService.GetVotesBySessionIdAsync(id);
            viewModel.Votes = _mapper.Map<IEnumerable<VoteViewModel>>(votes);
            viewModel.TotalVotes = votes.Count();
            var creator = await _employeeService.GetEmployeeByIdAsync(viewModel.VoteSessionCreatorId);
            var birthdayPerson = await _employeeService.GetEmployeeByIdAsync(viewModel.BirthdayPersonId);
            viewModel.CreatorName = creator?.FullName;
            viewModel.BirthdayPersonName = birthdayPerson?.FullName;
            var gifts = await _giftService.GetAllGiftsAsync();
            var giftDict = gifts.ToDictionary(g => g.GiftId, g => g.Name);
            var voteResults = votes
                .GroupBy(v => v.GiftId)
                .Select(g => new VoteResultViewModel
                {
                    GiftId = g.Key,
                    GiftName = giftDict.ContainsKey(g.Key) ? giftDict[g.Key] : "Unknown Gift",
                    VoteCount = g.Count(),
                    Percentage = (double)g.Count() / votes.Count() * 100
                })
                .OrderByDescending(r => r.VoteCount)
                .ToList();
            viewModel.VoteResults = voteResults;
            return View(viewModel);
        }
    }
}
