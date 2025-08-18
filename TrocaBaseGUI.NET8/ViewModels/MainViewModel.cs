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
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using Microsoft.IdentityModel.Tokens;

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
        public static string exeFile;

        public SqlServerConnectionModel LocalSQLServerConnection { get; set; } = new SqlServerConnectionModel();
        public OracleConnectionModel LocalOracleConnection { get; set; } = new OracleConnectionModel();
        public SqlServerConnectionModel ServerSQLServerConnection { get; set; } = new SqlServerConnectionModel();
        
        public OracleConnectionModel ServerOracleConnection { get; set; } = new OracleConnectionModel();
        public OracleService OracleService;
        public SqlServerService SqlService;
        public ObservableCollection<DatabaseModel> Databases { get; set; } = new ObservableCollection<DatabaseModel>();
        //public ObservableCollection<DatabaseModel> serverDatabases { get; set; } = new ObservableCollection<DatabaseModel>();
        private const int MaxHistory = 10;
        public ObservableCollection<SysDirectory> History { get; set; } = new ObservableCollection<SysDirectory>();
        public MainViewModel()
        {
            conexaoFileService = new ConexaoFileService();
            LoadState();

            SqlService = new SqlServerService(LocalSQLServerConnection);

            OracleService = new OracleService(LocalOracleConnection);

            conexaoFileService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ConexaoFileService.ConexaoFile))
                {
                    OnPropertyChanged(nameof(conexaoFile));
                }
            };


        }
        public async Task openSqlConn(SqlServerService sqlservice, string server, string username = "CNP", string password = null)
        {
            if (await sqlservice.ValidateConnection(server, username, password))
            {
                var databases = await sqlservice.LoadSqlServerDatabases(server, username, password);
                databases.ForEach(db => {
                    if (Databases.Any(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase) && d.Environment.Equals(db.Environment, StringComparison.OrdinalIgnoreCase)))
                    {
                        return;
                    }
                        Databases.Add(db);
                });
                Console.WriteLine("\n[Conexão com SQL Server estabelecida]\n");
            }
            else
            {
                if (Databases.Count() > 0)
                {
                    var removable = Databases
                        .Where(item => item.DbType != null && item.DbType.ToLower().StartsWith("s"))
                        .ToList();

                    foreach (var item in removable)
                    {
                       Databases.Remove(item);
                    }
                }
                Console.WriteLine("\n[Conexão com SQL Server inválida]\n");
            }
        }


        //Refatorar
        public async Task openOracleConn(OracleService oracleService, string server, string password, string port, string serverInstance = null)
        {
            if (string.IsNullOrEmpty(serverInstance))
            {
                //MessageBox.Show("\n\nTESTE\n\n");
                foreach (string instance in oracleService.GetRunningInstances())
                {
                    if (await OracleService.ValidateConnection(LocalOracleConnection.GetLocalConnectionString(server, password, port, instance)))
                    {
                        List<DatabaseModel> dbs = await oracleService.GetDatabases(server, password, port, instance);
                        dbs.ForEach(db =>
                        {
                            if (Databases.Any(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase) && d.Environment.Equals(db.Environment, StringComparison.OrdinalIgnoreCase)))
                            {
                                return;
                            }
                            Databases.Add(db);
                        });
                        Console.WriteLine("\n[Conexão com Oracle estabelecida]\n");
                    }
                    else
                    {
                        if (Databases.Count() > 0)
                        {
                            var removable = Databases
                                .Where(item => item.DbType != null && item.DbType.ToLower().StartsWith("o"))
                                .ToList();

                            foreach (var item in removable)
                            {
                                Databases.Remove(item);
                            }
                        }
                        Console.WriteLine("\n[Conexão com Oracle inválida]\n");
                    }
                }
            } else
            {
                if (await OracleService.ValidateConnection(ServerOracleConnection.GetServerConnectionString(server, password, port, serverInstance)))
                {
                    //serverInstance redundante a ser removido
                    List<DatabaseModel> dbs = await oracleService.GetDatabases(server, password, port, serverInstance, serverInstance);
                    dbs.ForEach(db =>
                    {
                        if (Databases.Any(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase) && d.Environment.Equals(db.Environment, StringComparison.OrdinalIgnoreCase)))
                        {
                            return;
                        }
                        Databases.Add(db);
                    });
                    Console.WriteLine("\n[Conexão com Oracle estabelecida]\n");
                }
                else
                {
                    if (Databases.Count() > 0)
                    {
                        var removable = Databases
                            .Where(item => item.DbType != null && item.DbType.ToLower().StartsWith("o"))
                            .ToList();

                        foreach (var item in removable)
                        {
                            Databases.Remove(item);
                        }
                    }
                    Console.WriteLine("\n[Conexão com Oracle inválida]\n");
                }
            }

        }
        public void SelectBase(ObservableCollection<DatabaseModel> dbs, string db)
        {
            var conexaoService = conexaoFileService;
            List<String> conexaoLines = File.ReadAllLines(conexaoFile).ToList();
            int bancoIndex = conexaoLines.FindIndex(line => line.IndexOf("[BANCODADOS]", StringComparison.OrdinalIgnoreCase) >= 0);
            int index = 0;

            if(conexaoLines.Count > 0)
            {
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
            }



            string newConn = dbs.Any(d => d.DbType.ToLower().StartsWith("s") && d.Name.Equals(db))
               ? SqlService.CreateSQLServerConnectionString(dbs.FirstOrDefault(d => d.Name.Equals(db)).Environment, db, dbs.FirstOrDefault(d => d.Name.Equals(db)).Server)
               : OracleService.CreateOracleConnectionString(dbs.FirstOrDefault(d => d.Name.Equals(db)).Environment, db, dbs.First(d => d.Name.Equals(db)).Instance, db);

            var newConnLines = newConn.Split('\n');

            // Adiciona a nova string
            conexaoLines.InsertRange(index, newConnLines);

            File.WriteAllLines(conexaoFile, conexaoLines);

            DatabaseModel.SetSelection(dbs, db);

            //Refatorar
            string cnxPath = conexaoFileService.ConexaoAddress;
            string selectedCnx = History.FirstOrDefault(d => d.FullPathAddress.Equals(cnxPath)).FullPathAddress;

            if (cnxPath.Equals(selectedCnx))
                History.FirstOrDefault(d => d.FullPathAddress.Equals(cnxPath)).SelectedBase = db;
        }

        public ObservableCollection<DatabaseModel> EnvironmentFilter(string environment, ObservableCollection<DatabaseModel> db)
        {
            return new ObservableCollection<DatabaseModel>(db.Where(i => i.Environment.Equals(environment)));
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

            List<DatabaseModel> dbList = Databases.ToList();
            string DatabasesSerialized = JsonSerializer.Serialize(dbList);
            if (DatabasesSerialized != null && !string.IsNullOrEmpty(DatabasesSerialized))
            {
                Properties.Settings.Default.DatabasesMem = DatabasesSerialized;
            }

            Properties.Settings.Default.ExeFileMem = exeFile;
            Properties.Settings.Default.ConexaoFileMem = conexaoFile;

            Properties.Settings.Default.LocalSqlServerMem = LocalSQLServerConnection.Server;
            Properties.Settings.Default.ServerSqlServerMem = ServerSQLServerConnection.Server;
            Properties.Settings.Default.ServerSqlServerPasswordMem = ServerSQLServerConnection.Password;
            Properties.Settings.Default.ServerSqlServerUsernameMem = ServerSQLServerConnection.Username;

            Properties.Settings.Default.LocalOraServerMem = LocalOracleConnection.Server;
            Properties.Settings.Default.LocalOraPasswordMem = LocalOracleConnection.Password;
            Properties.Settings.Default.LocalOraPortMem = LocalOracleConnection.Port;

            Properties.Settings.Default.ServerOraServerMem = ServerOracleConnection.Server;
            Properties.Settings.Default.ServerOraPasswordMem = ServerOracleConnection.Password;
            Properties.Settings.Default.ServerOraPortMem = ServerOracleConnection.Port;
            Properties.Settings.Default.ServerOraInstanceMem = ServerOracleConnection.Instance;

            Properties.Settings.Default.Save();
        }

        public void LoadState()
        {
            exeFile = Properties.Settings.Default.ExeFileMem;
            conexaoFile = Properties.Settings.Default.ConexaoFileMem;
            //conexaoFileService.SetConexaoAddress(Properties.Settings.Default.ConexaoFileMem);
            LocalSQLServerConnection.Server = Properties.Settings.Default.LocalSqlServerMem;
            ServerSQLServerConnection.Server = Properties.Settings.Default.ServerSqlServerMem;
            ServerSQLServerConnection.Password = Properties.Settings.Default.ServerSqlServerPasswordMem;
            ServerSQLServerConnection.Username = string.IsNullOrEmpty(Properties.Settings.Default.ServerSqlServerUsernameMem) ? "CNP" : Properties.Settings.Default.ServerSqlServerUsernameMem;

            LocalOracleConnection.Server = Properties.Settings.Default.LocalOraServerMem;
            LocalOracleConnection.Password = Properties.Settings.Default.LocalOraPasswordMem;
            LocalOracleConnection.Port = string.IsNullOrEmpty(LocalOracleConnection.Port) ? Properties.Settings.Default.LocalOraPortMem : "1521";

            ServerOracleConnection.Server = Properties.Settings.Default.ServerOraServerMem;
            ServerOracleConnection.Password = Properties.Settings.Default.ServerOraPasswordMem;
            ServerOracleConnection.Port = string.IsNullOrEmpty(ServerOracleConnection.Port) ? Properties.Settings.Default.ServerOraPortMem : "1521";
            ServerOracleConnection.Instance = Properties.Settings.Default.ServerOraInstanceMem;

            string HistoricoSerialized = Properties.Settings.Default.HistoricoMem;
            if (HistoricoSerialized != null && !string.IsNullOrEmpty(HistoricoSerialized))
            {
                History =
                JsonSerializer.Deserialize<ObservableCollection<SysDirectory>>(HistoricoSerialized)
                ?? new ObservableCollection<SysDirectory>();
            }

            //string DatabasesSerialized = Properties.Settings.Default.DatabasesMem;
            //if (DatabasesSerialized != null && !string.IsNullOrEmpty(DatabasesSerialized))
            //{
            //    Databases =
            //        JsonSerializer.Deserialize<ObservableCollection<DatabaseModel>>(DatabasesSerialized)
            //        ?? new ObservableCollection<DatabaseModel>();
            //}
        }

        public void ClearApp()
        {
            //conexaoFile = "";
            //selectedBase = "";
            exeFile = "";
            LocalSQLServerConnection.Server = "";
            LocalOracleConnection.Server = "";
            LocalOracleConnection.Password = "";
            LocalOracleConnection.Port = "1521";
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
