using BirthdayGifts.Services.DTOs.VotingSession;

namespace BirthdayGifts.Services.Interfaces.VotingSession
{
    public interface IVotingSessionService
    {
        Task<VotingSessionDto?> GetSessionByIdAsync(int sessionId);
        Task<IEnumerable<VotingSessionDto>> GetAllSessionsAsync();
        Task<IEnumerable<VotingSessionDto>> GetFilteredSessionsAsync(VotingSessionFilterDto filter);
        Task<VotingSessionDto> CreateSessionAsync(CreateVotingSessionDto sessionDto);
        Task<VotingSessionDto> UpdateSessionAsync(int sessionId, VotingSessionUpdateDto sessionDto);
        Task<bool> DeleteSessionAsync(int sessionId);
        Task<IEnumerable<VotingSessionDto>> GetActiveSessionsAsync();
        Task<IEnumerable<VotingSessionDto>> GetSessionsByEmployeeIdAsync(int employeeId);
    }
} 