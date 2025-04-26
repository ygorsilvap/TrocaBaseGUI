using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace TrocaBaseGUI
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Banco> listaBancos { get; set; }
        public ObservableCollection<SysDirectory> hist { get; set; }
        private MainViewModel viewModel;
        public int tabSelected;
        public string rbSelected;
        public string sysSelected;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel();
            this.DataContext = viewModel;

            hist = viewModel.History;
            listaBancos = new ObservableCollection<Banco>(viewModel.dbFiles ?? new ObservableCollection<Banco>());
            lstTodosBancos.ItemsSource = listaBancos;

            RadioButton_Checked(rbTodos, null);
            tabSelected = TabControl.SelectedIndex;
            dirSys.SelectedValue = hist.First().Address;

            GetFilter(listaBancos);
        }
        private void GetFilter(ObservableCollection<Banco> db)
        {
            if (!(DataContext is MainViewModel vm)) return;

            string instance = tabSelected == 0 ? "local" : "server";
            string type = null;

            if(rbOracle.IsChecked == true)
            {
                type = "Oracle";
            }
            else if(rbSqlServer.IsChecked == true)
            {
                type = "SQLServer";
            }
            else
            {
                lstTodosBancos.ItemsSource = vm.InstanceFilter(instance, db);
                return;
            }

            ObservableCollection<Banco> bases = vm.InstanceFilter(instance, db);

            lstTodosBancos.ItemsSource = vm.DbTypeFilter(type, bases);
        }

        private void Refresh()
        {
            viewModel.AtualizarDbFiles();

            listaBancos = new ObservableCollection<Banco>(viewModel.dbFiles ?? new ObservableCollection<Banco>());
            lstTodosBancos.ItemsSource = listaBancos;

            RadioButton_Checked(rbTodos, null);
            tabSelected = TabControl.SelectedIndex;
            conexaoCheck.Text = string.IsNullOrEmpty(File.ReadAllText(MainViewModel.ConexaoFile)) ? "conexao.dat vazio" : "";
            dirBase.Text = $"...\\{System.IO.Path.GetFileName(MainViewModel.DbDirectory)}";
            GetFilter(listaBancos);
        }

        private void TrocarBase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.TrocarBase(lstTodosBancos.SelectedItem);
            }
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
             GetFilter(listaBancos);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
                tabSelected = tabControl.SelectedIndex;

            GetFilter(listaBancos);
        }

        private void SelecionarDiretorio_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = @"C:\",
                Title = "Selecione o diretório do LinxDMS/Bravos."
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok && viewModel.ValidateSystemPath(dialog.FileName))
            {
                viewModel.AdicionarDiretorio($"\\{System.IO.Path.GetFileName(dialog.FileName)}", dialog.FileName);
                viewModel.SetConexaoAddress(dialog.FileName);

                dirSys.SelectedItem = viewModel.History.FirstOrDefault();

                Refresh();
            } else
            {
                MessageBox.Show("Diretório inválido.\nconexao.dat não encontrado.", "Seleção de Diretório Inválida", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelecionarDiretorioBase_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = @"C:\",
                Title = "Selecione o diretório das bases."
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                // Atualiza o DbDirectory no ViewModel
                viewModel.SetBaseAddress(dialog.FileName);

                // Altera o texto do TextBlock diretamente no código-behind
                dirBase.Text = $"...\\{System.IO.Path.GetFileName(dialog.FileName)}";

                Refresh();
            }
        }

        private void dirSys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as SysDirectory;

            viewModel.SetConexaoAddress(selectedItem.FullPathAddress);

            viewModel.AtualizarDbFiles();

            Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewModel.SalvarEstado(); // Salva os dados antes de fechar
        }
    }
}