using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TrocaBaseGUI.Models
{
    public class ConexaoFileModel : INotifyPropertyChanged

    {
        public ConexaoFileModel() 
        {
            if(Tier == 3)
            {
                ports = CreateDefaultAppConn();
            }
        }

        ////Construtor 2Camadas
        //public ConexaoFileModel(string path, int tier, string defaultLogin = null, string defaultPassword = null,
        //    string textEditorPath = null, string updateFolder = null, string useWebMenu = null)
        //{
        //    Path = path;
        //    Tier = tier;
        //    DefaultLogin = defaultLogin;
        //    DefaultPassword = defaultPassword;
        //    TextEditorPath = textEditorPath;
        //    UpdateFolder = updateFolder;
        //    UseWebMenu = useWebMenu;
        //    UseRedirect = null;
        //    RedirectPort = null;
        //    ServerPorts = null;
        //    ClientPorts = null;
        //    RedirectPorts = null;
        //}

        ////Construtor 3Camadas Portas Padrões
        //public ConexaoFileModel(string path, int tier, string defaultLogin = null, string defaultPassword = null,
        //    string textEditorPath = null, string updateFolder = null, string useWebMenu = null, string useRedirect = null, string redirectPort = null)
        //{
        //    Path = path;
        //    Tier = tier;
        //    DefaultLogin = defaultLogin;
        //    DefaultPassword = defaultPassword;
        //    TextEditorPath = textEditorPath;
        //    UpdateFolder = updateFolder;
        //    UseWebMenu = useWebMenu;
        //    UseRedirect = useRedirect;
        //    RedirectPort = redirectPort;
        //    ServerPorts = CreateDefaultAppConn();
        //    ClientPorts = CreateDefaultAppConn();
        //    RedirectPorts = CreateDefaultAppConnNames();
        //}

        ////Construtor 3Camadas Portas Customizadas
        //public ConexaoFileModel(string path, int tier, string redirectPort,
        //    ObservableCollection<AppConn> serverPorts, ObservableCollection<AppConn> clientPorts,
        //    ObservableCollection<AppConn> redirectPorts, string defaultLogin = null, string defaultPassword = null,
        //    string textEditorPath = null, string updateFolder = null, string useWebMenu = null, string useRedirect = null)
        //{
        //    Path = path;
        //    Tier = tier;
        //    DefaultLogin = defaultLogin;
        //    DefaultPassword = defaultPassword;
        //    TextEditorPath = textEditorPath;
        //    UpdateFolder = updateFolder;
        //    UseWebMenu = useWebMenu;
        //    UseRedirect = useRedirect;
        //    RedirectPort = redirectPort;
        //    ServerPorts = serverPorts ?? CreateDefaultAppConn();
        //    ClientPorts = clientPorts ?? CreateDefaultAppConn();
        //    RedirectPorts = redirectPorts ?? CreateDefaultAppConnNames();
        //}

        private static readonly string[] AppNames = new[]
        {
            "Apollo", "Apollo Fiscal", "Auxiliar", "Controle Patrimonial", "EIS",
            "Fabrica", "Fabrica Ford", "Fabrica GM", "Fabrica MBB", "Fabrica VW",
            "Fabrica HD", "Fabrica Fiat", "Frente Caixa", "Gerencial",
            "Nota Fiscal Eletronica", "Relatorios Apollo", "Sped Contabil",
            "Central Agendamentos"
        };

        private static readonly string[] AppDefaultPorts = new[]
        { 
            "211", "213", "214", "215", "216", "217", "218", "219", "220", 
            "221", "222", "223", "224", "225", "226", "227", "228", "229"
        };

        //private string path;
        //public string Path
        //{
        //    get => path;
        //    set
        //    { path = value; OnPropertyChanged(); }
        //}

        private int tier;
        public int Tier
        {
            get => tier;
            set
            { tier = value; OnPropertyChanged(); }
        }

        private string defaultLogin;
        public string DefaultLogin
        {
            get => defaultLogin;
            set
            { defaultLogin = value; OnPropertyChanged(); }
        }

        private string defaultPassword;
        public string DefaultPassword
        {
            get => defaultPassword;
            set
            { defaultPassword = value; OnPropertyChanged(); }
        }

        private string textEditorPath;
        public string TextEditorPath
        {
            get => textEditorPath;
            set
            { textEditorPath = value; OnPropertyChanged(); }
        }

        private string updateFolder;
        public string UpdateFolder
        {
            get => updateFolder;
            set
            { updateFolder = value; OnPropertyChanged(); }
        }

        private bool useWebMenu;
        public bool UseWebMenu
        {
            get => useWebMenu;
            set
            { useWebMenu = value; OnPropertyChanged(); }
        }

        private bool useRedirect;
        public bool UseRedirect
        {
            get => useRedirect;
            set
            { useRedirect = value; OnPropertyChanged(); }
        }

        private string redirectPort = "240";
        public string RedirectPort
        {
            get => redirectPort;
            set
            { redirectPort = value; OnPropertyChanged(); }
        }

        private string dbServer;
        public string DbServer
        {
            get => dbServer;
            set
            { dbServer = value; OnPropertyChanged(); }
        }

        private string verifierPort = "210";
        public string VerifierPort
        {
            get => verifierPort;
            set
            { verifierPort = value; OnPropertyChanged(); }
        }

        private ObservableCollection<AppConn> ports = CreateDefaultAppConn();
        public ObservableCollection<AppConn> Ports
        {
            get => ports;
            set { ports = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private static ObservableCollection<AppConn> CreateDefaultAppConnNames()
        {
            var list = new ObservableCollection<AppConn>();
            foreach (var nome in AppNames)
                list.Add(new AppConn { App = nome });
            return list;
        }

        private static ObservableCollection<AppConn> CreateDefaultAppConn()
        {
            var list = CreateDefaultAppConnNames();
            for (int i = 0; i < list.Count && i < AppDefaultPorts.Length; i++)
            {
                list[i].Port = AppDefaultPorts[i];
            }
            return list;
        }

    }
}
