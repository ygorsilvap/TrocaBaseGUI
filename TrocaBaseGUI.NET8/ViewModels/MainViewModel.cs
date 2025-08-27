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
using Oracle.ManagedDataAccess.Client;

namespace TrocaBaseGUI.ViewModels
{

    //REFATORAR TODO O TRATAMENTO DE EXEFILE E SYSDIRECTORY//TALVEZ ADD INDEX NO SYSDIRECTORY
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
        public SqlServerConnectionModel ServerSQLServerConnection { get; set; } = new SqlServerConnectionModel();
        public OracleConnectionModel LocalOracleConnection { get; set; } = new OracleConnectionModel() { Environment = "local" };
        public OracleConnectionModel ServerOracleConnection { get; set; } = new OracleConnectionModel() { Environment = "server" };
        public OracleService OracleService;
        public SqlServerService SqlService;
        public ObservableCollection<DatabaseModel> Databases { get; set; } = new ObservableCollection<DatabaseModel>();
        public DatabaseModel SelDatabase { get; set; } = new DatabaseModel();

        private const int MaxHistory = 10;
        public ObservableCollection<SysDirectory> History { get; set; } = new ObservableCollection<SysDirectory>();
        public ObservableCollection<string> ExeFilesList { get; set; } = new ObservableCollection<string>();

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

        //Refatorar
        public async Task openSqlConn(SqlServerService sqlservice, SqlServerConnectionModel sqlServerConnection)
        {
            //Revisar lógica
            if (sqlServerConnection.IsValid())
                return;

            if (await sqlservice.ValidateConnection(sqlServerConnection))
            {
                var databases = await sqlservice.GetDatabases(sqlServerConnection);

                databases.ForEach(db => {
                    if (Databases.Any(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase) &&
                    d.Environment.Equals(db.Environment, StringComparison.OrdinalIgnoreCase)))
                    {
                        return;
                    }
                    Databases.Add(db);
                    Databases[Databases.Count - 1].Id = Databases.Count - 1;
                });
                Console.WriteLine("\n[Conexão com SQL Server estabelecida]\n");
            }
            else
            {
                if (Databases.Count() > 0)
                {
                    //Refatorar
                    var environment = string.IsNullOrEmpty(sqlServerConnection.Password) ? "local" : "server";
                    var removable = Databases
                        .Where(item => item.DbType != null && item.DbType.ToLower().StartsWith("s") && item.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    foreach (var item in removable)
                    {
                        Databases.Remove(item);
                    }
                }
                Console.WriteLine("\n[Conexão com SQL Server inválida]\n");
            }
        }
        public async Task openOracleConn(OracleService oracleService, OracleConnectionModel oracleConnection)
        {
            //Revisar lógica
            if (oracleConnection.IsValid())
                return;

            List<String> instances = string.IsNullOrEmpty(oracleConnection.Instance) ?
                oracleService.GetRunningInstances() : new List<string> { oracleConnection.Instance };

            foreach (string instance in instances)
            {
                if (await OracleService.ValidateConnection(oracleConnection, instance))
                {
                    List<DatabaseModel> dbs = await oracleService.GetDatabases(oracleConnection, instance);
                    dbs.ForEach(db =>
                    {
                        if (Databases.Any(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase) &&
                                          d.Environment.Equals(db.Environment, StringComparison.OrdinalIgnoreCase)))
                            return;

                        Databases.Add(db);
                        Databases[Databases.Count - 1].Id = Databases.Count - 1;
                    });
                    return;
                }

                if (Databases.Count() > 0)
                {
                    var removable = Databases
                        .Where(db => db.DbType != null &&
                            db.DbType.ToLower().StartsWith("o") &&
                            db.Environment.Equals(oracleConnection.Environment, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    removable.ForEach(db => {
                        Debug.WriteLine($"\nRemovendo {db.Name} - {db.Environment}\n");
                        Databases.Remove(db);
                    });
                }
            }
        }
        public void SelectBase(ObservableCollection<DatabaseModel> dbs, int id, string dirSys)
        {
            var conexaoService = conexaoFileService;
            List<String> conexaoLines = File.ReadAllLines(conexaoFile).ToList();
            int bancoIndex = conexaoLines.FindIndex(line => line.IndexOf("[BANCODADOS]", StringComparison.OrdinalIgnoreCase) >= 0);
            int index = 0;

            string cnxPath = conexaoFileService.ConexaoFilePath;
            string selectedCnx = History.Any(d => d.Path.Equals(cnxPath)) ?
                History.FirstOrDefault(d => d.Path.Equals(cnxPath)).Path : string.Empty;

            if (string.IsNullOrEmpty(selectedCnx))
                return;

            if (conexaoLines.Count > 0)
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

            string newConn = dbs[id].DbType.ToLower().StartsWith("s")
               ? SqlService.CreateSQLServerConnectionString(dbs[id].Environment, dbs[id].Name, dbs[id].Server)
               : OracleService.CreateOracleConnectionString(dbs[id].Environment, dbs[id].Server, dbs[id].Instance, dbs[id].Name);

            var newConnLines = newConn.Split('\n');

            //Adiciona a nova string
            conexaoLines.InsertRange(index, newConnLines);

            File.WriteAllLines(conexaoFile, conexaoLines);

            DatabaseModel.SetSelection(dbs, dbs[id].Id);
            SysDirectory.GetDir(History, dirSys).SelectedBase = dbs[id].Id;
            SelDatabase = dbs[id];

            //Debug.WriteLine($"\n\nid: {History.FirstOrDefault(d => d.SelectedBase > -1).SelectedBase}, name: {id}");
        }

        public ObservableCollection<DatabaseModel> EnvironmentFilter(string environment, ObservableCollection<DatabaseModel> db)
        {
            return new ObservableCollection<DatabaseModel>(db.Where(i => i.Environment.Equals(environment)));
        }

        public ObservableCollection<DatabaseModel> DbTypeFilter(string type, ObservableCollection<DatabaseModel> db)
        {
            return new ObservableCollection<DatabaseModel>(db.Where(i => i.DbType.Equals(type, StringComparison.OrdinalIgnoreCase)));
        }

        public void AddDirectory(string folder, string path, string exe, ObservableCollection<string> exeList)
        {
            // Verifica se já existe o diretório no histórico
            var existente = History.FirstOrDefault(d => d.Folder == folder);
            if (existente != null)
            {
                History.Remove(existente); // Remove para mover para o topo
            }

            // Adiciona no início da lista
            History.Insert(0, new SysDirectory(folder, path, exe, exeList));
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

            string DatabasesSerialized = Properties.Settings.Default.DatabasesMem;
            if (DatabasesSerialized != null && !string.IsNullOrEmpty(DatabasesSerialized))
            {
                Databases =
                    JsonSerializer.Deserialize<ObservableCollection<DatabaseModel>>(DatabasesSerialized)
                    ?? new ObservableCollection<DatabaseModel>();
            }
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
