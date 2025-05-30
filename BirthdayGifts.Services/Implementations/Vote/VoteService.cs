using BirthdayGifts.Models;
using BirthdayGifts.Repository.Interfaces;
using BirthdayGifts.Services.DTOs.Vote;
using BirthdayGifts.Services.Interfaces.Vote;
using BirthdayGifts.Services.Interfaces.VotingSession;
using AutoMapper;

namespace BirthdayGifts.Services.Implementations.Vote
{
    public class VoteService : IVoteService
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IVotingSessionService _votingSessionService;
        private readonly IMapper _mapper;

        public VoteService(
            IVoteRepository voteRepository,
            IVotingSessionService votingSessionService,
            IMapper mapper)
        {
            _voteRepository = voteRepository;
            _votingSessionService = votingSessionService;
            _mapper = mapper;
        }

        public async Task<VoteDto?> GetVoteByIdAsync(int voteId)
        {
            var vote = await _voteRepository.RetrieveByIdAsync(voteId);
            return _mapper.Map<VoteDto>(vote);
        }

        public async Task<IEnumerable<VoteDto>> GetAllVotesAsync()
        {
            var votes = await _voteRepository.RetrieveAllAsync();
            return _mapper.Map<IEnumerable<VoteDto>>(votes);
        }

        public async Task<IEnumerable<VoteDto>> GetFilteredVotesAsync(VoteFilterDto filter)
        {
            var filterModel = _mapper.Map<VoteFilter>(filter);
            var votes = await _voteRepository.GetFilteredAsync(filterModel);
            return _mapper.Map<IEnumerable<VoteDto>>(votes);
        }

        public async Task<VoteDto> CreateVoteAsync(CreateVoteDto voteDto)
        {
            // Validate voting session exists and is active
            var session = await _votingSessionService.GetSessionByIdAsync(voteDto.VoteSessionId);
            if (session == null)
                throw new InvalidOperationException("Voting session not found.");
            if (!session.IsActive)
                throw new InvalidOperationException("Voting session is not active.");

            // Prevent the birthday person from voting in their own session
            if (voteDto.VoterId == session.BirthdayPersonId)
                throw new InvalidOperationException("You cannot vote in your own birthday session.");

            // Check if user has already voted in this session
            var existingVotes = await GetVotesBySessionIdAsync(voteDto.VoteSessionId);
            if (existingVotes.Any(v => v.VoterId == voteDto.VoterId))
                throw new InvalidOperationException("You have already voted in this session.");

            var vote = _mapper.Map<Models.Vote>(voteDto);
            var id = await _voteRepository.CreateAsync(vote);
            vote.VoteId = id;
            return _mapper.Map<VoteDto>(vote);
        }

        public async Task<VoteDto> UpdateVoteAsync(int voteId, VoteUpdateDto voteDto)
        {
            var vote = await _voteRepository.RetrieveByIdAsync(voteId);
            if (vote == null)
                throw new KeyNotFoundException($"Vote with ID {voteId} not found");

            // Validate voting session is still active if changing session
            if (voteDto.VoteSessionId.HasValue && voteDto.VoteSessionId != vote.VoteSessionId)
            {
                var session = await _votingSessionService.GetSessionByIdAsync(voteDto.VoteSessionId.Value);
                if (session == null || !session.IsActive)
                    throw new InvalidOperationException("Cannot change vote to an inactive session.");
            }

            _mapper.Map(voteDto, vote);
            await _voteRepository.UpdateAsync(vote);
            return _mapper.Map<VoteDto>(vote);
        }

        public async Task<bool> DeleteVoteAsync(int voteId)
        {
            return await _voteRepository.DeleteAsync(voteId);
        }

        public async Task<IEnumerable<VoteDto>> GetVotesBySessionIdAsync(int sessionId)
        {
            var filter = new VoteFilterDto { VoteSessionId = sessionId };
            return await GetFilteredVotesAsync(filter);
        }

        public async Task<IEnumerable<VoteDto>> GetVotesByVoterIdAsync(int voterId)
        {
            var filter = new VoteFilterDto { VoterId = voterId };
            return await GetFilteredVotesAsync(filter);
        }

        public async Task<IEnumerable<VoteDto>> GetVotesByGiftIdAsync(int giftId)
        {
            var filter = new VoteFilterDto { GiftId = giftId };
            return await GetFilteredVotesAsync(filter);
        }
    }
}
