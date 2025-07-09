using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Services
{
    public class SqlServerService
    {
        private readonly SqlServerConnectionModel _connection;

        public SqlServerService(SqlServerConnectionModel connection)
        {
            _connection = connection;
        }

        public List<DatabaseModel> LoadSqlServerDatabases(string server)
        {
            var databases = new List<DatabaseModel>();
            using (var conn = new SqlConnection(_connection.GetConnectionString(server)))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT name FROM sys.databases WHERE database_id > 4", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    databases.Add(new DatabaseModel { Name = reader.GetString(0), DbType = "SQLServer", Instance = "local" });
                }
            }
            return databases;
        }
    }
}
