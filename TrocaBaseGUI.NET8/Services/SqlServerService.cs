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
using TrocaBaseGUI.Properties.Constants;

namespace TrocaBaseGUI.Services
{
    public class SqlServerService
    {
        private readonly SqlServerConnectionModel _connection;

        public SqlServerService(SqlServerConnectionModel connection)
        {
            _connection = connection;
        }

        public string GetConnectionString(SqlServerConnectionModel sqlServerConnection)
        {
            if (String.IsNullOrEmpty(sqlServerConnection.Password))
            {
                return $"Server={sqlServerConnection.Server};Integrated Security=True;TrustServerCertificate=True;";
            }
            else
            {
                return $"Server={sqlServerConnection.Server};User Id={sqlServerConnection.Username};Password={sqlServerConnection.Password};TrustServerCertificate=True;";
            }
        }

        public async Task<List<DatabaseModel>> GetSqlServerDatabases(SqlServerConnectionModel sqlServerConnection)
        {
                List<DatabaseModel> databases = new List<DatabaseModel>();
                using (var conn = new SqlConnection(GetConnectionString(sqlServerConnection)))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(@"
                            CREATE TABLE #Bancos (DatabaseName NVARCHAR(128), CreateDate DATETIME);

                            DECLARE @sql NVARCHAR(MAX) = N'';

                            SELECT @sql = @sql + '
                            IF EXISTS (SELECT 1 FROM [' + name + '].sys.tables WHERE name = ''fat_movimento_capa'')
                            BEGIN
                                INSERT INTO #Bancos (DatabaseName, CreateDate)
                                SELECT ''' + name + ''', create_date
                                FROM sys.databases
                                WHERE name = ''' + name + ''';
                            END
                            '
                            FROM sys.databases
                            WHERE database_id > 4  -- ignora bancos de sistema
                                    AND HAS_DBACCESS(name) = 1;  -- busca por bancos que são acessíveis com usuário CNP

                            EXEC sp_executesql @sql;

                            -- Retorna todos os bancos com data de criação
                            SELECT DatabaseName, CreateDate
                            FROM #Bancos
                            ORDER BY DatabaseName;

                            DROP TABLE #Bancos;
                        ", conn);
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                    string date = reader.GetDateTime(1).ToString("dd/MM/yy");//reader.GetDateTime(1).ToString("dd/MM/yyyy");

                        //Debug.WriteLine($"\n\nsqls date: {date}\n\n");

                        if (sqlServerConnection.Environment.Equals("local", StringComparison.OrdinalIgnoreCase))
                        {
                            databases.Add(new DatabaseModel { Name = reader.GetString(0), DbType = "SQL Server", Environment = "local", Server = sqlServerConnection.Server, ImportDate = date });
                        }
                        else
                        {
                            databases.Add(new DatabaseModel { Name = reader.GetString(0), DbType = "SQL Server", Environment = "server", Server = sqlServerConnection.Server, ImportDate = date });
                            //Debug.WriteLine($"\nDatabase: {reader.GetString(0)}, Type: {"SQL Server"}, Environment: {"server"}, Server: {sqlServerConnection.Server}, Date: {date}\n");

                        }
                    }
                }
                return databases;
        }


        //Refatorar
        public async Task<Boolean> ValidateConnection(SqlServerConnectionModel sqlServerConnection, double timeoutSeconds = 5000)
        {
            if (!sqlServerConnection.IsValid())
                return false;

            try
            {
                var connectionString = GetConnectionString(sqlServerConnection);

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
                if (sqlServerConnection.Environment.Equals("server", StringComparison.OrdinalIgnoreCase))
                {
                    //MessageBox.Show($"{ex.GetType().Name} - {ex.Message}", "Falha na conexão do servidor com SQL Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                else
                {
                    //MessageBox.Show($"{ex.GetType().Name} - {ex.Message}", "Falha na conexão local com SQL Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
        }

        public string CreateSQLServerConnectionString(string environment, string db, string server = null)
        {
            if(environment.ToLower() == "local" && 
                (server.ToLower().Contains(Dns.GetHostEntry(string.Empty).HostName) || Dns.GetHostEntry(string.Empty).HostName.ToLower().Contains(server.ToLower())))
            {
                return $"{GlobalStrings.BancoDadosTag}=SQLSERVER\n{GlobalStrings.DatabaseTag}={Dns.GetHostEntry(string.Empty).HostName}:{db.ToUpper()}";
            }
            else
            {
                return $"{GlobalStrings.BancoDadosTag}=SQLSERVER\n{GlobalStrings.DatabaseTag}={server}:{db.ToUpper()}";
            }
        }
    }
}
