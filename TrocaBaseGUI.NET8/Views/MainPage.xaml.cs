using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.Services;
using TrocaBaseGUI.Utils;
using TrocaBaseGUI.ViewModels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace TrocaBaseGUI.Views
{
    public partial class MainPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<DatabaseModel> listaBancos { get; set; }
        public ObservableCollection<SysDirectory> hist { get; set; }
        private MainViewModel viewModel;
        public int tabSelected;
        public string rbSelected;
        public string sysSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage(MainViewModel vm)
        {
            InitializeComponent();

            viewModel = vm;
            this.DataContext = viewModel;

            hist = new ObservableCollection<SysDirectory>(viewModel.History);

            listaBancos = new ObservableCollection<DatabaseModel>(viewModel.Databases ?? new ObservableCollection<DatabaseModel>());
            lstTodosBancos.ItemsSource = listaBancos;

            //foreach (var item in viewModel.Databases) DatabaseModel.SetDisplayName(item, item.DisplayName);
            foreach (var item in listaBancos) DatabaseModel.SetDisplayName(item, item.DisplayName);


            RadioButton_Checked(rbTodos, null);
            tabSelected = TabControl.SelectedIndex;
            dirSys.SelectedValue = hist.Count > 0 ? hist.First().Address : "";

            OpenSysButton.Content = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Iniciar sistema" : $"Iniciar \n{StringUtils.ToCapitalize(MainViewModel.exeFile)}";
            IsThereSysDirectory.Text = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Nenhum executável encontrado.\nSelecione um executável." : "";
            GetFilter(listaBancos);

            //foreach (var item in viewModel.Databases)
            //{
            //    Debug.WriteLine($"\n Id: {item.Id}, Database: {item.Name}, Type: {item.DbType}, Environment: {item.Environment}, Server: {item.Server}\n");
            //}
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Local
            //await viewModel.openSqlConn(viewModel.SqlService, viewModel.LocalSQLServerConnection.Server);
            await viewModel.openSqlConn(viewModel.SqlService, viewModel.LocalSQLServerConnection);
            //await viewModel.openOracleConn(viewModel.OracleService, viewModel.LocalOracleConnection.Server, viewModel.LocalOracleConnection.Password, viewModel.LocalOracleConnection.Port, viewModel.LocalOracleConnection.Environment);
            await viewModel.openOracleConn(viewModel.OracleService, viewModel.LocalOracleConnection);
            //Debug.WriteLine($"\n\nOKOKOKOKOKOK\n\n");

            //Server
            //await viewModel.openSqlConn(viewModel.SqlService, viewModel.ServerSQLServerConnection.Server, viewModel.ServerSQLServerConnection.Username, viewModel.ServerSQLServerConnection.Password);
            //await viewModel.openOracleConn(viewModel.OracleService, viewModel.ServerOracleConnection.Server, viewModel.ServerOracleConnection.Password, viewModel.ServerOracleConnection.Port, viewModel.ServerOracleConnection.Environment, viewModel.ServerOracleConnection.Instance);
            //await viewModel.openOracleConn(viewModel.OracleService, viewModel.ServerOracleConnection);

            listaBancos.Clear();

            foreach (var db in viewModel.Databases)
            {
                DatabaseModel.SetDisplayName(db, db.DisplayName);
                listaBancos.Add(db);
            }

            //DatabaseModel.SetSelection(listaBancos,viewModel.SelDatabase.Id);
            //DatabaseModel.SetSelection(listaBancos, viewModel.History.FirstOrDefault(i => i.SelectedBase >= 0).SelectedBase);


            GetFilter(listaBancos);
        }

        private void GetFilter(ObservableCollection<DatabaseModel> db)
        {
            if (!(DataContext is MainViewModel vm)) return;

            string environment = tabSelected == 0 ? "local" : "server";
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
                lstTodosBancos.ItemsSource = vm.EnvironmentFilter(environment, db);
                return;
            }

            ObservableCollection<DatabaseModel> bases = vm.EnvironmentFilter(environment, db);

            lstTodosBancos.ItemsSource = vm.DbTypeFilter(type, bases);
        }

        public void Refresh()
        {
            IsThereSysDirectory.Text = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Nenhum executável encontrado.\nSelecione um executável." : "";

            //viewModel.AtualizarDbFiles();

            listaBancos = new ObservableCollection<DatabaseModel>(viewModel.Databases ?? new ObservableCollection<DatabaseModel>());
            lstTodosBancos.ItemsSource = listaBancos;

            RadioButton_Checked(rbTodos, null);
            tabSelected = TabControl.SelectedIndex;
            OpenSysButton.Content = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Iniciar sistema" : $"Iniciar \n{StringUtils.ToCapitalize(MainViewModel.exeFile)}";

            GetFilter(listaBancos);
        }

        private void TrocarBase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm && !String.IsNullOrEmpty(MainViewModel.exeFile))
            {
                vm.SelectBase(vm.Databases, viewModel.SelDatabase.Id, dirSys.SelectedValue.ToString());
            } else
            {
                MessageBox.Show("Nenhum executável encontrado.\nSelecione um executável.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Refresh();
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SaveState();
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

                viewModel.conexaoFileService.SetConexaoAddress(System.IO.Path.GetDirectoryName(dialog.FileName));

                dirSys.SelectedItem = viewModel.History.FirstOrDefault();

                Refresh();
            }
            //Debug.WriteLine($"\n\n\nConFile: {viewModel.conexaoFileService.ConexaoAddress}\n\n\n");
        }

        private void dirSys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as SysDirectory;

            string selectedDir = comboBox.SelectedValue as string;

            int selectedBaseDir = SysDirectory.GetDir(viewModel.History, selectedDir).SelectedBase;

            if (selectedItem != null)
            {
                viewModel.conexaoFileService.SetConexaoAddress(selectedItem.FullPathAddress);
                MainViewModel.exeFile = selectedItem.ExeFile;

                if (listaBancos.Count() > 0 && viewModel.History.Any(i => i.SelectedBase >= 0))
                {
                    DatabaseModel.SetSelection(listaBancos, selectedBaseDir);
                    viewModel.SelDatabase = listaBancos[selectedBaseDir];
                }
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

        private void OpenSysButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start($@"{System.IO.Path.GetDirectoryName(viewModel.conexaoFile)}\{MainViewModel.exeFile}.exe");
            //Application.Current.Shutdown();
        }

        private void ToSettings_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFramePublic.Navigate(new LocalSettingsPage());
        }

        private void EditarBase_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;

            if (menuItem?.DataContext is DatabaseModel db)
            {
                ((MainWindow)Application.Current.MainWindow).MainFramePublic.Navigate(new EditDbPage(viewModel, db));
            }
        }

        private void CopyStringClick_Click(object sender, RoutedEventArgs e)
        {
            string connString = viewModel.SelDatabase.DbType.ToLower().StartsWith("s")
                ? viewModel.SqlService.CreateSQLServerConnectionString(viewModel.SelDatabase.Environment, viewModel.SelDatabase.Name, viewModel.SelDatabase.Server)
                : viewModel.OracleService.CreateOracleConnectionString(viewModel.SelDatabase.Environment, viewModel.SelDatabase.Server, viewModel.SelDatabase.Instance, viewModel.SelDatabase.Name);

            Clipboard.SetText(connString);
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
