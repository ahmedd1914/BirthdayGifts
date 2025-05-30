using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BirthdayGifts.Models;
using BirthdayGifts.Repository.Base;

namespace BirthdayGifts.Repository.Interfaces
{
    public interface IEmployeeRepository : IBaseRepository<Models.Employee, EmployeeFilter, EmployeeUpdate>
    {
        Task<IEnumerable<Models.Employee>> GetFilteredAsync(EmployeeFilter filter);
        Task<IEnumerable<Models.Employee>> GetEmployeesWithUpcomingBirthdaysAsync(int daysAhead);
    }
}
