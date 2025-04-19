using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using TrocaBaseGUI.Models;

namespace TrocaBaseGUI.ViewModels
{
    internal class MainViewModel
    {
        const string DbDirectory = @"C:\Users\ygor\Desktop\TrocaBase";
        const string ConexaoFile = @"C:\Users\ygor\Desktop\TrocaBase\conexao.dat";
        static string conexao = "";

        static List<string> AllFiles = new List<string>(Directory.GetFiles(DbDirectory, "*.dat"));
        static List<string> exceptions = new List<string> { "Help", "iphist" };
        
        public ObservableCollection<Banco> dbFiles { get; set; }
        public ObservableCollection<Banco> oracleFiles { get; set; }
        public ObservableCollection<Banco> sqlServerFiles { get; set; }

        public MainViewModel()
        {
            dbFiles = FilterFiles(AllFiles, exceptions);

            init(dbFiles);
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
                    item.Name += " (Banco Selecionado)";
                }
            }
        }

        static void UnselectBase(ObservableCollection<Banco> db)
        {
            foreach (var item in db)
            {
                if (item.Name.Contains("(Banco Selecionado)"))
                {
                    item.Name = item.Name.Remove(item.Name.Length - 20);
                }
            }
        }

        static ObservableCollection<Banco> FilterFiles(List<string> list, List<string> except)
        {

            ObservableCollection<Banco> filteredFiles = new ObservableCollection<Banco>();

            string oracleDb = "[BANCODADOS]=ORACLE";
            string sqlServerDb = "[BANCODADOS]=SQLSERVER";

            foreach (string file in list)
            {
                filteredFiles.Add(new Banco { Name = Path.GetFileName(file.Replace(".dat", "")) });
            };

            //filtra os .dat que são db
            for (int i = 0; i < filteredFiles.Count; i++)
            {
                for (int j = 0; j < except.Count; j++)
                {
                    if (filteredFiles[i].Name.Equals(except[j]))
                    {
                        filteredFiles.RemoveAt(i);
                    }
                }
            }

            //nomeia a variavel "conexao" para referencia
            for (int i = 0; i < filteredFiles.Count; i++)
            {
                if (filteredFiles[i].Name.Equals("conexao"))
                {
                    conexao = filteredFiles[i].Name;
                }

                //Inicial do nome do banco maiúscula
                string dbName = ToCapitalize(File.ReadLines($@"{DbDirectory}\{filteredFiles[i].Name}.dat").First());

                //IMPLEMENTAR A IDENTIFICAÇÃO DO TIPO DE DB
                string dbType =
                    File.ReadLines($@"{DbDirectory}\{filteredFiles[i].Name}.dat").Any(l => l.Contains(oracleDb)) ? "Oracle" :
                    File.ReadLines($@"{DbDirectory}\{filteredFiles[i].Name}.dat").Any(l => l.Contains(sqlServerDb)) ? "SQLServer" : "Arquivo Inválido";

                //Atribui os nomes e os tipos dos bancos
                filteredFiles[i] = (new Banco { Name = dbName, DbType = dbType });
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
            if (!opt.Contains("(Banco Selecionado)"))
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
    }
}
