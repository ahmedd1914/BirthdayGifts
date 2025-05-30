using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BirthdayGifts.Models;
using BirthdayGifts.Repository.Base;
using BirthdayGifts.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BirthdayGifts.Repository.Implementations
{
    public class GiftRepository : BaseRepository<Gift>, IGiftRepository
    {
            private const string COLUMNS = "Name, Description, Price";
        private const string IdDbFieldEnumeratorName = "GiftId";
        
        public GiftRepository(string connectionString) : base(connectionString, "Gifts", "GiftId")
        {
        }

        protected override string GetColumns() => COLUMNS;

        protected override Gift MapToEntity(SqlDataReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            return new Gift
            {
                GiftId = Convert.ToInt32(reader[IdDbFieldEnumeratorName]),
                Name = Convert.ToString(reader["Name"]),
                Description = Convert.ToString(reader["Description"]),
                Price = Convert.ToDecimal(reader["Price"])
            };
        }

        public async Task<IEnumerable<Gift>> GetFilteredAsync(GiftFilter filter)
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                var conditions = new List<string>();
                var parameters = new List<SqlParameter>();

                if (filter.Name != null)
                {
                    conditions.Add("Name LIKE @Name");
                    parameters.Add(new SqlParameter("@Name", $"%{filter.Name}%"));
                }
                if (filter.MinPrice.HasValue)
                {
                    conditions.Add("Price >= @MinPrice");
                    parameters.Add(new SqlParameter("@MinPrice", filter.MinPrice.Value));
                }
                if (filter.MaxPrice.HasValue)
                {
                    conditions.Add("Price <= @MaxPrice");
                    parameters.Add(new SqlParameter("@MaxPrice", filter.MaxPrice.Value));
                }

                var whereClause = conditions.Any() ? $"WHERE {string.Join(" AND ", conditions)}" : "";
                command.CommandText = $"SELECT {GetColumns()}, {_idColumnName} FROM {_tableName} {whereClause}";

                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
            }

                using var reader = await command.ExecuteReaderAsync();
                var gifts = new List<Gift>();
                while (await reader.ReadAsync())
                {
                    gifts.Add(MapToEntity(reader));
                }
                return gifts;
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
