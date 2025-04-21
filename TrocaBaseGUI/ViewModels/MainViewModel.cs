using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TrocaBaseGUI.Models;

namespace TrocaBaseGUI.ViewModels
{
    //LINHA 151
    internal class MainViewModel
    {
        static string DbDirectory;
        static string ConexaoFile;// = @"C:\Users\ygor\Desktop\TrocaBase\conexao.dat";
        static string conexao = "";

        static List<string> allFiles;// = new List<string>(Directory.GetFiles(DbDirectory, "*.dat"));
        static List<string> exceptions = new List<string> { "Help", "iphist", "conexao" };


        private const int MaxHistory = 5;
        public ObservableCollection<SysDirectory> History { get; } = new ObservableCollection<SysDirectory>();
        public ObservableCollection<Banco> dbFiles { get; set; }
        public MainViewModel()
        {
            if ((!string.IsNullOrWhiteSpace(DbDirectory) && Directory.Exists(DbDirectory)) && (!string.IsNullOrWhiteSpace(ConexaoFile) && Directory.Exists(ConexaoFile)))
            {
                allFiles = new List<string>(Directory.GetFiles(DbDirectory, "*.dat"));

                dbFiles = FilterFiles(allFiles, exceptions);
                init(dbFiles, allFiles);
                Console.WriteLine(ConexaoFile);
            }
        }

        static void init(ObservableCollection<Banco> db, List<string> allFiles)
        {
            if (!string.IsNullOrWhiteSpace(ConexaoFile) && File.Exists(ConexaoFile))
            {
                var linha = File.ReadLines(ConexaoFile).FirstOrDefault();
                conexao = linha != null ? ToCapitalize(linha) : "";
            }
            else
            {
                conexao = "";
            }

            SelectBase(db);
        }

        static void SelectBase(ObservableCollection<Banco> db)
        {
            foreach (var item in db)
            {
                if (item.Name.Equals(conexao))
                {
                    item.Name += " (Base Selecionada)";
                    Console.WriteLine(item.Name);
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

        static ObservableCollection<Banco> FilterFiles(List<string> list, List<string> except)
        {
            ObservableCollection<Banco> filteredFiles = new ObservableCollection<Banco>();

            string oracleDb = "[BANCODADOS]=ORACLE";
            string sqlServerDb = "[BANCODADOS]=SQLSERVER";

            string serverOracle = "[DATABASE]=150.230.86.225";
            string serverSqlServer = "[DATABASE]=AZ-BD-AUTO-03";

            if (string.IsNullOrEmpty(DbDirectory) || string.IsNullOrEmpty(ConexaoFile))
            {
                return new ObservableCollection<Banco>();
            }

            foreach (string file in list)
            {
                filteredFiles.Add(new Banco { FileName = Path.GetFileName(file.Replace(".dat", "")) });
            };

            //filtra os .dat que são db
            for (int i = 0; i < filteredFiles.Count; i++)
            {
                for (int j = 0; j < except.Count; j++)
                {
                    if (filteredFiles[i].FileName.Equals(except[j]))
                    {
                        filteredFiles.RemoveAt(i);
                    }
                }
            }
           


            //nomeia a variavel "conexao" para referencia
            for (int i = 0; i < filteredFiles.Count; i++)
            {
                if (ConexaoFile != null && filteredFiles[i].FileName.Equals(File.ReadLines($"{ConexaoFile}").First()))
                {
                    conexao = filteredFiles[i].Name;
                }

                //Inicial do nome do banco maiúscula
                string dbName = ToCapitalize(File.ReadLines($@"{DbDirectory}\{filteredFiles[i].FileName}.dat").First());

                //IMPLEMENTAR A IDENTIFICAÇÃO DO TIPO DE DB
                string dbType =
                    File.ReadLines($@"{DbDirectory}\{filteredFiles[i].FileName}.dat").Any(l => l.Contains(oracleDb)) ? "Oracle" :
                    File.ReadLines($@"{DbDirectory}\{filteredFiles[i].FileName}.dat").Any(l => l.Contains(sqlServerDb)) ? "SQLServer" : "Arquivo Inválido";
                string instanceType =
                    File.ReadLines($@"{DbDirectory}\{filteredFiles[i].FileName}.dat").Any(i => i.Contains(serverOracle)) ? "server" :
                    File.ReadLines($@"{DbDirectory}\{filteredFiles[i].FileName}.dat").Any(i => i.Contains(serverSqlServer)) ? "server" : "local";

                //Atribui os nomes e os tipos dos bancos
                filteredFiles[i] = (new Banco { Name = dbName, DbType = dbType, Instance = instanceType, FileName = filteredFiles[i].FileName });
            }

            return new ObservableCollection<Banco>(filteredFiles.OrderBy(l => l.Name));
        }

        //Retorna o conexao.dat para o nome do banco
        static void DbToConexao(ObservableCollection<Banco> db, string opt)
        {
            //Verifica se estamos selecionando um banco já selecionado
            if (!opt.Contains("(Base Selecionada)"))
            {
                string dbText = File.ReadAllText($@"{DbDirectory}\{opt}.dat");

                File.WriteAllText($"{ConexaoFile}", dbText);

                UnselectBase(db);

                SelectBase(db);

                //////////CORRIGIR A MUDANÇA DE NOME DE BASE SELECIONADA
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

        public void AdicionarDiretorio(string endereco)
        {
            // Verifica se já existe o diretório no histórico
            var existente = History.FirstOrDefault(d => d.Address == endereco);
            if (existente != null)
            {
                History.Remove(existente); // Remove para mover para o topo
            }

            // Adiciona no início da lista
            History.Insert(0, new SysDirectory(endereco));

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
                dbFiles = FilterFiles(allFiles, exceptions);
                init(dbFiles, allFiles);
            }
            else
            {
                dbFiles = new ObservableCollection<Banco>();
            }
        }
    }
}
