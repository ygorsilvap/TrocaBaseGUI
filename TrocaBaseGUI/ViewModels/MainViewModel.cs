using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text.Json;
using System.Windows;
using TrocaBaseGUI.Models;

namespace TrocaBaseGUI.ViewModels
{
    //validar se o conexao.dat tem conteúdo inválido
    //decidir se haverá a separação do cabeçalho dos arquivos de base
    //colocar as mensagens de pasta de base vazia e pasta do sistema vazio
    //criar mais validações com messagebox 
    //colocar botão para escolher o executável do sistema
    //colocar botão para fechar e abrir o sistema
    //alternativo, trocar o botão de escolha da pasta do sistema para o executável do sistema, através disso fazer o registro dos caminhnos como já está sendo mas aproveitar o input
        //para definir o caminho do .exe
    //colocar um botão para limpar toda as seleções e excluir histórico
    //aumentar o limite do histórico de 5 para 10
    //possíveis pequenas alterações no layout
    //refatoração rápida
    //proteger a aplicação com try catch

    internal class MainViewModel
    {
        public static string DbDirectory;
        public static string ConexaoFile;
        static string conexao = "";

        static List<string> allFiles;
        private const int MaxHistory = 5;
        public ObservableCollection<SysDirectory> History { get; set; } = new ObservableCollection<SysDirectory>();
        public ObservableCollection<Banco> dbFiles { get; set; }
        public MainViewModel()
        {
            LoadState();

            if ((!string.IsNullOrWhiteSpace(DbDirectory) && Directory.Exists(DbDirectory)) && (!string.IsNullOrWhiteSpace(ConexaoFile) && File.Exists(ConexaoFile)))
            {
                allFiles = new List<string>(Directory.GetFiles(DbDirectory, "*.dat"));
                dbFiles = FilterFiles(allFiles);
                init(dbFiles, allFiles);
            }
        }

        static void init(ObservableCollection<Banco> db, List<string> allFiles)
        {
            if (!string.IsNullOrWhiteSpace(ConexaoFile) && File.Exists(ConexaoFile) && !string.IsNullOrEmpty(File.ReadAllText(ConexaoFile))) 
            {
                var linha = File.ReadLines(ConexaoFile).FirstOrDefault().Remove(0, 12);
                conexao = linha != null ? ToCapitalize(linha) : "";
                SelectBase(db);
            }
        }

        static ObservableCollection<Banco> FilterFiles(List<string> list)
        {
            ObservableCollection<Banco> filteredFiles = new ObservableCollection<Banco>();

            string oracleDb = "[BANCODADOS]=ORACLE";
            string sqlServerDb = "[BANCODADOS]=SQLSERVER";

            string serverOracle = "[DATABASE]=150.230.86.225";
            string serverSqlServer = "[DATABASE]=AZ-BD-AUTO-03";

            string nameTag = "[NOMEBANCO]";

            if (string.IsNullOrEmpty(DbDirectory) || string.IsNullOrEmpty(ConexaoFile)) return new ObservableCollection<Banco>();

            foreach (string file in list)
            {
                if (File.ReadLines(file).ToList().Any(n => n.IndexOf(nameTag, StringComparison.OrdinalIgnoreCase) >= 0) && !(Path.GetFileName(file).Equals("conexao.dat")))
                {
                    filteredFiles.Add(new Banco { FileName = Path.GetFileName(file.Replace(".dat", "")) });
                };
            }

            for (int i = 0; i < filteredFiles.Count; i++)
            {
                var lines = File.ReadLines($@"{DbDirectory}\{filteredFiles[i].FileName}.dat").ToList();

                //Define nome do banco e nicial do nome do banco maiúscula
                string dbName = ToCapitalize((lines.FirstOrDefault(b => b.Contains(nameTag)) ?? string.Empty).Remove(0, 12));

                //Define o tipo de base(oracle, sqlserver)
                string dbType =
                    lines.Any(l => l.IndexOf(oracleDb, StringComparison.OrdinalIgnoreCase) >= 0) ? "Oracle" :
                    lines.Any(l => l.IndexOf(sqlServerDb, StringComparison.OrdinalIgnoreCase) >= 0) ? "SQLServer" : "Inválido";

                //Define o tipo de instância(local, server)
                string instanceType =
                    lines.Any(l => l.IndexOf(serverOracle, StringComparison.OrdinalIgnoreCase) >= 0) ? "server" :
                    lines.Any(l => l.IndexOf(serverSqlServer, StringComparison.OrdinalIgnoreCase) >= 0) ? "server" : "local";


                //Atribui os nomes e os tipos dos bancos
                filteredFiles[i] = (new Banco { Name = dbName, DbType = dbType, Instance = instanceType, FileName = filteredFiles[i].FileName });

                //nomeia a variavel "conexao" para referencia
                if (ConexaoFile != null && !string.IsNullOrEmpty(File.ReadAllText(ConexaoFile)))
                {
                    conexao = filteredFiles[i].Name.Equals(ToCapitalize(File.ReadLines($"{ConexaoFile}").FirstOrDefault().Remove(0, 12))) ? filteredFiles[i].Name : conexao;
                }
            }

            return new ObservableCollection<Banco>(filteredFiles.OrderBy(l => l.Name));
        }

