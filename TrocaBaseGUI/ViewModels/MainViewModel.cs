using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using TrocaBaseGUI.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Net;
using Oracle.ManagedDataAccess.Client;
using System.ServiceProcess;
using System.Windows.Shapes;
using TrocaBaseGUI.Services;
using System.ServiceModel.Channels;

namespace TrocaBaseGUI.ViewModels
{
    public class MainViewModel
    {
        //public static ConexaoFileService conexaoFileService { get; set; } = new ConexaoFileService();
        //public string ConexaoFile = conexaoFileService.ConexaoFile;
        public static string ConexaoFile = new ConexaoFileService().ConexaoFile;
        static string selectedBase;
        public static string exeFile;
        public string hostname;

        public SqlServerConnectionModel SQLServerConnection { get; set; } = new SqlServerConnectionModel();

        public ObservableCollection<DatabaseModel> Databases { get; set; } = new ObservableCollection<DatabaseModel>();

        private const int MaxHistory = 10;
        public ObservableCollection<SysDirectory> History { get; set; } = new ObservableCollection<SysDirectory>();
        public MainViewModel()
        {
            LoadState();
            var sqlService = new SqlServerService(SQLServerConnection);
            sqlService.LoadSqlServerDatabases("MTZNOTFS058680").ForEach(db => Databases.Add(db));
            

            var oracleService = new OracleService();
            oracleService.GetDatabases("User Id=sys;Password=oracle;Data Source=MTZNOTFS058680:1521/LINX;DBA Privilege=SYSDBA;")
                .ForEach(db => Databases.Add(db));

            foreach (var item in Databases) DatabaseModel.SetDisplayName(item);

            hostname = Dns.GetHostName();

            foreach (var item in Databases)
            {
                Console.WriteLine("t: " + item.DbType);
            }
            Console.WriteLine(SQLServerConnection);
            Console.WriteLine("cnx: " + ConexaoFile);
        }


        public void SelectBase(ObservableCollection<DatabaseModel> dbs, string db)
        {
            var conexaoService = new ConexaoFileService();
            string oracleDb = "[BANCODADOS]=ORACLE";
            string sqlServerDb = "[BANCODADOS]=SQLSERVER";
            var conexaoLines = File.ReadAllLines(ConexaoFile).ToList();
            int bancoIndex = conexaoLines.FindIndex(line => line.IndexOf("[BANCODADOS]", StringComparison.OrdinalIgnoreCase) >= 0);
            int index;

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


            string newConn = dbs.Any(d => d.DbType.ToLower().StartsWith("s") && d.Name.Equals(db))
               ? conexaoService.CreateConnectionString(sqlServerDb, conexaoService.Domain, db)
               : conexaoService.CreateConnectionString(oracleDb, conexaoService.Domain, db);

            var newConnLines = newConn.Split('\n');

            // Adiciona a nova string
            conexaoLines.InsertRange(index, newConnLines);

            File.WriteAllLines(ConexaoFile, conexaoLines);

            if(dbs.Any(b => b.IsSelected == true)) dbs.FirstOrDefault(b => b.IsSelected == true).IsSelected = false;

            dbs.FirstOrDefault(b => b.Name.Equals(db)).IsSelected = true;

            selectedBase = db;
        }

        //public void VmSetConexaoAddress(string path)
        //{

        //    ConexaoFile.SetConexaoAddress(path);
        //}

        public ObservableCollection<DatabaseModel> InstanceFilter(string instance, ObservableCollection<DatabaseModel> db)
        {
            return new ObservableCollection<DatabaseModel>(db.Where(i => i.Instance.Equals(instance)));
        }

        public ObservableCollection<DatabaseModel> DbTypeFilter(string type, ObservableCollection<DatabaseModel> db)
        {
            return new ObservableCollection<DatabaseModel>(db.Where(i => i.DbType.Equals(type, StringComparison.OrdinalIgnoreCase)));
        }

        public void AddDirectory(string endereco, string enderecoCompleto, string exe)
        {
            // Verifica se já existe o diretório no histórico
            var existente = History.FirstOrDefault(d => d.Address == endereco);
            if (existente != null)
            {
                History.Remove(existente); // Remove para mover para o topo
            }

            // Adiciona no início da lista
            History.Insert(0, new SysDirectory(endereco, enderecoCompleto, exe));
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
                Properties.Settings.Default.historico = HistoricoSerialized;
            }

            Properties.Settings.Default.ExeFile = exeFile;
            Properties.Settings.Default.conexaoFile = ConexaoFile;
            Properties.Settings.Default.Conexao = selectedBase;
            Properties.Settings.Default.Save();
        }

        public void LoadState()
        {
            exeFile = Properties.Settings.Default.ExeFile;
            ConexaoFile = Properties.Settings.Default.conexaoFile;
            selectedBase = Properties.Settings.Default.Conexao;

            string HistoricoSerialized = Properties.Settings.Default.historico;
            if (HistoricoSerialized != null && !string.IsNullOrEmpty(HistoricoSerialized))
            {
                History =
                JsonSerializer.Deserialize<ObservableCollection<SysDirectory>>(HistoricoSerialized)
                ?? new ObservableCollection<SysDirectory>();
            }
        }

        public void ClearApp()
        {
            ConexaoFile = "";
            selectedBase = "";
            exeFile = "";

            History.Clear();
            Properties.Settings.Default.historico = "";
            Properties.Settings.Default.Save();
        }
    }
}
