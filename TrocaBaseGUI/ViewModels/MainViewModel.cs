using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TrocaBaseGUI.Models;

namespace TrocaBaseGUI.ViewModels
{
    //CRIAR BOTÃO PARA TELA DE CONFIGURAÇÃO
    //COLOCAR A SELEÇÃO DO DIRETÓRIO DAS BASES
    //COLOCAR A SELEÇÃO DO DIRETÓRIO DO SISTEMA
    internal class MainViewModel
    {
        const string DbDirectory = @"C:\Users\ygor\Desktop\TrocaBase";
        const string ConexaoFile = @"C:\Users\ygor\Desktop\TrocaBase\conexao.dat";
        static string conexao = "";

        static List<string> AllFiles = new List<string>(Directory.GetFiles(DbDirectory, "*.dat"));
        static List<string> exceptions = new List<string> { "Help", "iphist" };
        
        public ObservableCollection<Banco> dbFiles { get; set; }
        public MainViewModel()
        {
            dbFiles = FilterFiles(AllFiles, exceptions);

            init(dbFiles);

            //foreach (var file in dbFiles)
            //{
            //    Console.WriteLine("name: " + file.Name + " - inst: " + file.Instance);    
            //}

        }

        static void init(ObservableCollection<Banco> db)
        {
            conexao = AllFiles.Exists(f => f == ConexaoFile) ? ToCapitalize(File.ReadLines($@"{ConexaoFile}").First()) : "";
            SelectBase(db);
            
        }

        static void SelectBase(ObservableCollection<Banco> db)
        {
            foreach (var item in db)
            {
                if (item.Name == conexao)
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

        static ObservableCollection<Banco> FilterFiles(List<string> list, List<string> except)
        {
            ObservableCollection<Banco> filteredFiles = new ObservableCollection<Banco>();

            string oracleDb = "[BANCODADOS]=ORACLE";
            string sqlServerDb = "[BANCODADOS]=SQLSERVER";

            string serverOracle = "[DATABASE]=150.230.86.225";
            string serverSqlServer = "[DATABASE]=AZ-BD-AUTO-03";

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
                if (filteredFiles[i].FileName.Equals("conexao"))
                {
                    conexao = filteredFiles[i].FileName;
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

        //Define o banco que vai ser selecionado. Passa o .dat do banco para conexao.dat
        static void DbToConexao(ObservableCollection<Banco> db, string opt)
        {
            //Renomeia o banco escolhido(opt) para conexao.dat
            File.Move($@"{DbDirectory}\{opt}.dat", $"{ConexaoFile}");
            conexao = opt;
            SelectBase(db);
            Console.WriteLine($"\n{conexao}.dat is now conexao.dat(DbToConexao())");
        }

        //Retorna o conexao.dat para o nome do banco
        static void ConexaoToDb(ObservableCollection<Banco> db, string opt)
        {
            Console.WriteLine("Conexao.dat Found! :)");

            //Verifica se estamos selecionando um banco já selecionado
            if (!opt.Contains("(Base Selecionada)"))
            {
                //Lê o nome do banco na primeira linha do conexao.dat
                string dbName = File.ReadLines($@"{ConexaoFile}").First();

                //Renomeia o conexao.dat para o nome do banco de dbName
                File.Move($"{ConexaoFile}", $@"{DbDirectory}\{dbName}.dat");

                UnselectBase(db);

                DbToConexao(db, opt);

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
                ConexaoToDb(dbFiles, selectedBanco);
            }
            else
            {
                DbToConexao(dbFiles, selectedBanco);
                
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
    }
}
