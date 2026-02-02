using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Windows;
using System.Windows.Forms;
using Microsoft.IdentityModel.Tokens;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.Services;
using TrocaBaseGUI.Utils;
using MessageBox = System.Windows.MessageBox;

namespace TrocaBaseGUI.ViewModels
{
    public class MainViewModel
    {
        //Não me recordo o motivo dessas 2 declarações da MVM, provavelmente coisa do chatgpt. REMOVER.
        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ??= new MainViewModel();

        public bool firstBoot = true;
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

        //public static string exeFile;

        public ConexaoFileModel Conexao2Camadas { get; set; } = new ConexaoFileModel() { Tier = 2 };
        public ConexaoFileModel Conexao3Camadas { get; set; } = new ConexaoFileModel() { Tier = 3 };

        public SqlServerConnectionModel LocalSQLServerConnection { get; set; } = new SqlServerConnectionModel() { Environment = "local" };
        public SqlServerConnectionModel ServerSQLServerConnection { get; set; } = new SqlServerConnectionModel() 
            { Environment = "server",
              Server = "AZ-BD-AUTO-03.linx.com.br",
              Password =  "ninguemsabe"
              };
        public OracleConnectionModel LocalOracleConnection { get; set; } = new OracleConnectionModel() { Environment = "local" };
        public OracleConnectionModel ServerOracleConnection { get; set; } = new OracleConnectionModel()
        {
            Environment = "server",
            Server = "150.230.86.225",
            Instance = "pdb_auto_dev_01.sub08051803480.vcnoradev.oraclevcn.com",
            Password = "ninguemsabe"
        };
        public OracleService OracleService;
        public SqlServerService SqlService;
        //public ObservableCollection<DatabaseModel> Databases { get; set; } = new ObservableCollection<DatabaseModel>();
        public DatabaseService DbService { get; set; } = new DatabaseService();
        public ObservableCollection<DatabaseModel> Databases
        {
            get => DbService.Databases;
            set => DbService.Databases = value;
        }

        //Refatorar a tratativa de base selecionada para retirar essa variavel da vm[1]
        public DatabaseModel SelectedDatabase { get; set; } = new DatabaseModel();

        private const int MaxSysDirectorySize = 10;
        public ObservableCollection<SysDirectoryModel> SysDirectoryList { get; set; } = new ObservableCollection<SysDirectoryModel>();
        public SysDirectoryService sysDirectoryService { get; set; } = new SysDirectoryService();
        //public ObservableCollection<string> ExeFilesList { get; set; } = new ObservableCollection<string>();
        public AppState appState { get; set; } = new AppState();
        public AppStateService appStateService { get; set; } = new AppStateService();
        public bool isDbListLoading;

        //ChatGPT
        private bool _isSqlLoading;
        public bool isSqlLoading
        {
            get => _isSqlLoading;
            set
            {
                if (_isSqlLoading != value)
                {
                    _isSqlLoading = value;
                    OnPropertyChanged(nameof(isSqlLoading));
                }
            }

        }

        private bool _isOracleLoading;
        public bool isOracleLoading
        {
            get => _isOracleLoading;
            set
            {
                if (_isOracleLoading != value)
                {
                    _isOracleLoading = value;
                    OnPropertyChanged(nameof(isOracleLoading));
                }
            }
        }
        //

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

        public void AddDatabases(List<DatabaseModel> databases)
        {
            string dbType = databases.FirstOrDefault().DbType;
            string environment = databases.FirstOrDefault().Environment;

            databases.ForEach(db =>
            {
                if (Databases.Any(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase) &&
                                  d.Environment.Equals(db.Environment, StringComparison.OrdinalIgnoreCase) &&
                                  d.DbType.Equals(db.DbType, StringComparison.OrdinalIgnoreCase)))
                    return;

                db.Id = UtilityService.IdGen();
                Databases.Add(db);
                //Databases.FirstOrDefault(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase) &&
                //                  d.Environment.Equals(db.Environment, StringComparison.OrdinalIgnoreCase) &&
                //                  d.DbType.Equals(db.DbType, StringComparison.OrdinalIgnoreCase)).Id = UtilityService.IdGen();
                //Databases[Databases.Count - 1].Id = Databases.Count - 1;

            });

            foreach (var db in Databases.Where(db => db.DbType.Equals(dbType) && db.Environment.Equals(environment)).ToList())
            {
                bool isDbThere = databases.Any(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase) &&
                                  d.Environment.Equals(db.Environment, StringComparison.OrdinalIgnoreCase) &&
                                  d.DbType.Equals(db.DbType, StringComparison.OrdinalIgnoreCase));

