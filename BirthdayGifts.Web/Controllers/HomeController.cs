using System.Diagnostics;
using BirthdayGifts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using BirthdayGifts.Services.Interfaces.VotingSession;
using BirthdayGifts.Web.Models.ViewModels.Voting;
using AutoMapper;
using BirthdayGifts.Web.Attributes;
using BirthdayGifts.Services.Interfaces.Employee;
using BirthdayGifts.Services.Interfaces.Vote;
using System.Linq;

namespace BirthdayGifts.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVotingSessionService _votingSessionService;
        private readonly IEmployeeService _employeeService;
        private readonly IVoteService _voteService;
        private readonly IMapper _mapper;

        public HomeController(
            ILogger<HomeController> logger,
            IVotingSessionService votingSessionService,
            IEmployeeService employeeService,
            IVoteService voteService,
            IMapper mapper)
        {
            _logger = logger;
            _votingSessionService = votingSessionService;
            _employeeService = employeeService;
            _voteService = voteService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _votingSessionService.GetAllSessionsAsync();
            var viewModels = _mapper.Map<List<VotingSessionViewModel>>(sessions);

            // Filter for completed sessions only (where IsActive is false)
            viewModels = viewModels.Where(vm => !vm.IsActive).ToList();

            var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;

            foreach (var viewModel in viewModels)
            {
                viewModel.IsCreator = viewModel.VoteSessionCreatorId == currentUserId;

                // Fetch and set creator and birthday person names
                var creator = await _employeeService.GetEmployeeByIdAsync(viewModel.VoteSessionCreatorId);
                var birthdayPerson = await _employeeService.GetEmployeeByIdAsync(viewModel.BirthdayPersonId);
                viewModel.CreatorName = creator?.FullName;
                viewModel.BirthdayPersonName = birthdayPerson?.FullName;

                // HasUserVoted and TotalVotes
                var votes = await _voteService.GetVotesBySessionIdAsync(viewModel.VotingSessionId);
                viewModel.HasUserVoted = votes.Any(v => v.VoterId == currentUserId);
                viewModel.TotalVotes = votes.Count();
            }

            return View(viewModels);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
