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
        public SqlServerConnectionModel LocalSQLServerConnection { get; set; } = new SqlServerConnectionModel();
        public SqlServerConnectionModel ServerSQLServerConnection { get; set; } = new SqlServerConnectionModel();
        public OracleConnectionModel LocalOracleConnection { get; set; } = new OracleConnectionModel() { Environment = "local" };
        public OracleConnectionModel ServerOracleConnection { get; set; } = new OracleConnectionModel() { Environment = "server" };

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //public void SaveState(AppState appState)
        //{
        //    var state = new AppState
        //    {
        //        History = appState.History.ToList(),
        //        Databases = appState.Databases.ToList(),
        //        ExeFile = appState.ExeFile,
        //        ConexaoFile = appState.ConexaoFile,
        //        LocalSQLServerConnection = appState.LocalSQLServerConnection,
        //        ServerSQLServerConnection = appState.ServerSQLServerConnection,
        //        LocalOracleConnection = appState.LocalOracleConnection,
        //        ServerOracleConnection = appState.ServerOracleConnection,
        //        LocalParams = appState.LocalParams ?? new AppParams(),
        //        ServerParams = appState.ServerParams ?? new AppParams()
        //    };

        //    string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
        //    Properties.Settings.Default.AppStateJson = json;
        //    Properties.Settings.Default.Save();
        //}

        //public void LoadState(AppState appState)
        //{
        //    string json = Properties.Settings.Default.AppStateJson;

        //    if (!string.IsNullOrEmpty(json))
        //    {
        //        var state = JsonSerializer.Deserialize<AppState>(json);

        //        if (state != null)
        //        {
        //            appState.History = new ObservableCollection<SysDirectoryModel>(state.History).ToList();
        //            appState.Databases = new ObservableCollection<DatabaseModel>(state.Databases).ToList();
        //            appState.ExeFile = state.ExeFile;
        //            appState.ConexaoFile = state.ConexaoFile;

        //            appState.LocalSQLServerConnection = state.LocalSQLServerConnection;
        //            appState.ServerSQLServerConnection = state.ServerSQLServerConnection;
        //            appState.LocalOracleConnection = state.LocalOracleConnection;
        //            appState.ServerOracleConnection = state.ServerOracleConnection;

        //            appState.LocalParams = state.LocalParams ?? new AppParams();
        //            appState.ServerParams = state.ServerParams ?? new AppParams();

        //            //appState = state;
        //        }
        //    }
        //}

        public void SaveState(MainViewModel vm)
        {
            var state = new AppState
            {
                History = vm.History.ToList(),
                Databases = vm.Databases.ToList(),
                ExeFile = MainViewModel.exeFile,
                ConexaoFile = vm.conexaoFile,
                LocalSQLServerConnection = vm.LocalSQLServerConnection,
                ServerSQLServerConnection = vm.ServerSQLServerConnection,
                LocalOracleConnection = vm.LocalOracleConnection,
                ServerOracleConnection = vm.ServerOracleConnection,
                LocalParams = vm.appState.LocalParams ?? new AppParams(),
                ServerParams = vm.appState.ServerParams ?? new AppParams()
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
                    vm.History = new ObservableCollection<SysDirectoryModel>(state.History);
                    vm.Databases = new ObservableCollection<DatabaseModel>(state.Databases);
                    MainViewModel.exeFile = state.ExeFile;
                    vm.conexaoFile = state.ConexaoFile;

                    vm.LocalSQLServerConnection = state.LocalSQLServerConnection;
                    vm.ServerSQLServerConnection = state.ServerSQLServerConnection;
                    vm.LocalOracleConnection = state.LocalOracleConnection;
                    vm.ServerOracleConnection = state.ServerOracleConnection;

                    vm.appState.LocalParams = state.LocalParams ?? new AppParams();
                    vm.appState.ServerParams = state.ServerParams ?? new AppParams();

                    //appState = state;
                }
            }
        }
    }
}
