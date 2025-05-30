using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using BirthdayGifts.Repository.Helpers;

namespace BirthdayGifts.Repository.Base
{
    public abstract class BaseRepository<T>
    {
        protected readonly string _connectionString;
        protected readonly string _tableName;
        protected readonly string _idColumnName;

        protected BaseRepository(string connectionString, string tableName, string idColumnName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _idColumnName = idColumnName;
            ConnectionFactory.SetConnectionString(connectionString);
        }

        protected async Task<SqlConnection> CreateConnectionAsync()
        {
            return (SqlConnection)await ConnectionFactory.CreateConnection();
        }

        protected virtual string GetColumns()
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != _idColumnName);
            return string.Join(", ", properties.Select(p => p.Name));
        }

        public virtual async Task<int> CreateAsync(T entity)
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                var columns = GetColumns();
                var columnNames = columns.Split(',').Select(c => c.Trim()).ToList();
                columnNames = columnNames.Where(c => !string.Equals(c, _idColumnName, StringComparison.OrdinalIgnoreCase)).ToList();
                var parameters = string.Join(", ", columnNames.Select(c => "@" + c));
                var command = connection.CreateCommand();
                command.CommandText = $@"INSERT INTO {_tableName} ({string.Join(", ", columnNames)}) 
                                       VALUES ({parameters});
                                       SELECT CAST(SCOPE_IDENTITY() as int)";

                foreach (var columnName in columnNames)
                {
                    var property = entity.GetType().GetProperties()
                        .FirstOrDefault(p => p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                    if (property != null)
                    {
                        var value = property.GetValue(entity) ?? DBNull.Value;
                        command.Parameters.AddWithValue("@" + columnName, value);
                    }   
                }

                var result = await command.ExecuteScalarAsync();
                if (result == null || result == DBNull.Value)
                {
                    throw new Exception("Failed to get the ID of the created record");
                }
                var id = Convert.ToInt32(result);
                return id;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public virtual async Task<T> RetrieveByIdAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT {GetColumns()}, {_idColumnName} FROM {_tableName} WHERE {_idColumnName} = @{_idColumnName}";
                command.Parameters.AddWithValue("@" + _idColumnName, id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return MapToEntity(reader);
                }
                return default(T);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public virtual async Task<IEnumerable<T>> RetrieveAllAsync()
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT {GetColumns()}, {_idColumnName} FROM {_tableName}";

                using var reader = await command.ExecuteReaderAsync();
                var entities = new List<T>();
                while (await reader.ReadAsync())
                {
                    entities.Add(MapToEntity(reader));
                }
                return entities;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                var properties = entity.GetType().GetProperties();
                var updates = string.Join(", ", properties.Where(p => p.Name != _idColumnName)
                    .Select(p => $"{p.Name} = @{p.Name}"));
                var command = connection.CreateCommand();
                command.CommandText = $"UPDATE {_tableName} SET {updates} WHERE {_idColumnName} = @{_idColumnName}";

                foreach (var property in properties)
                {
                    var value = property.GetValue(entity) ?? DBNull.Value;
                    command.Parameters.AddWithValue("@" + property.Name, value);
                }

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

        public virtual async Task<bool> DeleteAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            try
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = $"DELETE FROM {_tableName} WHERE {_idColumnName} = @{_idColumnName}";
                command.Parameters.AddWithValue("@" + _idColumnName, id);

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

        protected abstract T MapToEntity(SqlDataReader reader);
    }
}
