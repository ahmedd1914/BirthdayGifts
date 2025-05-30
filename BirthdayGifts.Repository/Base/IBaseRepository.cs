using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayGifts.Repository.Base
{
    public interface IBaseRepository<T, F, U> where T : class
    {
        Task<int> CreateAsync(T entity);
        Task<T> RetrieveByIdAsync(int id);
        Task<IEnumerable<T>> RetrieveAllAsync();
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}