        static void SelectBase(ObservableCollection<Banco> db)
        {
            foreach (var item in db)
            {
                if (!string.IsNullOrEmpty(item.Name) && item.Name.Equals(conexao))
                {
                    item.Name += " (Base Selecionada)";
                }
            }
        }

        static void UnselectBase(ObservableCollection<Banco> db)
        {
            foreach (var item in db)
            {
                if (item.Name.Contains("(Base Selecionada)"))
                {
                    item.Name = item.Name.Remove(item.Name.Length - 19);
                }
            }
        }

        static void DbToConexao(ObservableCollection<Banco> db, string opt)
        {
            //Verifica se estamos selecionando um banco já selecionado
            if (!opt.Contains("(Base Selecionada)"))
            {
                string dbText = File.ReadAllText($@"{DbDirectory}\{opt}.dat");

                File.WriteAllText($"{ConexaoFile}", dbText);

                conexao = opt;

                UnselectBase(db);

                SelectBase(db);
            }
            else
            {
                Console.WriteLine("Banco já Selecionado.");
            }
        }

        static Boolean IsThereConexaoDat()
        {
            if (conexao == "")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public void TrocarBase(object selectedItem)
        {
            if (selectedItem == null) return;

            string selectedBanco = selectedItem.ToString();

            if (IsThereConexaoDat())
            {
                DbToConexao(dbFiles, selectedBanco);
            }
            else
            {                
            }
        }

        public static string ToCapitalize(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        public ObservableCollection<Banco> InstanceFilter(string instance, ObservableCollection<Banco> db)
        {
            return new ObservableCollection<Banco>(db.Where(i => i.Instance.Equals(instance)));
        }

        public ObservableCollection<Banco> DbTypeFilter(string type, ObservableCollection<Banco> db)
        {
            return new ObservableCollection<Banco>(db.Where(i => i.DbType.Equals(type, StringComparison.OrdinalIgnoreCase)));
        }

        public void AdicionarDiretorio(string endereco, string enderecoCompleto)
        {
            // Verifica se já existe o diretório no histórico
            var existente = History.FirstOrDefault(d => d.Address == endereco);
            if (existente != null)
            {
                History.Remove(existente); // Remove para mover para o topo
            }

            // Adiciona no início da lista
            History.Insert(0, new SysDirectory(endereco, enderecoCompleto));

            // Garante que só existam no máximo 5
            while (History.Count > MaxHistory)
            {
                History.RemoveAt(History.Count - 1); // Remove o mais antigo (último)
            }
        }

        public void SetBaseAddress(string add)
        {
            DbDirectory = add;
        }

        public void SetConexaoAddress(string add)
        {
            ConexaoFile = add + @"\conexao.dat";
        }

        public void AtualizarDbFiles()
        {
            if (!string.IsNullOrEmpty(DbDirectory))
            {
                allFiles = new List<string>(Directory.GetFiles(DbDirectory, "*.dat"));
                dbFiles = FilterFiles(allFiles);
                init(dbFiles, allFiles);
            }
            else
            {
                dbFiles = new ObservableCollection<Banco>();
            }
        }

        public Boolean ValidateSystemPath(string path)
        {
            return File.Exists(path + "\\conexao.dat") ? true : false;
        }

        public void SalvarEstado()
        {
            List<SysDirectory> historyList = History.ToList();
            string HistoricoSerialized = JsonSerializer.Serialize(historyList);
            if (HistoricoSerialized != null && !string.IsNullOrEmpty(HistoricoSerialized))
            {
                Properties.Settings.Default.historico = HistoricoSerialized;
            }

            Properties.Settings.Default.dbDirectory = DbDirectory;
            Properties.Settings.Default.conexaoFile = ConexaoFile;
            Properties.Settings.Default.Conexao = conexao;
            Properties.Settings.Default.Save();
        }

        public void LoadState()
        {
            DbDirectory = Properties.Settings.Default.dbDirectory;
            ConexaoFile = Properties.Settings.Default.conexaoFile;
            conexao = Properties.Settings.Default.Conexao;

            string HistoricoSerialized = Properties.Settings.Default.historico;
            if (HistoricoSerialized != null && !string.IsNullOrEmpty(HistoricoSerialized))
            {
                History =
                JsonSerializer.Deserialize<ObservableCollection<SysDirectory>>(HistoricoSerialized)
                ?? new ObservableCollection<SysDirectory>();
            }
        }
    }
}
