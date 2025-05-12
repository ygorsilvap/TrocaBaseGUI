using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using TrocaBaseGUI.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Net;
using Oracle.ManagedDataAccess.Client;
using System.ServiceProcess;

namespace TrocaBaseGUI.ViewModels
{
    internal class MainViewModel
    {
        public static string ConexaoFile;
        static string selectedBase;
        public static string exeFile;
        public string hostname;

        public SqlServerConnectionModel SQLServerConnection { get; set; } = new SqlServerConnectionModel();

        public ObservableCollection<DatabaseModel> Databases { get; set; } = new ObservableCollection<DatabaseModel>();

        private const int MaxHistory = 10;
        public ObservableCollection<SysDirectory> History { get; set; } = new ObservableCollection<SysDirectory>();
        public MainViewModel()
        {
            //LoadState();
            LoadSqlServerDatabases();
            GetOracleInstances();
            GetOracleInstancesDatabases();  
            hostname = Dns.GetHostName();
            //Console.WriteLine(hostname);

            //SelectBase(Databases);
        }

        
        public void LoadSqlServerDatabases()
        {
            using (var conn = new SqlConnection(SQLServerConnection.GetConnectionString()))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT name FROM sys.databases WHERE database_id > 4", conn);
                var reader = cmd.ExecuteReader();

                Databases.Clear();
                while (reader.Read())
                {
                    Databases.Add(new DatabaseModel { Name = reader.GetString(0), DbType = "SQLServer", Instance = "local"});
                }
            }
        }

        public List<string> GetOracleInstances()
        {
            List<string> inst = new List<string>();

            // Lista todos os serviços do sistema
            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController service in services)
            {
                // Verifica se o nome do serviço contém "Oracle"
                if (service.ServiceName.Contains("OracleService"))
                {
                    if(service.Status.ToString().ToLower().Equals("running")) inst.Add(service.ServiceName.Remove(0, 13));
                }
            }
            return inst;
        }

        public void GetOracleInstancesDatabases()
        {
            string exception = "'SYS', 'SYSTEM', 'OUTLN', 'DBSNMP', 'APPQOSSYS', 'AUDSYS', 'CTXSYS', 'DBSFWUSER', 'GGSYS', 'GSMADMIN_INTERNAL', " +
                "'OJVMSYS', 'ORACLE_OCM', 'ORDDATA', 'ORDPLUGINS', 'ORDSYS', 'XDB', 'XS$NULL', 'MDSYS', 'WMSYS', 'LBACSYS', 'ANONYMOUS', 'SI_INFORMTN_SCHEMA', 'OLAPSYS', 'DVF', 'DVSYS'";

            string connectionString = "User Id=sys;Password=oracle;Data Source=MTZNOTFS058680:1521/LINX;DBA Privilege=SYSDBA;"; 
            //string connectionString = "User Id=sys;Password=oracle;Data Source=DESKTOP-N8OLEBQ:1521/LINX;DBA Privilege=SYSDBA;";

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Conexão realizada com sucesso!");

                    OracleCommand cmd = new OracleCommand("SELECT username FROM dba_users WHERE account_status = 'OPEN' AND default_tablespace NOT IN ('SYSTEM', 'SYSAUX') " + 
                        $"AND username NOT IN ({exception}) ORDER BY username", conn);
                    OracleDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Databases.Add(new DatabaseModel { Name = reader.GetString(0), DbType = "Oracle", Instance = "local" });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro: " + ex.Message);
                }
            }
        }

        public void SelectBase(ObservableCollection<DatabaseModel> dbs, string db)
        {
            if (dbs.Any(d => d.DbType.ToLower().StartsWith("s") && d.Name.Equals(db)))
            {
                Console.WriteLine("SQLServer");
            }
            else if (dbs.Any(d => d.DbType.ToLower().StartsWith("o") && d.Name.Equals(db)))
            {
                Console.WriteLine("Oracle");
            }
        }

        static Boolean IsThereConexaoDat()
        {
            if (selectedBase == "")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public static string ToCapitalize(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        public ObservableCollection<DatabaseModel> InstanceFilter(string instance, ObservableCollection<DatabaseModel> db)
        {
            return new ObservableCollection<DatabaseModel>(db.Where(i => i.Instance.Equals(instance)));
        }

        public ObservableCollection<DatabaseModel> DbTypeFilter(string type, ObservableCollection<DatabaseModel> db)
        {
            return new ObservableCollection<DatabaseModel>(db.Where(i => i.DbType.Equals(type, StringComparison.OrdinalIgnoreCase)));
        }

        public void AddDirectory(string endereco, string enderecoCompleto, string exe)
        {
            // Verifica se já existe o diretório no histórico
            var existente = History.FirstOrDefault(d => d.Address == endereco);
            if (existente != null)
            {
                History.Remove(existente); // Remove para mover para o topo
            }

            // Adiciona no início da lista
            History.Insert(0, new SysDirectory(endereco, enderecoCompleto, exe));
            exeFile = exe;

            // Garante que só existam no máximo 5
            while (History.Count > MaxHistory)
            {
                History.RemoveAt(History.Count - 1); // Remove o mais antigo (último)
            }
        }

        //public void SetBaseAddress(string add)
        //{
        //    DbDirectory = add;
        //}

        public void SetConexaoAddress(string add)
        {
            ConexaoFile = add + @"\conexao.dat";
        }

        public Boolean ValidateSystemPath(string path)
        {
            return File.Exists(path + "\\conexao.dat") ? true : false;
        }

        public void SaveState()
        {
            List<SysDirectory> historyList = History.ToList();
            string HistoricoSerialized = JsonSerializer.Serialize(historyList);
            if (HistoricoSerialized != null && !string.IsNullOrEmpty(HistoricoSerialized))
            {
                Properties.Settings.Default.historico = HistoricoSerialized;
            }

            Properties.Settings.Default.ExeFile = exeFile;
            Properties.Settings.Default.conexaoFile = ConexaoFile;
            Properties.Settings.Default.Conexao = selectedBase;
            Properties.Settings.Default.Save();
        }

        public void LoadState()
        {
            exeFile = Properties.Settings.Default.ExeFile;
            ConexaoFile = Properties.Settings.Default.conexaoFile;
            selectedBase = Properties.Settings.Default.Conexao;

            string HistoricoSerialized = Properties.Settings.Default.historico;
            if (HistoricoSerialized != null && !string.IsNullOrEmpty(HistoricoSerialized))
            {
                History =
                JsonSerializer.Deserialize<ObservableCollection<SysDirectory>>(HistoricoSerialized)
                ?? new ObservableCollection<SysDirectory>();
            }
        }

        public void ClearApp()
        {
            ConexaoFile = "";
            selectedBase = "";
            exeFile = "";

            History.Clear();
            Properties.Settings.Default.historico = "";
            Properties.Settings.Default.Save();
        }
    }
}
