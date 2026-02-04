using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Services
{
    public class AppStateService
    {
        private ObservableCollection<SysDirectoryModel> _history = new();
        public ObservableCollection<SysDirectoryModel> History
        {
            get => _history;
            set { _history = value; OnPropertyChanged(); }
        }

        private ObservableCollection<DatabaseModel> _databases = new();
        public ObservableCollection<DatabaseModel> Databases
        {
            get => _databases;
            set { _databases = value; OnPropertyChanged(); }
        }

        public string ExeFile { get; set; }
        public string ConexaoFile { get; set; }
        public ConexaoFileModel Conexao2Camadas { get; set; } = new ConexaoFileModel() { Tier = 2 };
        public ConexaoFileModel Conexao3Camadas { get; set; } = new ConexaoFileModel() { Tier = 3 };
        public SqlServerConnectionModel LocalSQLServerConnection { get; set; } = new SqlServerConnectionModel();
        public SqlServerConnectionModel ServerSQLServerConnection { get; set; } = new SqlServerConnectionModel();
        public OracleConnectionModel LocalOracleConnection { get; set; } = new OracleConnectionModel() { Environment = "local" };
        public OracleConnectionModel ServerOracleConnection { get; set; } = new OracleConnectionModel() { Environment = "server" };

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void SaveState(MainViewModel vm)
        {
            var state = new AppState
            {
                History = vm.SysDirectoryList.ToList(),
                Databases = vm.Databases.ToList(),
                //ExeFile = MainViewModel.exeFile,
                ConexaoFile = vm.conexaoFile,
                LocalSQLServerConnection = vm.LocalSQLServerConnection,
                ServerSQLServerConnection = vm.ServerSQLServerConnection,
                LocalOracleConnection = vm.LocalOracleConnection,
                ServerOracleConnection = vm.ServerOracleConnection,
                LocalParams = vm.appState.LocalParams ?? new AppParams(),
                ServerParams = vm.appState.ServerParams ?? new AppParams(),
                Conexao2Camadas = vm.Conexao2Camadas,
                Conexao3Camadas = vm.Conexao3Camadas,
                SelectedFolder  = vm.appState.SelectedFolder
            };

            string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            Properties.Settings.Default.AppStateJson = json;
            Properties.Settings.Default.Save();
        }

        public void LoadState(MainViewModel vm)
        {
            string json = Properties.Settings.Default.AppStateJson;

            if (!string.IsNullOrEmpty(json))
            {
                var state = JsonSerializer.Deserialize<AppState>(json);

                if (state != null)
                {
                    vm.SysDirectoryList = new ObservableCollection<SysDirectoryModel>(state.History);
                    vm.Databases = new ObservableCollection<DatabaseModel>(state.Databases);
                    //MainViewModel.exeFile = state.ExeFile;
                    vm.conexaoFile = state.ConexaoFile;

                    vm.Conexao2Camadas = state.Conexao2Camadas ?? new ConexaoFileModel() { Tier = 2 };
                    vm.Conexao3Camadas = state.Conexao3Camadas ?? new ConexaoFileModel() { Tier = 3 };

                    vm.LocalSQLServerConnection = state.LocalSQLServerConnection;
                    vm.ServerSQLServerConnection = state.ServerSQLServerConnection;
                    vm.LocalOracleConnection = state.LocalOracleConnection;
                    vm.ServerOracleConnection = state.ServerOracleConnection;

                    vm.appState.LocalParams = state.LocalParams ?? new AppParams();
                    vm.appState.ServerParams = state.ServerParams ?? new AppParams();
                    vm.appState.SelectedFolder = state.SelectedFolder;

                    //appState = state;
                }
            }
        }

        //public void ClearApp(MainViewModel vm)
        //{
        //    //exeFile = "";

        //    LocalSQLServerConnection = new SqlServerConnectionModel();
        //    ServerSQLServerConnection = new SqlServerConnectionModel();

        //    LocalOracleConnection = new OracleConnectionModel() { Environment = "local" };
        //    ServerOracleConnection = new OracleConnectionModel() { Environment = "server" };

        //    appState = new AppState();

        //    SysDirectoryList.Clear();
        //    Databases.Clear();

        //    vm = new MainViewModel();

        //    Properties.Settings.Default.Save();
        //}
    }
}
