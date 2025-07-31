using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace TrocaBaseGUI.Services
{
    public class OracleService
    {
        public List<string> GetRunningInstances()
        {
            return ServiceController.GetServices()
                .Where(s => s.ServiceName.Contains("OracleService") && s.Status == ServiceControllerStatus.Running)
                .Select(s => s.ServiceName.Remove(0, 13))
                .ToList();
        }

        public List<DatabaseModel> GetDatabases(string connectionString)
        {
            string exception = "'SYS', 'SYSTEM', 'OUTLN', 'DBSNMP', 'APPQOSSYS', 'AUDSYS', 'CTXSYS', 'DBSFWUSER', 'GGSYS', 'GSMADMIN_INTERNAL', " +
                "'OJVMSYS', 'ORACLE_OCM', 'ORDDATA', 'ORDPLUGINS', 'ORDSYS', 'XDB', 'XS$NULL', 'MDSYS', 'WMSYS', 'LBACSYS', 'ANONYMOUS', 'SI_INFORMTN_SCHEMA', 'OLAPSYS', 'DVF', 'DVSYS'";


            var databases = new List<DatabaseModel>();
            using (var conn = new OracleConnection(connectionString))
            {
                conn.Open();
                var cmd = new OracleCommand("SELECT username FROM dba_users WHERE account_status = 'OPEN' AND default_tablespace NOT IN ('SYSTEM', 'SYSAUX') " + $"AND username NOT IN ({exception}) ORDER BY username", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    databases.Add(new DatabaseModel { Name = reader.GetString(0), DbType = "Oracle", Instance = "local" });
                }
            }
            return databases;
        }

        public async Task<Boolean> ValidateConnection(string connectionString, int timeoutSeconds = 2)
        {
            using (var conn = new OracleConnection(connectionString))
            {
                var openTask = Task.Run(() => {
                    try
                    {
                        conn.Open();
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            conn.Close();
                        }
                        return true;
                    }
                    catch
                    {
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            conn.Close();
                        }
                        return false;
                    }
                });

                if(await Task.WhenAny(openTask, Task.Delay(TimeSpan.FromSeconds(timeoutSeconds))) == openTask)
                {
                    return openTask.Result;
                } else
                {
                    return false;
                }
            }
        }
    }
}
