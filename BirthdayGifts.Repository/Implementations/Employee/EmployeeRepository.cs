using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BirthdayGifts.Models;
using BirthdayGifts.Repository.Base;
using BirthdayGifts.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using BirthdayGifts.Repository.Helpers;
using System.Data.SqlTypes;

namespace BirthdayGifts.Repository.Implementations
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private const string IdDbFieldEnumeratorName = "EmployeeId";
        private const string COLUMNS = "Username, PasswordHash, FullName, DateOfBirth";

        public EmployeeRepository(string connectionString) : base(connectionString, "Employees", "EmployeeId")
        {
        }

        protected override string GetColumns() => COLUMNS;

        public async Task<Employee> GetByUsernameAsync(string username)
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                var filter = new Filter("Username", username);
                command.CommandText = $"SELECT {GetColumns()}, {_idColumnName} FROM {_tableName} WHERE {filter.ToSql()}";
                command.Parameters.AddWithValue(filter.ParameterName, filter.Value);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return MapToEntity(reader);
                }
                return null;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesWithUpcomingBirthdaysAsync(int daysAhead)
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                var today = DateTime.Today;
                var endDate = today.AddDays(daysAhead);

                // Build the date filter condition
                var monthDayFilter = new StringBuilder();
                var parameters = new List<SqlParameter>();

                // Handle case where the date range crosses year boundary
                if (endDate.Year > today.Year)
                {
                    // First part: from today to end of year
                    monthDayFilter.Append("(MONTH(DateOfBirth) > @startMonth OR (MONTH(DateOfBirth) = @startMonth AND DAY(DateOfBirth) >= @startDay))");
                    parameters.Add(new SqlParameter("@startMonth", today.Month));
                    parameters.Add(new SqlParameter("@startDay", today.Day));

                    // Second part: from start of year to end date
                    monthDayFilter.Append(" OR (MONTH(DateOfBirth) < @endMonth OR (MONTH(DateOfBirth) = @endMonth AND DAY(DateOfBirth) <= @endDay))");
                    parameters.Add(new SqlParameter("@endMonth", endDate.Month));
                    parameters.Add(new SqlParameter("@endDay", endDate.Day));
                }
                else
                {
                    // Simple case: within same year
                    monthDayFilter.Append("(MONTH(DateOfBirth) > @startMonth OR (MONTH(DateOfBirth) = @startMonth AND DAY(DateOfBirth) >= @startDay))");
                    monthDayFilter.Append(" AND (MONTH(DateOfBirth) < @endMonth OR (MONTH(DateOfBirth) = @endMonth AND DAY(DateOfBirth) <= @endDay))");
                    parameters.Add(new SqlParameter("@startMonth", today.Month));
                    parameters.Add(new SqlParameter("@startDay", today.Day));
                    parameters.Add(new SqlParameter("@endMonth", endDate.Month));
                    parameters.Add(new SqlParameter("@endDay", endDate.Day));
                }

                command.CommandText = $@"SELECT {GetColumns()}, {_idColumnName} 
                                      FROM {_tableName} 
                                      WHERE {monthDayFilter}";

                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }

                using var reader = await command.ExecuteReaderAsync();
                var employees = new List<Employee>();
                while (await reader.ReadAsync())
                {
                    employees.Add(MapToEntity(reader));
                }
                return employees;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public async Task<bool> UpdatePasswordAsync(int employeeId, string hashedPassword)
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                using var update = new Update(connection, _tableName, "EmployeeId", employeeId);
                update.AddSetClause("PasswordHash", new SqlString(hashedPassword));
                return await update.ExecuteNonQueryAsync() > 0;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        protected override Employee MapToEntity(SqlDataReader reader)
        {
            return new Employee
            {
                EmployeeId = Convert.ToInt32(reader[IdDbFieldEnumeratorName]),
                Username = Convert.ToString(reader["Username"]),
                PasswordHash = Convert.ToString(reader["PasswordHash"]),
                FullName = Convert.ToString(reader["FullName"]),
                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"])
            };
        }

        public async Task<IEnumerable<Employee>> GetFilteredAsync(EmployeeFilter filter)
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                var conditions = new List<string>();
                var parameters = new List<SqlParameter>();

                if (filter.Username != null)
                {
                    conditions.Add("Username = @Username");
                    parameters.Add(new SqlParameter("@Username", filter.Username));
                }
                if (filter.FullName != null)
                {
                    conditions.Add("FullName = @FullName");
                    parameters.Add(new SqlParameter("@FullName", filter.FullName));
                }
                if (filter.DateOfBirth != null)
                {
                    conditions.Add("DateOfBirth = @DateOfBirth");
                    parameters.Add(new SqlParameter("@DateOfBirth", filter.DateOfBirth));
                }

                var whereClause = conditions.Any() ? $"WHERE {string.Join(" AND ", conditions)}" : "";
                command.CommandText = $"SELECT {GetColumns()}, {_idColumnName} FROM {_tableName} {whereClause}";

                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }

                using var reader = await command.ExecuteReaderAsync();
                var employees = new List<Employee>();
                while (await reader.ReadAsync())
                {
                    employees.Add(MapToEntity(reader));
                }
                return employees;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }
    }
}
