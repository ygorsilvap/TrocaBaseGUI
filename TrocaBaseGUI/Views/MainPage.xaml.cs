using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Threading;

namespace TrocaBaseGUI.Views
{
    public partial class MainPage : Page
    {
        //public ObservableCollection<Banco> listaBancos { get; set; }
        public ObservableCollection<DatabaseModel> listaBancos { get; set; }
        public ObservableCollection<SysDirectory> hist { get; set; }
        private MainViewModel viewModel;
        public int tabSelected;
        public string rbSelected;
        public string sysSelected;

        public MainPage()
        {
            InitializeComponent();

            viewModel = new MainViewModel();
            this.DataContext = viewModel;

            hist = new ObservableCollection<SysDirectory>(viewModel.History);
            //listaBancos = new ObservableCollection<Banco>(viewModel.dbFiles ?? new ObservableCollection<Banco>());
            listaBancos = new ObservableCollection<DatabaseModel>(viewModel.Databases ?? new ObservableCollection<DatabaseModel>());
            lstTodosBancos.ItemsSource = listaBancos;

            RadioButton_Checked(rbTodos, null);
            tabSelected = TabControl.SelectedIndex;
            dirSys.SelectedValue = hist.Count > 0 ? hist.First().Address : "";
            CloseNSysButton.Content = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Fechar e iniciar sistema" : $"Fechar e iniciar \n{MainViewModel.ToCapitalize(MainViewModel.exeFile)}";
            //IsThereDbDirectory.Text = string.IsNullOrWhiteSpace(MainViewModel.DbDirectory) ? "Nenhuma base encontrada.\nSelecione um diretório." : "";
            IsThereSysDirectory.Text = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Nenhum executável encontrado.\nSelecione um executável." : "";

            //Console.WriteLine(hist.Count());

            GetFilter(listaBancos);
        }
        private void GetFilter(ObservableCollection<DatabaseModel> db)
        {
            if (!(DataContext is MainViewModel vm)) return;

            string instance = tabSelected == 0 ? "local" : "server";
            string type = null;

            if (rbOracle.IsChecked == true)
            {
                type = "Oracle";
            }
            else if (rbSqlServer.IsChecked == true)
            {
                type = "SQLServer";
            }
            else
            {
                lstTodosBancos.ItemsSource = vm.InstanceFilter(instance, db);
                return;
            }

            ObservableCollection<DatabaseModel> bases = vm.InstanceFilter(instance, db);

            lstTodosBancos.ItemsSource = vm.DbTypeFilter(type, bases);
        }

        private void Refresh()
        {
            //IsThereDbDirectory.Text = string.IsNullOrWhiteSpace(MainViewModel.DbDirectory) ? "Nenhuma base encontrada.\nSelecione um diretório." : "";
            IsThereSysDirectory.Text = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Nenhum executável encontrado.\nSelecione um executável." : "";

            //viewModel.AtualizarDbFiles();

            listaBancos = new ObservableCollection<DatabaseModel>(viewModel.Databases ?? new ObservableCollection<DatabaseModel>());
            lstTodosBancos.ItemsSource = listaBancos;

            RadioButton_Checked(rbTodos, null);
            tabSelected = TabControl.SelectedIndex;

            if (!string.IsNullOrEmpty(MainViewModel.ConexaoFile))
            {
                conexaoCheck.Text = string.IsNullOrEmpty(File.ReadAllText(MainViewModel.ConexaoFile)) ||
                    !File.ReadAllText(MainViewModel.ConexaoFile).Contains("[NOMEBANCO]") ? "Nenhuma base selecionada." : "";
            }
            else
            {
                conexaoCheck.Text = "";
            }
            //dirBase.Text = string.IsNullOrEmpty(System.IO.Path.GetFileName(MainViewModel.DbDirectory)) ? "" : $"...\\{System.IO.Path.GetFileName(MainViewModel.DbDirectory)}";
            CloseNSysButton.Content = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Fechar e iniciar sistema" : $"Fechar e iniciar \n{MainViewModel.ToCapitalize(MainViewModel.exeFile)}";

            GetFilter(listaBancos);
        }

        private void TrocarBase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm && !String.IsNullOrEmpty(MainViewModel.exeFile))
            {
                vm.SelectBase(vm.Databases, lstTodosBancos.SelectedItem.ToString());
            } else
            {
                MessageBox.Show("Nenhum executável encontrado.\nSelecione um executável.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Refresh();
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

        private void SelecionarExecutavel_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Selecione o executável do sistema.",
                InitialDirectory = @"C:\",
                Filters = { new CommonFileDialogFilter("Executáveis", "*.exe") }
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok && File.Exists(dialog.FileName))
            {
                viewModel.AddDirectory($"\\{System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(dialog.FileName))}",
                    System.IO.Path.GetDirectoryName(dialog.FileName), System.IO.Path.GetFileNameWithoutExtension(dialog.FileName));
                viewModel.SetConexaoAddress(System.IO.Path.GetDirectoryName(dialog.FileName));

                dirSys.SelectedItem = viewModel.History.FirstOrDefault();

                Refresh();
            }
        }

        //private void SelecionarDiretorioBase_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new CommonOpenFileDialog
        //    {
        //        IsFolderPicker = true,
        //        InitialDirectory = @"C:\",
        //        Title = "Selecione o diretório das bases."
        //    };

        //    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        //    {
        //        // Atualiza o DbDirectory no ViewModel
        //        viewModel.SetBaseAddress(dialog.FileName);

        //        // Altera o texto do TextBlock diretamente no código-behind
        //        dirBase.Text = $"...\\{System.IO.Path.GetFileName(dialog.FileName)}";

        //        Refresh();
        //    }
        //}

        private void dirSys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as SysDirectory;

            if (selectedItem != null)
            {
                viewModel.SetConexaoAddress(selectedItem.FullPathAddress);
                MainViewModel.exeFile = selectedItem.ExeFile;
            }
            Refresh();
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            var del = MessageBox.Show("Isso fará com que todos os dados da aplicação sejam deletados.\n\nDesejar continuar?", "Hard Reset",
                MessageBoxButton.YesNo, MessageBoxImage.Warning)
                .ToString().ToLower();

            if (del.Equals("yes"))
            {
                viewModel.ClearApp();
                Refresh();
            }
        }

        private void CloseNSysButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start($@"{System.IO.Path.GetDirectoryName(MainViewModel.ConexaoFile)}\{MainViewModel.exeFile}.exe");
            Application.Current.Shutdown();
        }

        private void ToSettings_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFramePublic.Navigate(new SettingsPage());
        }
    }
}
