using BirthdayGifts.Repository.Interfaces;
using BirthdayGifts.Services.DTOs.Authentication;
using BirthdayGifts.Services.Helpers;
using BirthdayGifts.Services.Interfaces.Authentication;
using AutoMapper;

namespace BirthdayGifts.Services.Implementations.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public AuthenticationService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
          
            
            var employees = await _employeeRepository.RetrieveAllAsync();
            var employee = employees.FirstOrDefault(e => e.Username == request.Username);

            if (employee == null)
            {
                
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }


            if (!SecurityHelper.VerifyPassword(request.Password, employee.PasswordHash))
            {
              
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

          
            return new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                Username = employee.Username,
                FullName = employee.FullName,
                EmployeeId = employee.EmployeeId
            };
        }
    }
}
