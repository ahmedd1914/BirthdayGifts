using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BirthdayGifts.Models;
using BirthdayGifts.Repository.Base;


namespace BirthdayGifts.Repository.Interfaces
{
    public interface IGiftRepository : IBaseRepository<Gift, GiftFilter, GiftUpdate>
    {
        Task<IEnumerable<Gift>> GetFilteredAsync(GiftFilter filter);
    }
}
