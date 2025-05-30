using BirthdayGifts.Models;
using BirthdayGifts.Repository.Interfaces;
using BirthdayGifts.Services.DTOs.Employee;
using BirthdayGifts.Services.Helpers;
using BirthdayGifts.Services.Interfaces.Employee;
using AutoMapper;

namespace BirthdayGifts.Services.Implementations.Employee
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _employeeRepository.RetrieveByIdAsync(employeeId);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.RetrieveAllAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<IEnumerable<EmployeeDto>> GetFilteredEmployeesAsync(EmployeeFilterDto filter)
        {
            var filterModel = _mapper.Map<EmployeeFilter>(filter);
            var employees = await _employeeRepository.GetFilteredAsync(filterModel);
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto employeeDto)
        {
            var employee = _mapper.Map<Models.Employee>(employeeDto);
            var id = await _employeeRepository.CreateAsync(employee);
            employee.EmployeeId = id;
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(int employeeId, EmployeeUpdateDto employeeDto)
        {
            var employee = await _employeeRepository.RetrieveByIdAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found");

            _mapper.Map(employeeDto, employee);
            await _employeeRepository.UpdateAsync(employee);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            return await _employeeRepository.DeleteAsync(employeeId);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesWithUpcomingBirthdaysAsync(int daysAhead)
        {
            var employees = await _employeeRepository.GetEmployeesWithUpcomingBirthdaysAsync(daysAhead);
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }
    }
}
