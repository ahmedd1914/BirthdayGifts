using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BirthdayGifts.Models;
using BirthdayGifts.Repository.Base;


namespace BirthdayGifts.Repository.Interfaces
{
    public interface IVotingSessionRepository : IBaseRepository<VotingSession, VotingSessionFilter, VotingSessionUpdate>
    {
        Task<IEnumerable<VotingSession>> GetFilteredAsync(VotingSessionFilter filter);
    }
}