                //base não está na lista de bases carregadas
                if (!isDbThere)
                    Databases.Remove(db);
            }
        }

        public void RemoveDatabases(List<DatabaseModel> databases)
        {
            if (databases.Count <= 0) return;

            string dbType = databases.FirstOrDefault().DbType;

            foreach (var db in databases)
            {
                Databases.Remove(db);
                if (SysDirectoryList.Any(d => d.SysDatabase == db.Id))
                {
                    SysDirectoryList.First(d => d.SysDatabase == db.Id).SysDatabase = string.Empty;
                }
            }
            if (SelectedDatabase != null && SelectedDatabase.DbType.StartsWith(dbType[0].ToString(), StringComparison.OrdinalIgnoreCase))
                SelectedDatabase = null;
        }


        //Refatorar
        public async Task OpenSqlConn(SqlServerService sqlservice, SqlServerConnectionModel sqlServerConnection, bool removeDb = false, bool showResult = true)
        {
            //Revisar lógica
            if (!sqlServerConnection.IsValid())
                return;

            if (await sqlservice.ValidateConnection(sqlServerConnection))
            {
                var databases = await sqlservice.GetSqlServerDatabases(sqlServerConnection);
                AddDatabases(databases);

                if(showResult)
                {
                    if (sqlServerConnection.Environment.Equals("server", StringComparison.OrdinalIgnoreCase))
                        MessageBox.Show("Conexão do servidor com SQL Server estabelecida.", "Conexão do servidor com SQL Server", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Conexão local com SQL Server estabelecida.", "Conexão local com SQL Server", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                if (removeDb)
                {
                    var removable = Databases
                        .Where(item => item.DbType != null && item.DbType.ToLower().StartsWith("s") && item.Environment.Equals(sqlServerConnection.Environment, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    
                    RemoveDatabases(removable);
                }
                if(showResult)
                {
                    if (sqlServerConnection.Environment.Equals("server", StringComparison.OrdinalIgnoreCase))
                        MessageBox.Show("Falha na conexão do servidor com SQL Server", "Conexão do servidor com SQL Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else
                        MessageBox.Show("Falha na conexão local com SQL Server", "Conexão local com SQL Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public async Task OpenOracleConn(OracleService oracleService, OracleConnectionModel oracleConnection, bool removeDb = false, bool showResult = true)
        {
            //Revisar lógica
            if (!oracleConnection.IsValid())
                return;

            List<String> instances = string.IsNullOrEmpty(oracleConnection.Instance) ?
                oracleService.GetRunningInstances() : new List<string> { oracleConnection.Instance };

            foreach (string instance in instances)
            {
                if (await OracleService.ValidateConnection(oracleConnection, instance, showResult))
                {
                    var databases = await oracleService.GetOracleDatabases(oracleConnection, instance);
                    AddDatabases(databases);

                    if(showResult)
                    {
                        if (oracleConnection.Environment.Equals("server", StringComparison.OrdinalIgnoreCase))
                            MessageBox.Show("Conexão do servidor com Oracle estabelecida.", "Conexão do servidor com Oracle", MessageBoxButton.OK, MessageBoxImage.Information);
                        else
                            MessageBox.Show("Conexão local com Oracle estabelecida.", "Conexão local com Oracle", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    return;
                }

                if (removeDb)
                {
                    var removable = Databases
                        .Where(db => db.DbType != null &&
                            db.DbType.ToLower().StartsWith("o") &&
                            db.Environment.Equals(oracleConnection.Environment, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    
                    RemoveDatabases(removable);
                }
            }
        }

        public void SelectDatabase(ObservableCollection<DatabaseModel> dbs, string id, SysDirectoryModel sysDirectory)
        {
            var conexaoService = conexaoFileService;

            if (sysDirectory.Tier == 3 && !conexaoService.Is3CSettingsValid(Conexao3Camadas))
            {
                return;
            }

            if (string.IsNullOrEmpty(sysDirectory.Path))
                return;

            var selectedDb = dbs.FirstOrDefault(db => db.Id.Equals(id));

            string newConn = selectedDb.DbType.ToLower().StartsWith("s")
               ? $"{SqlService.CreateSQLServerConnectionString(selectedDb.Environment, selectedDb.Name, selectedDb.Server)}\n\n"
               : $"{OracleService.CreateOracleConnectionString(selectedDb.Environment, selectedDb.Server, selectedDb.Instance, selectedDb.Name)}\n\n";

            if (sysDirectory.Tier == 3)
            {
                File.WriteAllText(conexaoServidorFile, string.Concat(newConn, conexaoService.Create3CConnectionServerFileSettings(Conexao3Camadas, appState.ServerParams)));
                File.WriteAllText(conexaoClienteFile, string.Concat(conexaoService.Create3CConnectionClientFileSettings(Conexao3Camadas, appState.ServerParams)));
            } else
            {
                File.WriteAllText(conexaoFile, string.Concat(newConn, conexaoService.Create2CConnectionFileSettings(Conexao2Camadas, appState.LocalParams)));
            }

            DatabaseService.SetSelection(dbs, selectedDb.Id);
            sysDirectoryService.GetDir(SysDirectoryList, sysDirectory.Path).SysDatabase = selectedDb.Id;
            SelectedDatabase = selectedDb;
        }

        public ObservableCollection<DatabaseModel> EnvironmentFilter(string environment, ObservableCollection<DatabaseModel> db)
        {
            return new ObservableCollection<DatabaseModel>(db.Where(i => i.Environment.Equals(environment)));
        }

        public ObservableCollection<DatabaseModel> DbTypeFilter(string type, ObservableCollection<DatabaseModel> db)
        {
            return new ObservableCollection<DatabaseModel>(db.Where(i => i.DbType.Equals(type, StringComparison.OrdinalIgnoreCase)));
        }

        //Refazer
        public void ClearApp()
        {
            //exeFile = "";

            LocalSQLServerConnection = new SqlServerConnectionModel();
            ServerSQLServerConnection = new SqlServerConnectionModel();

            LocalOracleConnection = new OracleConnectionModel() { Environment = "local" };
            ServerOracleConnection = new OracleConnectionModel() { Environment = "server" };

            appState = new AppState();

            SysDirectoryList.Clear();
            Databases.Clear();

            Properties.Settings.Default.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
