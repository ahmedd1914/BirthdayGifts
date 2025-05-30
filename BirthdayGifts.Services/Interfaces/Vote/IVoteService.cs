using BirthdayGifts.Services.DTOs.Vote;

namespace BirthdayGifts.Services.Interfaces.Vote
{
    public interface IVoteService
    {
        Task<VoteDto?> GetVoteByIdAsync(int voteId);
        Task<IEnumerable<VoteDto>> GetAllVotesAsync();
        Task<IEnumerable<VoteDto>> GetFilteredVotesAsync(VoteFilterDto filter);
        Task<VoteDto> CreateVoteAsync(CreateVoteDto voteDto);
        Task<VoteDto> UpdateVoteAsync(int voteId, VoteUpdateDto voteDto);
        Task<bool> DeleteVoteAsync(int voteId);
        Task<IEnumerable<VoteDto>> GetVotesBySessionIdAsync(int sessionId);
        Task<IEnumerable<VoteDto>> GetVotesByVoterIdAsync(int voterId);
        Task<IEnumerable<VoteDto>> GetVotesByGiftIdAsync(int giftId);
    }
} 