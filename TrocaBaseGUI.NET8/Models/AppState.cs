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


        private bool defaultLoginCheckbox;
        public bool DefaultLoginCheckbox
        {
            get => defaultLoginCheckbox;
            set
            { defaultLoginCheckbox = value; OnPropertyChanged(); }
        }

        private bool defaultPasswordCheckbox;
        public bool DefaultPasswordCheckbox
        {
            get => defaultPasswordCheckbox;
            set
            { defaultPasswordCheckbox = value; OnPropertyChanged(); }
        }

        private bool editorCheckbox;
        public bool EditorCheckbox
        {
            get => editorCheckbox;
            set
            { editorCheckbox = value; OnPropertyChanged(); }
        }

        private bool dirUpdateCheckbox;
        public bool DirUpdateCheckbox
        {
            get => dirUpdateCheckbox;
            set
            { dirUpdateCheckbox = value; OnPropertyChanged(); }
        }

        private bool useWebMenuCheckbox;
        public bool UseWebMenuCheckbox
        {
            get => useWebMenuCheckbox;
            set
            { useWebMenuCheckbox = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
