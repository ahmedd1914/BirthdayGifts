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
    public class VoteRepository : BaseRepository<Vote>, IVoteRepository
    {
        private const string COLUMNS = "VoteSessionId, VoterId, GiftId";
        private const string IdDbFieldEnumeratorName = "VoteId";

        public VoteRepository(string connectionString) : base(connectionString, "Votes", "VoteId")
        {
        }

        protected override string GetColumns() => COLUMNS;

        protected override Vote MapToEntity(SqlDataReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            return new Vote
            {
                VoteId = Convert.ToInt32(reader[IdDbFieldEnumeratorName]),
                VoteSessionId = Convert.ToInt32(reader["VoteSessionId"]),
                VoterId = Convert.ToInt32(reader["VoterId"]),
                GiftId = Convert.ToInt32(reader["GiftId"])
            };
        }

        public async Task<IEnumerable<Vote>> GetFilteredAsync(VoteFilter filter)
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                var conditions = new List<string>();
                var parameters = new List<SqlParameter>();

                if (filter.VoteSessionId.HasValue)
                {
                    conditions.Add("VoteSessionId = @VoteSessionId");
                    parameters.Add(new SqlParameter("@VoteSessionId", filter.VoteSessionId.Value));
                }
                if (filter.VoterId.HasValue)
                {
                    conditions.Add("VoterId = @VoterId");
                    parameters.Add(new SqlParameter("@VoterId", filter.VoterId.Value));
                }
                if (filter.GiftId.HasValue)
                {
                    conditions.Add("GiftId = @GiftId");
                    parameters.Add(new SqlParameter("@GiftId", filter.GiftId.Value));
                }

                var whereClause = conditions.Any() ? $"WHERE {string.Join(" AND ", conditions)}" : "";
                command.CommandText = $"SELECT {GetColumns()}, {_idColumnName} FROM {_tableName} {whereClause}";

                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }

                using var reader = await command.ExecuteReaderAsync();
                var votes = new List<Vote>();
                while (await reader.ReadAsync())
                {
                    votes.Add(MapToEntity(reader));
                }
                return votes;
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
