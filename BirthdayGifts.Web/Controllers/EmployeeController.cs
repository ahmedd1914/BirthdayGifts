using Microsoft.AspNetCore.Mvc;
using BirthdayGifts.Services.Interfaces.Employee;
using BirthdayGifts.Services.DTOs.Employee;
using BirthdayGifts.Web.Models.ViewModels.Employee;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using System.Linq;
using BirthdayGifts.Web.Attributes;

namespace BirthdayGifts.Web.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeeController(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var viewModels = _mapper.Map<List<EmployeeViewModel>>(employees);

            // Calculate DaysToNextBirthday for each employee
            foreach (var employee in viewModels)
            {
                var today = DateTime.Today;
                var nextBirthday = new DateTime(today.Year, employee.DateOfBirth.Month, employee.DateOfBirth.Day);
                if (nextBirthday < today)
                    nextBirthday = nextBirthday.AddYears(1);
                employee.DaysToNextBirthday = (nextBirthday - today).Days;
            }

            var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            // Get upcoming birthdays (next 30 days, top 5), excluding the logged-in user
            var upcoming = viewModels
                .Where(e => e.DaysToNextBirthday > 0 && e.DaysToNextBirthday <= 30 && e.EmployeeId != currentUserId)
                .OrderBy(e => e.DaysToNextBirthday)
                .Take(5)
                .ToList();

            var model = new EmployeeListViewModel
            {
                Employees = viewModels,
                UpcomingBirthdays = upcoming,
                TotalCount = viewModels.Count
            };

            return View(model);
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            var viewModel = _mapper.Map<EmployeeViewModel>(employee);
            return View(viewModel);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var createDto = _mapper.Map<CreateEmployeeDto>(viewModel);
                var result = await _employeeService.CreateEmployeeAsync(createDto);
                TempData["SuccessMessage"] = "Employee created successfully.";
                return RedirectToAction(nameof(Details), new { id = result.EmployeeId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(viewModel);
            }
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<EditEmployeeViewModel>(employee);
            return View(viewModel);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditEmployeeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var updateDto = _mapper.Map<EmployeeUpdateDto>(viewModel);
                var result = await _employeeService.UpdateEmployeeAsync(id, updateDto);
                TempData["SuccessMessage"] = "Employee updated successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(viewModel);
            }
        }

        // GET: Employee/UpcomingBirthdays
        public async Task<IActionResult> UpcomingBirthdays(int daysAhead = 30)
        {
            var employees = await _employeeService.GetEmployeesWithUpcomingBirthdaysAsync(daysAhead);
            var viewModels = _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
            return View(viewModels);
        }

        // GET: Employee/Search
        public async Task<IActionResult> Search(EmployeeFilterViewModel filter)
        {
            if (filter == null)
            {
                filter = new EmployeeFilterViewModel();
            }

            var filterDto = _mapper.Map<EmployeeFilterDto>(filter);
            var employees = await _employeeService.GetFilteredEmployeesAsync(filterDto);
            var viewModels = _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
            return View(viewModels);
        }
    }
}
