using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using TrocaBaseGUI.Models;
using System.Net;
using TrocaBaseGUI.Services;
using System.ComponentModel;
using System.Data.SqlClient;

namespace TrocaBaseGUI.ViewModels
{
    public class MainViewModel
    {
        public ConexaoFileService conexaoFileService { get; set; }
        public string conexaoFile
        {
            get => conexaoFileService.ConexaoFile;
            set => conexaoFileService.ConexaoFile = value;
        }
        static string selectedBase;
        public static string exeFile;
        public string hostname;

        public SqlServerConnectionModel SQLServerConnection { get; set; } = new SqlServerConnectionModel();
        public OracleConnectionModel OracleConnection { get; set; } = new OracleConnectionModel();
        public OracleService OracleService;
        public SqlServerService SqlService;
        public ObservableCollection<DatabaseModel> Databases { get; set; } = new ObservableCollection<DatabaseModel>();
        private const int MaxHistory = 10;
        public ObservableCollection<SysDirectory> History { get; set; } = new ObservableCollection<SysDirectory>();
        public MainViewModel()
        {
            conexaoFileService = new ConexaoFileService();
            LoadState();

            SqlService = new SqlServerService(SQLServerConnection);
            openSqlConn(SqlService);

            OracleService = new OracleService();
            //openOracleConn(OracleService, "localhost", "oracle", "1521");
            openOracleConn(OracleService, OracleConnection.User, OracleConnection.Password, OracleConnection.Port);

            hostname = Dns.GetHostName();

            conexaoFileService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ConexaoFileService.ConexaoFile))
                {
                    OnPropertyChanged(nameof(conexaoFile));
                }
            };
        }

        public void openSqlConn(SqlServerService sqlservice)
        {
            if (String.IsNullOrEmpty(SQLServerConnection.Server))
            {
                Console.WriteLine("sem sql conx");
            }
            else
            {
                sqlservice.LoadSqlServerDatabases(SQLServerConnection.Server).ForEach(db => {
                    if (Databases.Any(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        return;
                    }
                    Databases.Add(db);
                });
                Console.WriteLine("\nserver: " + SQLServerConnection.Server + "\n");
            }
        }

        public void openOracleConn(OracleService oracleService, string hostname, string password, string port)
        {
            if (!OracleService.ValidateConnection(OracleConnection.GetConnectionString(hostname, password, port)))
            {
                Console.WriteLine("sem ora conx");
            }
            else
            {
                oracleService.GetDatabases(OracleConnection.GetConnectionString(hostname, password, port))
                .ForEach(db => {
                    if (Databases.Any(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        return;
                    }
                    Databases.Add(db);
                });
                Console.WriteLine("\noracle: " + OracleConnection.GetConnectionString(hostname, password, port) + "\n");
            }
            //if (String.IsNullOrEmpty(OracleConnection.GetConnectionString(hostname, password, port)))
            //{
            //    Console.WriteLine("sem ora conx");
            //}
            //else
            //{
            //    oracleService.GetDatabases(OracleConnection.GetConnectionString(hostname, password, port))
            //    .ForEach(db => {
            //        if (Databases.Any(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase)))
            //        {
            //            return;
            //        }
            //        Databases.Add(db);
            //        });
            //    Console.WriteLine("\noracle: " + OracleConnection.GetConnectionString(hostname, password, port) + "\n");
            //}

        }
        public void SelectBase(ObservableCollection<DatabaseModel> dbs, string db)
        {
            var conexaoService = conexaoFileService;
            string oracleDb = "[BANCODADOS]=ORACLE";
            string sqlServerDb = "[BANCODADOS]=SQLSERVER";
            var conexaoLines = File.ReadAllLines(conexaoFile).ToList();
            int bancoIndex = conexaoLines.FindIndex(line => line.IndexOf("[BANCODADOS]", StringComparison.OrdinalIgnoreCase) >= 0);
            int index;

            if (bancoIndex >= 0)
            {
                // Remove tudo a partir do [BANCODADOS]
                conexaoLines.RemoveRange(bancoIndex, conexaoLines.Count - bancoIndex);
                index = bancoIndex;
            }
            else
            {
                // Se não encontrou, adiciona duas linhas vazias e escreve no final
                if (!string.IsNullOrWhiteSpace(conexaoLines.Last()))
                {
                    conexaoLines.Add("");
                    conexaoLines.Add("");
                }
                index = conexaoLines.Count;
            }


            string newConn = dbs.Any(d => d.DbType.ToLower().StartsWith("s") && d.Name.Equals(db))
               ? conexaoService.CreateConnectionString(sqlServerDb, conexaoService.Domain, db)
               : conexaoService.CreateConnectionString(oracleDb, conexaoService.Domain, db);

            var newConnLines = newConn.Split('\n');

            // Adiciona a nova string
            conexaoLines.InsertRange(index, newConnLines);

            File.WriteAllLines(conexaoFile, conexaoLines);

            if(dbs.Any(b => b.IsSelected == true)) dbs.FirstOrDefault(b => b.IsSelected == true).IsSelected = false;

            dbs.FirstOrDefault(b => b.Name.Equals(db)).IsSelected = true;

            selectedBase = db;
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

            // Garante que só existam no máximo X itens no histórico
            while (History.Count > MaxHistory)
            {
                History.RemoveAt(History.Count - 1); // Remove o mais antigo (último)
            }
        }

        public void SaveState()
        {
            List<SysDirectory> historyList = History.ToList();
            string HistoricoSerialized = JsonSerializer.Serialize(historyList);
            if (HistoricoSerialized != null && !string.IsNullOrEmpty(HistoricoSerialized))
            {
                Properties.Settings.Default.HistoricoMem = HistoricoSerialized;
            }

            Properties.Settings.Default.ExeFileMem = exeFile;
            Properties.Settings.Default.ConexaoFileMem = conexaoFile;
            Properties.Settings.Default.Conexao = selectedBase;
            Properties.Settings.Default.SqlServerMem = SQLServerConnection.Server;
            Properties.Settings.Default.Save();
        }

        public void LoadState()
        {
            exeFile = Properties.Settings.Default.ExeFileMem;
            conexaoFile = Properties.Settings.Default.ConexaoFileMem;
            //conexaoFileService.SetConexaoAddress(Properties.Settings.Default.ConexaoFileMem);
            selectedBase = Properties.Settings.Default.Conexao;
            SQLServerConnection.Server = Properties.Settings.Default.SqlServerMem;

            string HistoricoSerialized = Properties.Settings.Default.HistoricoMem;
            if (HistoricoSerialized != null && !string.IsNullOrEmpty(HistoricoSerialized))
            {
                History =
                JsonSerializer.Deserialize<ObservableCollection<SysDirectory>>(HistoricoSerialized)
                ?? new ObservableCollection<SysDirectory>();
            }
        }

        public void ClearApp()
        {
            //conexaoFile = "";
            selectedBase = "";
            exeFile = "";
            SQLServerConnection.Server = "";
            Databases = new ObservableCollection<DatabaseModel>();

            History.Clear();
            Properties.Settings.Default.HistoricoMem = "";
            Properties.Settings.Default.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
