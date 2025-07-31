using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace TrocaBaseGUI.Services
{
    public class SqlServerService
    {
        private readonly SqlServerConnectionModel _connection;

        public SqlServerService(SqlServerConnectionModel connection)
        {
            _connection = connection;
        }

        public async Task<List<DatabaseModel>> LoadSqlServerDatabases(string server)
        {
            var databases = new List<DatabaseModel>();
            using (var conn = new SqlConnection(_connection.GetConnectionString(server)))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT name FROM sys.databases WHERE database_id > 4", conn);
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    databases.Add(new DatabaseModel { Name = reader.GetString(0), DbType = "SQLServer", Instance = "local" });
                }
            }
            return databases;
        }

        public async Task<Boolean> ValidateConnection(string server, int timeoutSeconds = 2)
        {
            try
            {
                var connectionString = _connection.GetConnectionString(server);

                if (!connectionString.ToLower().Contains("connect timeout"))
                    connectionString += ";Connect Timeout=" + timeoutSeconds;

                using (var conn = new SqlConnection(connectionString))
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
                    await conn.OpenAsync(cts.Token);
                    return conn.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
