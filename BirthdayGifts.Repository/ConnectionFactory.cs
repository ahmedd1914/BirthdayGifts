using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BirthdayGifts.Repository
{
    public static class ConnectionFactory
    {
        public static string ConnectionString;

        public static void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public static async Task<IDbConnection> CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
