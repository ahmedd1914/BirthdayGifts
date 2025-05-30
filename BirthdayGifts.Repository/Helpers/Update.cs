using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace BirthdayGifts.Repository.Helpers
{
    public class Update : IDisposable
    {
            
    
        private List<string> setClauses = new List<string>();
        private SqlCommand sqlCommand;
        private readonly string idDbFieldName;
        private readonly int idDbFieldValue;

        public Update(
            SqlConnection sqlConnection,
            string tableName,
            string idDbFieldName, int idDbFieldValue,
            SqlTransaction? transaction = null)
        {
            sqlCommand = sqlConnection.CreateCommand();
            if (transaction != null)
            {
                sqlCommand.Transaction = transaction;
            }
            sqlCommand.CommandText = $"UPDATE [{tableName}]";

            this.idDbFieldName = idDbFieldName;
            this.idDbFieldValue = idDbFieldValue;
        }

        public void AddSetClause(string dbFieldName, INullable? dbFieldValue)
        {
            if (dbFieldValue is not null)
            {
                setClauses.Add($"[{dbFieldName}] = @{dbFieldName}");
                sqlCommand.Parameters.AddWithValue($"@{dbFieldName}", dbFieldValue);
            }
        }

        public async Task<int> ExecuteNonQueryAsync()
        {
            if (setClauses.Count == 0)
            {
                throw new Exception("No fields to update! You should pass at least one!");
            }

            sqlCommand.CommandText +=
@$"SET {string.Join(", ", setClauses)}
WHERE [{idDbFieldName}] = @{idDbFieldName}";

            sqlCommand.Parameters.AddWithValue($"@{idDbFieldName}", idDbFieldValue);

            Console.WriteLine($"Debug - SQL Query: {sqlCommand.CommandText}");
            foreach (SqlParameter param in sqlCommand.Parameters)
            {
                Console.WriteLine($"Debug - Parameter: {param.ParameterName} = {param.Value}");
            }

            // Execute the update command and return the number of affected rows
            int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

            if (rowsAffected != 1)
            {
                throw new Exception($"Just one row should be updated! Command aborted, because {rowsAffected} could have been updated!");
            }

            return rowsAffected;
        }

        public void Dispose()
        {
            sqlCommand.Dispose();
        }
    }
}
