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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
