using BirthdayGifts.Models;
using BirthdayGifts.Repository.Interfaces;
using BirthdayGifts.Services.DTOs.VotingSession;
using BirthdayGifts.Services.Interfaces.VotingSession;
using AutoMapper;

namespace BirthdayGifts.Services.Implementations.VotingSession
{
    public class VotingSessionService : IVotingSessionService
    {
        private readonly IVotingSessionRepository _sessionRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public VotingSessionService(
            IVotingSessionRepository sessionRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _sessionRepository = sessionRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<VotingSessionDto?> GetSessionByIdAsync(int sessionId)
        {
            var session = await _sessionRepository.RetrieveByIdAsync(sessionId);
            return _mapper.Map<VotingSessionDto>(session);
        }

        public async Task<IEnumerable<VotingSessionDto>> GetAllSessionsAsync()
        {
            var sessions = await _sessionRepository.RetrieveAllAsync();
            return _mapper.Map<IEnumerable<VotingSessionDto>>(sessions);
        }

        public async Task<VotingSessionDto> CreateSessionAsync(CreateVotingSessionDto sessionDto)
        {
            // Set automatic fields in the service layer
            sessionDto.IsActive = true;
            sessionDto.StartedAt = DateTime.Now;
            sessionDto.Year = DateTime.Now.Year;

            // Fetch the employee's date of birth
            var employee = await _employeeRepository.RetrieveByIdAsync(sessionDto.BirthdayPersonId);
            if (employee == null)
                throw new Exception("Birthday person not found.");

            var today = DateTime.Today;
            var nextBirthday = new DateTime(today.Year, employee.DateOfBirth.Month, employee.DateOfBirth.Day);
            if (nextBirthday < today)
                nextBirthday = nextBirthday.AddYears(1);
            sessionDto.EndedAt = nextBirthday;

            // Check if a session already exists for this birthday person in the given year
            var existingSessions = await _sessionRepository.RetrieveAllAsync();
            var existingSession = existingSessions.FirstOrDefault(s => 
                s.BirthdayPersonId == sessionDto.BirthdayPersonId && 
                s.Year == sessionDto.Year &&
                s.isActive);  // Only check active sessions

            if (existingSession != null)
            {
                throw new InvalidOperationException($"An active voting session already exists for employee ID {sessionDto.BirthdayPersonId} in year {sessionDto.Year}");
            }

            var session = _mapper.Map<Models.VotingSession>(sessionDto);
            
            try
            {
                var sessionId = await _sessionRepository.CreateAsync(session);
                if (sessionId <= 0)
                {
                    throw new Exception("Failed to create voting session - invalid session ID returned");
                }

                // Get the created session
                var createdSession = await _sessionRepository.RetrieveByIdAsync(sessionId);
                if (createdSession == null)
                {
                    throw new Exception("Voting session was created but could not be retrieved");
                }

                // Map to DTO and ensure ID is set
                var result = _mapper.Map<VotingSessionDto>(createdSession);
                result.VoteSessionId = sessionId;  // Ensure ID is set correctly
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create voting session: {ex.Message}", ex);
            }
        }

        public async Task<VotingSessionDto> UpdateSessionAsync(int sessionId, VotingSessionUpdateDto sessionDto)
        {
            try
            {
                var session = await _sessionRepository.RetrieveByIdAsync(sessionId);
                if (session == null)
                    throw new KeyNotFoundException($"Voting session with ID {sessionId} not found");

                // Only update fields that are set in the DTO
                if (sessionDto.IsActive.HasValue)
                    session.isActive = sessionDto.IsActive.Value;
                if (sessionDto.EndedAt.HasValue)
                    session.EndedAt = sessionDto.EndedAt.Value;
                if (sessionDto.VoteSessionCreatorId.HasValue)
                    session.VoteSessionCreatorId = sessionDto.VoteSessionCreatorId.Value;
                if (sessionDto.BirthdayPersonId.HasValue)
                    session.BirthdayPersonId = sessionDto.BirthdayPersonId.Value;
                if (sessionDto.Year.HasValue)
                    session.Year = sessionDto.Year.Value;
                // Do NOT update StartedAt unless explicitly set (to avoid DateTime.MinValue)
                if (sessionDto.StartedAt.HasValue && sessionDto.StartedAt.Value > new DateTime(1753,1,1))
                    session.StartedAt = sessionDto.StartedAt.Value;

                await _sessionRepository.UpdateAsync(session);
                return _mapper.Map<VotingSessionDto>(session);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update voting session: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteSessionAsync(int sessionId)
        {
            return await _sessionRepository.DeleteAsync(sessionId);
        }

        public async Task<IEnumerable<VotingSessionDto>> GetActiveSessionsAsync()
        {
            var sessions = await _sessionRepository.RetrieveAllAsync();
            var now = DateTime.Now;
            var activeSessions = sessions.Where(s => 
                s.isActive && 
                s.StartedAt <= now && 
                (s.EndedAt == null || s.EndedAt > now));
            return _mapper.Map<IEnumerable<VotingSessionDto>>(activeSessions);
        }

        public async Task<IEnumerable<VotingSessionDto>> GetSessionsByEmployeeIdAsync(int employeeId)
        {
            var sessions = await _sessionRepository.RetrieveAllAsync();
            var employeeSessions = sessions.Where(s => 
                s.VoteSessionCreatorId == employeeId || s.BirthdayPersonId == employeeId);
            return _mapper.Map<IEnumerable<VotingSessionDto>>(employeeSessions);
        }

        public async Task<IEnumerable<VotingSessionDto>> GetFilteredSessionsAsync(VotingSessionFilterDto filter)
        {
            var filterModel = _mapper.Map<VotingSessionFilter>(filter);
            var sessions = await _sessionRepository.GetFilteredAsync(filterModel);
            return _mapper.Map<IEnumerable<VotingSessionDto>>(sessions);
        }
    }
}
