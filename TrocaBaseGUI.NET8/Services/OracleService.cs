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

        public async Task<List<DatabaseModel>> GetDatabases(string server, string password, string port, string instance, string environment)
        {
            string exception = "'SYS', 'SYSTEM', 'OUTLN', 'DBSNMP', 'APPQOSSYS', 'AUDSYS', 'CTXSYS', 'DBSFWUSER', 'GGSYS', 'GSMADMIN_INTERNAL', " +
                "'OJVMSYS', 'ORACLE_OCM', 'ORDDATA', 'ORDPLUGINS', 'ORDSYS', 'XDB', 'XS$NULL', 'MDSYS', 'WMSYS', 'LBACSYS', 'ANONYMOUS', 'SI_INFORMTN_SCHEMA', 'OLAPSYS', 'DVF', 'DVSYS'";

            List<DatabaseModel> databases = new List<DatabaseModel>();

            string connectionString = _connection.GetConnectionString(server, password, port, instance, environment);

            if (string.IsNullOrWhiteSpace(connectionString))
                return databases;

            using (var conn = new OracleConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new OracleCommand(@"
                    SELECT username, SYS_CONTEXT('USERENV', 'INSTANCE_NAME') as instance
                    FROM dba_users
                    WHERE account_status = 'OPEN' 
                    AND default_tablespace NOT IN ('SYSTEM', 'SYSAUX') 
                    AND username NOT IN (" + exception + @")
                    ORDER BY username", conn); 
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    if (connectionString.Contains("DBA"))
                    {
                        databases.Add(new DatabaseModel { Name = reader.GetString(0), DbType = "Oracle", Environment = "local", Instance = instance, Server = server });
                    }
                    else
                    {
                        databases.Add(new DatabaseModel { Name = reader.GetString(0), DbType = "Oracle", Environment = "server", Instance = instance, Server = server });
                    }
                }
            }
            return databases;
        }

        public async Task<bool> ValidateConnection(string server, string password, string port, string instance, string environment, double timeoutSeconds = 3000)
        {
            string connectionString = _connection.GetConnectionString(server, password, port, instance, environment);
            using var conn = new OracleConnection(connectionString);

            Debug.WriteLine($"[Oracle] Conectando com: {connectionString}");

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
                    {
                        conn.Close();
                    }
                    if (connectionString.Contains("DBA"))
                    {
                        Debug.WriteLine($"[Validate-Oracle-Local] Falha: {ex.GetType().Name} - {ex.Message}");
                        return false;
                    }
                    else
                    {
                        Debug.WriteLine($"[Validate-Oracle-Server] Falha: {ex.GetType().Name} - {ex.Message}");

                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        //public async Task<bool> ValidateConnection(string connectionString, double timeoutSeconds = 300)
        //{
        //    using var conn = new OracleConnection(connectionString);

        //        var openTask = conn.OpenAsync();

        //    if (await Task.WhenAny(openTask, Task.Delay(TimeSpan.FromSeconds(timeoutSeconds))) == openTask)
        //    {
        //        try
        //        {
        //            await openTask;
        //            await conn.CloseAsync();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            if (conn.State == System.Data.ConnectionState.Open)
        //            {
        //                conn.Close();
        //            }
        //            if (connectionString.Contains("DBA"))
        //            {
        //                Debug.WriteLine($"[Validate-Oracle-Local] Falha: {ex.GetType().Name} - {ex.Message}");
        //                return false;
        //            }
        //            else
        //            {
        //                Debug.WriteLine($"[Validate-Oracle-Server] Falha: {ex.GetType().Name} - {ex.Message}");

        //                return false;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public string CreateOracleConnectionString(string environment, string server, string instance, string db)
        {
            //CRIAR A DIFERENÇA DE CONN STRING DE LOCAL PARA SERVER. TROCAR O SERVER DO LOCAL PARA DNS.GETHOSTNAME
            if (environment.ToLower() == "local")
            {
                return $"[BANCODADOS]=ORACLE\n[DATABASE]={Dns.GetHostEntry(string.Empty).HostName}/{instance}\n[USUARIO_ORACLE]={db.ToUpper()}";
            }
            else
            {
                return $"[BANCODADOS]=ORACLE\n[DATABASE]={server}/{instance}\n[USUARIO_ORACLE]={db.ToUpper()}";
            }
        }
    }
}
    