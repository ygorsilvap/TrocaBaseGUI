using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace TrocaBaseGUI
{
    public partial class MainWindow : Window
    {
        const string DbDirectory = @"C:\2Camadas-Bravos0518-Evolutivo";
        const string ConexaoFile = @"C:\2Camadas-Bravos0518-Evolutivo\conexao.dat";
        static string conexao = "";

        static List<string> AllFiles = new List<string>(Directory.GetFiles(DbDirectory, "*.dat"));
        static List<string> exceptions = new List<string> { "Help", "iphist" };

        static ObservableCollection<Banco> dbFiles = new ObservableCollection<Banco>(FilterFiles(AllFiles, exceptions).Select(name => new Banco { Name = name.ToString() }).ToList());
        
        public MainWindow()
        {
            init(dbFiles);

            InitializeComponent();

            lstTodosBancos.ItemsSource = dbFiles;

        }

        static void init(ObservableCollection<Banco> db)
        {
            conexao = AllFiles.Exists(f => f == ConexaoFile) ? File.ReadLines($@"{ConexaoFile}").First() : "";

            Console.WriteLine(conexao);

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
                if(item.Name.Contains("(Banco Selecionado)"))
                {
                    item.Name = item.Name.Remove(item.Name.Length - 20);
                }
            }
        }

        static List<string> FilterFiles(List<string> list, List<string> except)
        {
            List<string> files = new List<string>(list);
            List<string> filteredFilesNames = new List<string>();
            List<string> exceptions = new List<string>(except);

            foreach (string file in files)
            {
                filteredFilesNames.Add(Path.GetFileName(file.Replace(".dat", "")));
            };

            for (int i = 0; i < filteredFilesNames.Count; i++)
            {
                for (int j = 0; j < exceptions.Count; j++)
                {
                    if (filteredFilesNames[i].Equals(exceptions[j]))
                    {
                        filteredFilesNames.RemoveAt(i);
                    }
                }
            }

            for (int i = 0; i < filteredFilesNames.Count; i++)
            {
                if (filteredFilesNames[i].Equals("conexao"))
                {
                    conexao = filteredFilesNames[i];
                }

                string dbName = File.ReadLines($@"{DbDirectory}\{filteredFilesNames[i]}.dat").First();
                filteredFilesNames[i] = dbName;
            }


            return filteredFilesNames;
        }

        static void DbToConexao(ObservableCollection<Banco> db, string opt)
        {
            //Renomeia o banco escolhido(opt) para conexao.dat
            File.Move($@"{DbDirectory}\{opt}.dat", $"{ConexaoFile}");
            conexao = opt;
            SelectBase(db);
            Console.WriteLine($"\n{conexao}.dat is now conexao.dat(DbToConexao())");
        }

        static void ConexaoToDb(ObservableCollection<Banco> db, string opt)
        {
            Console.WriteLine("Conexao.dat Found! :)");
            

            //Verifica se estamos selecionando um banco já selecionado
            if(!opt.Contains("(Banco Selecionado)"))
            {
                //Lê o nome do banco na primeira linha do conexao.dat
                string dbName = File.ReadLines($@"{ConexaoFile}").First();

                //Renomeia o conexao.dat para o nome do banco de dbName
                File.Move($"{ConexaoFile}", $@"{DbDirectory}\{dbName}.dat");

                UnselectBase(db);

                DbToConexao(db, opt);

                SelectBase(db);
            } else
            {
                Console.WriteLine("Banco já Selecionado.");
            }
        }

        static Boolean IsThereConexaoDat()
        {
            if(conexao == "")
            {
                return false;
            } else
            {
                return true;
            }

        }

        private void TrocarBase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (IsThereConexaoDat())
            {
                ConexaoToDb(dbFiles, lstTodosBancos.SelectedItem.ToString());   
            }
            else
            {
                DbToConexao(dbFiles, lstTodosBancos.SelectedItem.ToString());
            }

        }
    }

    public class Banco : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString()
        {
            return Name; // Para mostrar o nome no ListBox diretamente
        }
    }
}
