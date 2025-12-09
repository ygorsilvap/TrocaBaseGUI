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

        public SqlServerConnectionModel LocalSQLServerConnection { get; set; } = new SqlServerConnectionModel();
        public SqlServerConnectionModel ServerSQLServerConnection { get; set; } = new SqlServerConnectionModel();
        public OracleConnectionModel LocalOracleConnection { get; set; } = new OracleConnectionModel() { Environment = "local" };
        public OracleConnectionModel ServerOracleConnection { get; set; } = new OracleConnectionModel() { Environment = "server" };
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
                var databases = await sqlservice.GetSqlServerDatabases(sqlServerConnection);

                databases.ForEach(db => {
                    if (Databases.Any(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase) &&
                    d.Environment.Equals(db.Environment, StringComparison.OrdinalIgnoreCase) &&
                    d.DbType.Equals(db.DbType, StringComparison.OrdinalIgnoreCase)))
                        return;

                    Databases.Add(db);

                    //Validar essa solução antes de usar
                    //Databases[Databases.Count - 1].Id = Databases.Any(db => db.Id.Equals((Databases.Count - 1))) ? Databases[Databases.Count - 1].Id : Databases.Count - 1;

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
                //DbService.SortDatabases(Databases);

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
                    var databases = await oracleService.GetOracleDatabases(oracleConnection, instance);
                    databases.ForEach(db =>
                    {
                        if (Databases.Any(d => d.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase) &&
                                          d.Environment.Equals(db.Environment, StringComparison.OrdinalIgnoreCase) &&
                                          d.DbType.Equals(db.DbType, StringComparison.OrdinalIgnoreCase)))
                            return;

                        Databases.Add(db);
                        Databases[Databases.Count - 1].Id = Databases.Count - 1;
                    });
                    //DbService.SortDatabases(Databases);
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
                    //DbService.SortDatabases(Databases);
                }
            }
        }

        public void SelectBase(ObservableCollection<DatabaseModel> dbs, int id, SysDirectoryModel sysDirectory)
        {
            //foreach (var item in Databases)
            //{
            //    Debug.WriteLine($"\n Id: {item.Id}, Database: {item.Name}, Type: {item.DbType}, Environment: {item.Environment}, Server: {item.Server}\n");
            //}
            var conexaoService = conexaoFileService;
            //int tier = conexaoService.GetTier(conexaoService.ConexaoFilePath);

            if (sysDirectory.Tier == 3 && !conexaoService.is3CSettingsValid(Conexao3Camadas))
            {
                return;
            }

            //Rever a necessidade disso
            //string cnxPath = conexaoFileService.ConexaoFilePath;
            //string selectedCnx = SysDirectoryList.Any(d => d.Path.Equals(cnxPath)) ?
            //    SysDirectoryList.FirstOrDefault(d => d.Path.Equals(cnxPath)).Path : string.Empty;

            if (string.IsNullOrEmpty(sysDirectory.Path))
                return;

            var selectedDb = dbs.FirstOrDefault(db => db.Id.Equals(id));

            string newConn = selectedDb.DbType.ToLower().StartsWith("s")
               ? $"{SqlService.CreateSQLServerConnectionString(selectedDb.Environment, selectedDb.Name, selectedDb.Server)}\n\n"
               : $"{OracleService.CreateOracleConnectionString(selectedDb.Environment, selectedDb.Server, selectedDb.Instance, selectedDb.Name)}\n\n";

            if (sysDirectory.Tier == 3)
            {
                //File.WriteAllText(conexaoServidorFile, string.Concat(newConn, conexaoService.Create3CConnectionServerFileSettings(Conexao3Camadas, appState.ServerParams)));
                //File.WriteAllText(conexaoClienteFile, string.Concat(conexaoService.Create3CConnectionClientFileSettings(Conexao3Camadas , appState.ServerParams)));
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

        //public void AddDirectory(string folder, string path, string exe, ObservableCollection<string> exeList)
        //{
        //    var existente = SysDirectoryList.FirstOrDefault(d => d.Folder == folder);
        //    if (existente != null)
        //    {
        //        SysDirectoryList.Remove(existente);
        //    }

        //    SysDirectoryList.Insert(0, new SysDirectoryModel(folder, path, exe, exeList));
        //    exeFile = exe;

        //    while (SysDirectoryList.Count > MaxSysDirectorySize)
        //    {
        //        SysDirectoryList.RemoveAt(SysDirectoryList.Count - 1);
        //    }
        //}

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
