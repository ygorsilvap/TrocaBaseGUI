using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.Services;
using TrocaBaseGUI.Utils;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class MainPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<DatabaseModel> databaseList { get; set; } = new ObservableCollection<DatabaseModel>();
        //public ObservableCollection<SysDirectoryModel> sysDirectoryList { get; set; }
        private MainViewModel viewModel;
        public int tabSelected;
        public string rbSelected;
        public string sysSelected;
        public string mainExe;
        public string secondaryExe;
        public int selectedDatabaseId;
        public SysDirectoryModel selectedSysDirectory;
        public ObservableCollection<string> exesList { get; set; } = new ObservableCollection<string>();
        //public string dbSearch;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage(MainViewModel vm)
        {
            InitializeComponent();

            viewModel = vm;
            this.DataContext = viewModel;

            //sysDirectoryList = new ObservableCollection<SysDirectoryModel>(viewModel.SysDirectoryList);

            RadioButton_Checked(rbTodos, null);
            tabSelected = TabControl.SelectedIndex;
            //dirSys.SelectedValue = hist.Count > 0 ? hist.First().Folder : "";

            //Fazer Binding com esses campos de exe
            //OpenMainExeButtonText.Text = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Selecione um executável" : $"Iniciar \n{StringUtils.ToCapitalize(mainExe)}";
            //OpenSecondaryExeButtonText.Text = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Selecione um executável" : $"Iniciar \n{StringUtils.ToCapitalize(MainViewModel.exeFile)}";

            //IsThereSysDirectory.Text = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Nenhum executável encontrado.\nSelecione um diretório." : "";
            GetFilter(databaseList);

            //foreach (var item in viewModel.Databases)
            //{
            //    Debug.WriteLine($"\n Id: {item.Id}, Database: {item.Name}, Type: {item.DbType}, Environment: {item.Environment}, Server: {item.Server}, Date: {item.ImportDate}\n");
            //}
            //Debug.WriteLine($"\n\nMPGloginPadrao: {viewModel.appState.LocalParams.DefaultLoginCheckbox}\n\n");
            //Debug.WriteLine($"\n\n{viewModel.Databases.LastOrDefault()}\n\n");
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            searchPlaceholder();

            viewModel.sysDirectoryService.UpdateSysDirectoriesFiles(viewModel.SysDirectoryList);

            SetExesSelection();

            SetSelectedDatabase(selectedDatabaseId);

            if (viewModel.Databases == null || viewModel.Databases.Count == 0)
            {
                var tasks = new List<Task>
                {
                    //Local
                    viewModel.openSqlConn(viewModel.SqlService, viewModel.LocalSQLServerConnection),
                    viewModel.openOracleConn(viewModel.OracleService, viewModel.LocalOracleConnection),
                    //Server
                    viewModel.openSqlConn(viewModel.SqlService, viewModel.ServerSQLServerConnection),
                    viewModel.openOracleConn(viewModel.OracleService, viewModel.ServerOracleConnection)
                };

                await Task.WhenAll(tasks);
            }

            databaseList.Clear();

            foreach (var db in viewModel.Databases)
            {
                DatabaseService.SetDisplayName(db, db.DisplayName);
                databaseList.Add(db);
            }

            viewModel.DbService.SortDatabases(databaseList);

            GetFilter(databaseList);
        }

        private void GetFilter(ObservableCollection<DatabaseModel> db)
        {
            if (!(DataContext is MainViewModel vm)) return;

            string environment = tabSelected == 0 ? "local" : "server";

            string type = string.Empty;

            if (rbOracle.IsChecked == true)
            {
                type = "Oracle";
            }
            else if (rbSqlServer.IsChecked == true)
            {
                type = "SQL Server";
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
            //IsThereSysDirectory.Text = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Nenhum executável encontrado.\nSelecione um executável." : "";

            //dbList = new ObservableCollection<DatabaseModel>(viewModel.Databases ?? new ObservableCollection<DatabaseModel>());

            ////Arrumar essa gambiarra sem vergonha
            //if(String.IsNullOrEmpty(dbSearch.Text))
            //{
            //    dbSearch.Text = dbSearch.Text;
            //} else
            //{
            //    lstTodosBancos.ItemsSource = dbList;
            //}

            //RadioButton_Checked(rbTodos, null);
            //tabSelected = TabControl.SelectedIndex;
            //OpenSysButtonText.Text = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Iniciar sistema" : $"Iniciar \n{StringUtils.ToCapitalize(MainViewModel.exeFile)}";

            //GetFilter(dbList);
        }

        public void SetExesSelection()
        {
            string secondaryExe = exesList.FirstOrDefault(exe => exe.StartsWith("frentecaixa", StringComparison.OrdinalIgnoreCase)).ToLower();

            secondaryExe = !string.IsNullOrEmpty(secondaryExe) && secondaryExe.Contains("client", StringComparison.OrdinalIgnoreCase) ? 
                secondaryExe.Replace("client", "") : secondaryExe;

            string mainExecutable = !string.IsNullOrEmpty(mainExe) && mainExe.EndsWith("client", StringComparison.OrdinalIgnoreCase) ? mainExe.Replace("client", "") : mainExe;

            OpenMainExeButtonText.Text = string.IsNullOrWhiteSpace(mainExecutable) ?
                "Selecione um executável" : $"Iniciar \n{StringUtils.ToCapitalize(mainExecutable)}";

            OpenSecondaryExeButtonText.Text = exesList.Count <= 0 || string.IsNullOrEmpty(secondaryExe) ?
                "Selecione um executável" : $"Iniciar \n{StringUtils.ToCapitalize(secondaryExe)}";
        }

        public void SetSelectedDatabase(int database)
        {
            viewModel.SelectedDatabase = database > -1 ?
                viewModel.Databases.FirstOrDefault(db => db.Id.Equals(database)) : new DatabaseModel();
        }

        private void TrocarBase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!String.IsNullOrEmpty(MainViewModel.exeFile))
            {
                Debug.WriteLine($"\n\nIDSelDb: {viewModel.SelectedDatabase.Id}\n\n");
                //viewModel.SelectBase(viewModel.Databases, viewModel.SelectedDatabase.Id, SysDirectory.SelectedValue.ToString());
                viewModel.SelectBase(viewModel.Databases, viewModel.SelectedDatabase.Id, selectedSysDirectory);
            }
            else
            {
                MessageBox.Show("Nenhum executável encontrado.\nSelecione um executável.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            GetFilter(databaseList);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
                tabSelected = tabControl.SelectedIndex;

            GetFilter(databaseList);
        }

        private void MenuDiretorios_Click(object sender, RoutedEventArgs e)
        {
            var owner = Window.GetWindow(this) ?? Application.Current.MainWindow;

            var dlg = new SysDirectorySelectionWindow()
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dlg.ShowDialog();
        }

        //Refatorar
        private void SysDirectory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as SysDirectoryModel;

            viewModel.appState.SelectedFolder = selectedItem == null ? 
                viewModel.SysDirectoryList.LastOrDefault() : selectedItem;

            if(selectedItem == null) {
                mainExe = string.Empty;
                ExesList.ItemsSource = new ObservableCollection<string>();
                selectedDatabaseId = -1;
                return;
            }

            exesList = selectedItem.ExeList;
            ExesList.ItemsSource = exesList;

            mainExe = selectedItem.MainExeFile;
            SetExesSelection();  

            //Revisar necessidade dessa variável
            selectedSysDirectory = selectedItem;

            SetSelectedDatabase(selectedItem.SysDatabase);
            DatabaseService.SetSelection(viewModel.Databases, selectedItem.SysDatabase);

            //Revisar a necessidade de um serviço para conexaoaddress. Utilizando assim apenas como paliativa.
            viewModel.conexaoFileService.SetConexaoAddress(selectedItem.Path);
        }

        private void ExeSys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as string;

            //mainExe = selectedItem;
            OpenSecondaryExeButtonText.Text = $"Iniciar \n{selectedItem}";
            secondaryExe = selectedItem;

            //Debug.WriteLine($"\nExeListSelected: {selectedItem}\n");
        }

        private void OpenMainExeButton_Click(object sender, RoutedEventArgs e)
        {
            string exe = $@"{selectedSysDirectory.Path}\{mainExe}";
            if (string.IsNullOrEmpty(mainExe) && !File.Exists(exe))
            {
                MessageBox.Show("Nenhum executável encontrado.\nSelecione um executável.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Process.Start(exe);
        }

        private void OpenSecondaryExeButton_Click(object sender, RoutedEventArgs e)
        {
            string exe = $@"{selectedSysDirectory.Path}\{secondaryExe}";
            if (string.IsNullOrEmpty(secondaryExe) && !File.Exists(exe))
            {
                MessageBox.Show("Nenhum executável selecionado.\nSelecione um executável.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Process.Start(exe);
        }

        private void ToSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow();

            Application.Current.MainWindow = settings;
            settings.Show();

            Window.GetWindow(this)?.Close();
        }

        private void AddDb_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFramePublic.Navigate(new EditDbPage(viewModel));
        }

        private void EditarBase_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;

            if (menuItem?.DataContext is DatabaseModel db)
            {
                ((MainWindow)Application.Current.MainWindow).MainFramePublic.Navigate(new EditDbPage(viewModel, db));
            }
        }

        private void DeleteDatabase_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem?.DataContext is DatabaseModel db)
            {
                var result = MessageBox.Show($"Tem certeza que deseja excluir o banco de dados '{db.DisplayName}'?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.Databases.Remove(db);
                    GetFilter(viewModel.Databases);
                }
            }
        }

        private void CopyStringClick_Click(object sender, RoutedEventArgs e)
        {
            string connString = viewModel.SelectedDatabase.DbType.ToLower().StartsWith("s")
                ? viewModel.SqlService.CreateSQLServerConnectionString(viewModel.SelectedDatabase.Environment, viewModel.SelectedDatabase.Name, viewModel.SelectedDatabase.Server)
                : viewModel.OracleService.CreateOracleConnectionString(viewModel.SelectedDatabase.Environment, viewModel.SelectedDatabase.Server, viewModel.SelectedDatabase.Instance, viewModel.SelectedDatabase.Name);

            Clipboard.SetText(connString);
        }

        //criar função de resetar o placeholder para quando trocar de tab o campo resetar
        private void dbSearchPlaceholder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dbSearch.Text) || dbSearch.Text.Equals("Pesquisar Bases...", StringComparison.CurrentCultureIgnoreCase))
            {
                dbSearch.Text = string.Empty;
                dbSearch.Foreground = (Brush)new BrushConverter().ConvertFromString("#37194D");
            }
        }

        private void dbSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dbSearch.Text))
            {
                searchPlaceholder();
            }
        }

        public void searchPlaceholder()
        {
            dbSearch.Text = "Pesquisar Bases...";
            dbSearch.Foreground = (Brush)new BrushConverter().ConvertFromString("#999897");
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid.Focus();
        }

        private void dbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string environment = tabSelected == 0 ? "local" : "server";

            if(lstTodosBancos.Items.Count > 0)
                lstTodosBancos.ItemsSource = !string.IsNullOrEmpty(dbSearch.Text) ? databaseList.Where(db => db.Name.Contains(dbSearch.Text, StringComparison.OrdinalIgnoreCase) 
                && db.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase)) : databaseList;

            //Debug.WriteLine($"\n\nSearch Text: {dbSearch.Text}\n\n");
        }

        //WIP
        private void Db_LostFocus(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine($"\n\nLost Focus\n\n");

            //var ctx = (ListBox)sender;



            //foreach (var item in ctx.Items)
            //{
            //    if (item is MenuItem mi)
            //    {
            //        var border = mi.Template.FindName("Border", mi) as Border;
            //        border.Background = (Brush)new BrushConverter().ConvertFromString("Red");
            //        border.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#2a353a");
            //    }
            //}

        }

        private async void RefreshDbList_Click(object sender, RoutedEventArgs e)
        {
            //Local
            await viewModel.openSqlConn(viewModel.SqlService, viewModel.LocalSQLServerConnection);
            await viewModel.openOracleConn(viewModel.OracleService, viewModel.LocalOracleConnection);

            //Server
            await viewModel.openSqlConn(viewModel.SqlService, viewModel.ServerSQLServerConnection);
            await viewModel.openOracleConn(viewModel.OracleService, viewModel.ServerOracleConnection);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
