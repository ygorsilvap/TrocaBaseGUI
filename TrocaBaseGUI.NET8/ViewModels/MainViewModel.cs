using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Windows;
using System.Windows.Forms;
using Microsoft.IdentityModel.Tokens;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.Services;
using MessageBox = System.Windows.MessageBox;

namespace TrocaBaseGUI.ViewModels
{

    //REFATORAR TODO O TRATAMENTO DE EXEFILE E SYSDIRECTORY//TALVEZ ADD INDEX NO SYSDIRECTORY
    public class MainViewModel
    {
        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ??= new MainViewModel();
        public ConexaoFileService conexaoFileService { get; set; }
        public string conexaoFile
        {
            get => conexaoFileService.ConexaoFile;
            set => conexaoFileService.ConexaoFile = value;
        }
        public string conexaoServidorFile
        {
            get => conexaoFileService.ConexaoServidorFile;
            set => conexaoFileService.ConexaoServidorFile = value;
        }
        public string conexaoRedirecionadorFile
        {
            get => conexaoFileService.ConexaoRedirecionadorFile;
            set => conexaoFileService.ConexaoRedirecionadorFile = value;
        }
        public string conexaoClienteFile
        {
            get => conexaoFileService.ConexaoClienteFile;
            set => conexaoFileService.ConexaoClienteFile = value;
        }

        public static string exeFile;

        public ConexaoFileModel Conexao2Camadas { get; set; } = new ConexaoFileModel() { Tier = 2 };
        public ConexaoFileModel Conexao3Camadas { get; set; } = new ConexaoFileModel() { Tier = 3 };

        public SqlServerConnectionModel LocalSQLServerConnection { get; set; } = new SqlServerConnectionModel();
        public SqlServerConnectionModel ServerSQLServerConnection { get; set; } = new SqlServerConnectionModel();
        public OracleConnectionModel LocalOracleConnection { get; set; } = new OracleConnectionModel() { Environment = "local" };
        public OracleConnectionModel ServerOracleConnection { get; set; } = new OracleConnectionModel() { Environment = "server" };
        public OracleService OracleService;
        public SqlServerService SqlService;
        public ObservableCollection<DatabaseModel> Databases { get; set; } = new ObservableCollection<DatabaseModel>();
        public DatabaseModel SelDatabase { get; set; } = new DatabaseModel();

        private const int MaxHistory = 10;
        public ObservableCollection<SysDirectoryModel> History { get; set; } = new ObservableCollection<SysDirectoryModel>();
        public ObservableCollection<string> ExeFilesList { get; set; } = new ObservableCollection<string>();
        public AppState appState { get; set; } = new AppState();
        public AppStateService appStateService { get; set; } = new AppStateService();

        public MainViewModel()
        {
            conexaoFileService = new ConexaoFileService();

            appStateService.LoadState(this);

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
            int tier = conexaoService.GetTier(conexaoService.ConexaoFilePath);

            if (tier == 3 && !conexaoService.is3CSettingsValid(Conexao3Camadas))
            {
                return;
            }

            //Rever a necessidade disso
            string cnxPath = conexaoFileService.ConexaoFilePath;
            string selectedCnx = History.Any(d => d.Path.Equals(cnxPath)) ?
                History.FirstOrDefault(d => d.Path.Equals(cnxPath)).Path : string.Empty;

            if (string.IsNullOrEmpty(selectedCnx))
                return;

            string newConn = dbs[id].DbType.ToLower().StartsWith("s")
               ? $"{SqlService.CreateSQLServerConnectionString(dbs[id].Environment, dbs[id].Name, dbs[id].Server)}\n\n"
               : $"{OracleService.CreateOracleConnectionString(dbs[id].Environment, dbs[id].Server, dbs[id].Instance, dbs[id].Name)}\n\n";

            if(tier > 2)
            {
                File.WriteAllText(conexaoServidorFile, string.Concat(newConn, conexaoService.Create3CConnectionServerFileSettings(Conexao3Camadas, appState.ServerParams)));
                File.WriteAllText(conexaoClienteFile, string.Concat(conexaoService.Create3CConnectionClientFileSettings(Conexao3Camadas , appState.ServerParams)));
            } else
            {
                File.WriteAllText(conexaoFile, string.Concat(newConn, conexaoService.Create2CConnectionFileSettings(Conexao2Camadas, appState.LocalParams)));
            }

                DatabaseModel.SetSelection(dbs, dbs[id].Id);
            SysDirectoryModel.GetDir(History, dirSys).SelectedBase = dbs[id].Id;
            SelDatabase = dbs[id];
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
            var existente = History.FirstOrDefault(d => d.Folder == folder);
            if (existente != null)
            {
                History.Remove(existente);
            }

            History.Insert(0, new SysDirectoryModel(folder, path, exe, exeList));
            exeFile = exe;

            while (History.Count > MaxHistory)
            {
                History.RemoveAt(History.Count - 1);
            }
        }

        //Refazer
        public void ClearApp()
        {
            exeFile = "";
            LocalSQLServerConnection.Server = "";
            LocalOracleConnection.Server = "";
            LocalOracleConnection.Password = "";
            LocalOracleConnection.Port = "1521";
            Databases = new ObservableCollection<DatabaseModel>();

            History.Clear();
            //Properties.Settings.Default.HistoricoMem = "";
            Properties.Settings.Default.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
