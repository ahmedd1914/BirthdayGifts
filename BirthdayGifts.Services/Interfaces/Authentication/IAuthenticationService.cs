using BirthdayGifts.Services.DTOs.Authentication;

namespace BirthdayGifts.Services.Interfaces.Authentication
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);

    }
} 