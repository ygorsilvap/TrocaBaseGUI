using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using System.Net;

namespace TrocaBaseGUI.Services
{
    public class SqlServerService
    {
        private readonly SqlServerConnectionModel _connection;

        public SqlServerService(SqlServerConnectionModel connection)
        {
            _connection = connection;
        }

        public async Task<List<DatabaseModel>> GetDatabases(SqlServerConnectionModel sqlServerConnection)
        {
            List<DatabaseModel> databases = new List<DatabaseModel>();
            using (var conn = new SqlConnection(_connection.GetConnectionString(sqlServerConnection)))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                            CREATE TABLE #Bancos (DatabaseName NVARCHAR(128));

                            DECLARE @sql NVARCHAR(MAX) = N'';

                            SELECT @sql = @sql + '
                            IF EXISTS (SELECT 1 FROM [' + name  + '].sys.tables WHERE name = ''fat_movimento_capa'')
                            BEGIN
                                INSERT INTO #Bancos(DatabaseName)
                                VALUES (''' + name + ''');
                            END
                            '
                            FROM sys.databases
                            WHERE database_id > 4;

                            EXEC sp_executesql @sql;

                            -- Retorna todos os bancos ordenados
                            SELECT DatabaseName FROM #Bancos ORDER BY DatabaseName;

                            DROP TABLE #Bancos;
                    ", conn);
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (String.IsNullOrWhiteSpace(sqlServerConnection.Password)) 
                    { 
                        databases.Add(new DatabaseModel { Name = reader.GetString(0), DbType = "SQLServer", Environment = "local", Server = sqlServerConnection.Server });
                    } else
                    {
                        databases.Add(new DatabaseModel { Name = reader.GetString(0), DbType = "SQLServer", Environment = "server", Server = sqlServerConnection.Server });
                    }
                }
            }
            return databases;
        }


        //Refatorar
        public async Task<Boolean> ValidateConnection(SqlServerConnectionModel sqlServerConnection, double timeoutSeconds = 3000)
        {
            if (sqlServerConnection.IsValid())
                return false;

            try
            {
                var connectionString = _connection.GetConnectionString(sqlServerConnection);

                if (!connectionString.ToLower().Contains("connect timeout"))
                    connectionString += ";Connect Timeout=" + timeoutSeconds;

                using (var conn = new SqlConnection(connectionString))
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeoutSeconds));
                    await conn.OpenAsync(cts.Token);
                    return conn.State == System.Data.ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {

                if (_connection.GetConnectionString(sqlServerConnection).Contains("Password"))
                {
                    Debug.WriteLine($"[ValidateConnection-SQLServer-Server] Falha: {ex.GetType().Name} - {ex.Message}");
                    return false;
                }
                else
                {
                    Debug.WriteLine($"[ValidateConnection-SQLServer-Local] Falha: {ex.GetType().Name} - {ex.Message}");
                    return false;
                }
            }
        }

        public string CreateSQLServerConnectionString(string environment, string db, string server = null)
        {
            //CRIAR A DIFERENÇA DE CONN STRING DE LOCAL PARA SERVER. TROCAR O SERVER DO LOCAL PARA DNS.GETHOSTNAME
            if(environment.ToLower() == "local" && 
                (server.ToLower().Contains(Dns.GetHostEntry(string.Empty).HostName) || Dns.GetHostEntry(string.Empty).HostName.ToLower().Contains(server.ToLower())))
            {
                return $"[BANCODADOS]=SQLSERVER\n[DATABASE]={Dns.GetHostEntry(string.Empty).HostName}:{db.ToUpper()}";
            }
            else
            {
                return $"[BANCODADOS]=SQLSERVER\n[DATABASE]={server}:{db.ToUpper()}";
            }
        }
    }
}
