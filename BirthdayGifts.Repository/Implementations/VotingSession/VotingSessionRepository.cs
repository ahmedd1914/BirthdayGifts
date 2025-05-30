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
    public class VotingSessionRepository : BaseRepository<VotingSession>, IVotingSessionRepository
    {
        private const string COLUMNS = "VoteSessionCreatorId, BirthdayPersonId, isActive, StartedAt, EndedAt, Year";
        private const string IdDbFieldEnumeratorName = "VoteSessionId";

        public VotingSessionRepository(string connectionString) : base(connectionString, "VoteSessions", "VoteSessionId")
        {
        }

        protected override string GetColumns()
        {
            // Return columns in the same order as they appear in the database
            return "VoteSessionCreatorId, BirthdayPersonId, isActive, StartedAt, EndedAt, Year";
        }

        protected override VotingSession MapToEntity(SqlDataReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            return new VotingSession
            {
                VotingSessionId = Convert.ToInt32(reader[IdDbFieldEnumeratorName]),
                VoteSessionCreatorId = Convert.ToInt32(reader["VoteSessionCreatorId"]),
                BirthdayPersonId = Convert.ToInt32(reader["BirthdayPersonId"]),
                isActive = Convert.ToBoolean(reader["isActive"]),
                StartedAt = Convert.ToDateTime(reader["StartedAt"]),
                EndedAt = reader["EndedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["EndedAt"]) : null,
                Year = Convert.ToInt32(reader["Year"])
            };
        }

        public async Task<IEnumerable<VotingSession>> GetFilteredAsync(VotingSessionFilter filter)
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                var conditions = new List<string>();
                var parameters = new List<SqlParameter>();

                if (filter.VoteSessionCreatorId.HasValue)
                {
                    conditions.Add("VoteSessionCreatorId = @VoteSessionCreatorId");
                    parameters.Add(new SqlParameter("@VoteSessionCreatorId", filter.VoteSessionCreatorId.Value));
                }
                if (filter.BirthdayPersonId.HasValue)
                {
                    conditions.Add("BirthdayPersonId = @BirthdayPersonId");
                    parameters.Add(new SqlParameter("@BirthdayPersonId", filter.BirthdayPersonId.Value));
                }
                if (filter.IsActive.HasValue)
                {
                    conditions.Add("isActive = @IsActive");
                    parameters.Add(new SqlParameter("@IsActive", filter.IsActive.Value));
                }
                if (filter.Year.HasValue)
                {
                    conditions.Add("Year = @Year");
                    parameters.Add(new SqlParameter("@Year", filter.Year.Value));
                }

                var whereClause = conditions.Any() ? $"WHERE {string.Join(" AND ", conditions)}" : "";
                command.CommandText = $"SELECT {GetColumns()}, {_idColumnName} FROM {_tableName} {whereClause}";

                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }

                using var reader = await command.ExecuteReaderAsync();
                var sessions = new List<VotingSession>();
                while (await reader.ReadAsync())
                {
                    sessions.Add(MapToEntity(reader));
                }
                return sessions;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public override async Task<bool> UpdateAsync(VotingSession entity)
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = $@"UPDATE {_tableName} 
                    SET VoteSessionCreatorId = @VoteSessionCreatorId,
                        BirthdayPersonId = @BirthdayPersonId,
                        isActive = @isActive,
                        StartedAt = @StartedAt,
                        EndedAt = @EndedAt,
                        Year = @Year
                    WHERE {_idColumnName} = @{_idColumnName}";

                command.Parameters.AddWithValue("@VoteSessionCreatorId", entity.VoteSessionCreatorId);
                command.Parameters.AddWithValue("@BirthdayPersonId", entity.BirthdayPersonId);
                command.Parameters.AddWithValue("@isActive", entity.isActive);
                command.Parameters.AddWithValue("@StartedAt", entity.StartedAt);
                var endedAtValue = (object?)entity.EndedAt == null || (entity.EndedAt.HasValue && entity.EndedAt.Value == DateTime.MinValue)
                    ? DBNull.Value
                    : (object)entity.EndedAt.Value;
                command.Parameters.AddWithValue("@EndedAt", endedAtValue);
                command.Parameters.AddWithValue("@Year", entity.Year);
                command.Parameters.AddWithValue($"@{_idColumnName}", entity.VotingSessionId);

                // Debug logging for date values
                Console.WriteLine($"[DEBUG] VotingSessionId: {entity.VotingSessionId}, StartedAt: {entity.StartedAt}, EndedAt: {entity.EndedAt}");

                return await command.ExecuteNonQueryAsync() > 0;
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
