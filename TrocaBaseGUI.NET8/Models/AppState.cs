using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Models
{
    public class AppState : INotifyPropertyChanged
    {
        public AppState() { }

        private List<SysDirectoryModel> history;
        public List<SysDirectoryModel> History
        {
            get => history;
            set
            { history = value; OnPropertyChanged(); }
        }

        private List<DatabaseModel> databases;
        public List<DatabaseModel> Databases
        {
            get => databases;
            set
            { databases = value; OnPropertyChanged(); }
        }

        private string exeFile;
        public string ExeFile 
        {
            get => exeFile;
            set
            { exeFile = value; OnPropertyChanged(); }
        }

        private string conexaoFile;
        public string ConexaoFile
        {
            get => conexaoFile;
            set
            { conexaoFile = value; OnPropertyChanged(); }
        }
        private ConexaoFileModel conexao2Camadas = new ConexaoFileModel() { Tier = 2 };
        public ConexaoFileModel Conexao2Camadas
        {
            get => conexao2Camadas;
            set
            { conexao2Camadas = value; OnPropertyChanged(); }
        }
        private ConexaoFileModel conexao3Camadas = new ConexaoFileModel() { Tier = 3 };
        public ConexaoFileModel Conexao3Camadas
        {
            get => conexao3Camadas;
            set
            { conexao3Camadas = value; OnPropertyChanged(); }
        }

        private SqlServerConnectionModel localSQLServerConnection;
        public SqlServerConnectionModel LocalSQLServerConnection
        {
            get => localSQLServerConnection;
            set
            { localSQLServerConnection = value; OnPropertyChanged(); }
        }


        private SqlServerConnectionModel serverSQLServerConnection;
        public SqlServerConnectionModel ServerSQLServerConnection
        {
            get => serverSQLServerConnection;
            set
            { serverSQLServerConnection = value; OnPropertyChanged(); }
        }


        private OracleConnectionModel localOracleConnection;
        public OracleConnectionModel LocalOracleConnection
        {
            get => localOracleConnection;
            set
            { localOracleConnection = value; OnPropertyChanged(); }
        }


        private OracleConnectionModel serverOracleConnection;
        public OracleConnectionModel ServerOracleConnection
        {
            get => serverOracleConnection;
            set
            { serverOracleConnection = value; OnPropertyChanged(); }
        }

        private AppParams localParams = new AppParams();
        public AppParams LocalParams
        {
            get => localParams;
            set
            { localParams = value; OnPropertyChanged(); }
        }

        private AppParams serverParams = new AppParams();
        public AppParams ServerParams
        {
            get => serverParams;
            set
            { serverParams = value; OnPropertyChanged(); }
        }

        private string selectedFolder;
        public string SelectedFolder
        {
            get => selectedFolder;
            set
            { selectedFolder = value; OnPropertyChanged(); }
        }

        private bool copy2TParams;
        public bool Copy2TParams
        {
            get => copy2TParams;
            set
            { copy2TParams = value; OnPropertyChanged(); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
