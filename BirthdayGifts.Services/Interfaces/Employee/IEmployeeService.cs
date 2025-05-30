using BirthdayGifts.Services.DTOs.Employee;

namespace BirthdayGifts.Services.Interfaces.Employee
{
    public interface IEmployeeService
    {
        Task<EmployeeDto?> GetEmployeeByIdAsync(int employeeId);
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<IEnumerable<EmployeeDto>> GetFilteredEmployeesAsync(EmployeeFilterDto filter);
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto employeeDto);
        Task<EmployeeDto> UpdateEmployeeAsync(int employeeId, EmployeeUpdateDto employeeDto);
        Task<bool> DeleteEmployeeAsync(int employeeId);
        Task<IEnumerable<EmployeeDto>> GetEmployeesWithUpcomingBirthdaysAsync(int daysAhead);
    }
} 