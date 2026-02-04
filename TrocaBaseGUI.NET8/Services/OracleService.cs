using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Oracle.ManagedDataAccess.Client;
using TrocaBaseGUI.Properties.Constants;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace TrocaBaseGUI.Services
{
    public class OracleService
    {
        private readonly OracleConnectionModel _connection;

        public OracleService(OracleConnectionModel connection)
        {
            _connection = connection;
        }
        public List<string> GetRunningInstances()
        {
            return ServiceController.GetServices()
                .Where(s => s.ServiceName.Contains("OracleService") && s.Status == ServiceControllerStatus.Running)
                .Select(s => s.ServiceName.Remove(0, 13))
                .ToList();
        }

        //Colocar a função ValidateConnection dentro dessa função
        public async Task<List<DatabaseModel>> GetOracleDatabases(OracleConnectionModel oracleConnection, string instance)
        {
            string exception = "'SYS', 'SYSTEM', 'OUTLN', 'DBSNMP', 'APPQOSSYS', 'AUDSYS', 'CTXSYS', 'DBSFWUSER', 'GGSYS', 'GSMADMIN_INTERNAL', " +
                "'OJVMSYS', 'ORACLE_OCM', 'ORDDATA', 'ORDPLUGINS', 'ORDSYS', 'XDB', 'XS$NULL', 'MDSYS', 'WMSYS', 'LBACSYS', 'ANONYMOUS', 'SI_INFORMTN_SCHEMA', 'OLAPSYS', 'DVF', 'DVSYS'";

            List<DatabaseModel> databases = new List<DatabaseModel>();

            //Debug.WriteLine($"\n\n{oracleConnection.Server}, {oracleConnection.Password}, {oracleConnection.Port}, {oracleConnection.Instance}, {oracleConnection.Environment}\n\n");

            string connectionString = _connection.GetConnectionString(oracleConnection, instance);

            if (string.IsNullOrWhiteSpace(connectionString))
                return databases;

            using (var conn = new OracleConnection(connectionString))
            {
                await conn.OpenAsync();

                //Eram 2 querys separadas e foi utilizada IA para unifica-las
                var cmd = new OracleCommand(@"
                    SELECT 
                        u.username,
                        SYS_CONTEXT('USERENV', 'INSTANCE_NAME') AS instance,
                        MIN(o.created) AS data_hora_importacao
                    FROM dba_users u
                    LEFT JOIN dba_objects o 
                        ON o.owner = u.username
                    WHERE 
                        u.account_status = 'OPEN'
                        AND u.default_tablespace NOT IN ('SYSTEM', 'SYSAUX')
                        AND u.username NOT IN (" + exception + @")
                    GROUP BY 
                        u.username
                    HAVING 
                        COUNT(o.object_name) > 0
                    ORDER BY 
                        u.username
                    ", conn);
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    string username = reader.GetString(0);
                    //string importDate = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    string importDate = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    //Debug.WriteLine($"\n\ndata: {importDate}\n\n");

                    if (connectionString.Contains("DBA"))
                    {
                        databases.Add(new DatabaseModel 
                        { 
                            Name = username, 
                            DbType = "Oracle", 
                            Environment = "local", 
                            Instance = instance, 
                            Server = oracleConnection.Server, 
                            ImportDate = importDate
                        });
                    }
                    else
                    {
                        databases.Add(new DatabaseModel
                        {
                            Name = username,
                            DbType = "Oracle",
                            Environment = "server",
                            Instance = instance,
                            Server = oracleConnection.Server, 
                            ImportDate = importDate });
                    }
                }
            }
            return databases;
        }
 
        public async Task<bool> ValidateConnection(OracleConnectionModel oracleConnection, string instance, bool showResult = true, double timeoutSeconds = 5000)
        {
            string connectionString = _connection.GetConnectionString(oracleConnection, instance);
            using var conn = new OracleConnection(connectionString);

            var openTask = conn.OpenAsync();

            if (await Task.WhenAny(openTask, Task.Delay(TimeSpan.FromSeconds(timeoutSeconds))) == openTask)
            {
                try
                {
                    await openTask;
                    await conn.CloseAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();

                    if (showResult)
                    {
                        if (oracleConnection.Environment.Equals("server", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show($"{ex.GetType().Name} - {ex.Message}", "Falha na conexão do servidor com Oracle", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                        else
                        {
                            MessageBox.Show($"{ex.GetType().Name} - {ex.Message}", "Falha na conexão local com Oracle", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public string CreateOracleConnectionString(string environment, string server, string instance, string db)
        {
            //CRIAR A DIFERENÇA DE CONN STRING DE LOCAL PARA SERVER. TROCAR O SERVER DO LOCAL PARA DNS.GETHOSTNAME
            if (environment.ToLower() == "local")
            {
                return $"{GlobalStrings.BancoDadosTag}=ORACLE\n{GlobalStrings.DatabaseTag}={server}/{instance}\n{GlobalStrings.UsuarioOracleTag}={db.ToUpper()}";
            }
            else
            {
                return $"{GlobalStrings.BancoDadosTag}=ORACLE\n{GlobalStrings.DatabaseTag}={server}/{instance}\n{GlobalStrings.UsuarioOracleTag}={db.ToUpper()}";
            }
        }
    }
}
    