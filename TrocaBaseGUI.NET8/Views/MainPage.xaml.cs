using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MessagePack;
using Microsoft.IdentityModel.Tokens;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.Services;
using TrocaBaseGUI.Utils;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class MainPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<DatabaseModel> databasesCopy { get; set; } = new ObservableCollection<DatabaseModel>();
        private MainViewModel viewModel;
        public int tabSelected;
        public string rbSelected;
        public string sysSelected;
        public string mainExe;
        public string secondaryExe;
        public string selectedDatabaseId;
        public SysDirectoryModel selectedSysDirectory = new SysDirectoryModel();
        public ObservableCollection<string> exesList { get; set; } = new ObservableCollection<string>();

        public string orderBy = "aa";

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage(MainViewModel vm)
        {
            InitializeComponent();

            viewModel = vm;
            DataContext = viewModel;

            RadioButton_Checked(rbTodos, null);
            tabSelected = TabControl.SelectedIndex;

            //foreach (var db in viewModel.Databases)
            //{
            //    Debug.WriteLine($"\nId: {db.Id}, Db: {db.Name}, Env: {db.Environment}, DbT: {db.DbType}, Server: {db.Server}");
            //}

            //Debug.WriteLine($"\n\nCopyStringClick_Click - DbId: {viewModel.SelectedDatabase.Environment}, {viewModel.SelectedDatabase.Name}, {viewModel.SelectedDatabase.Server}\n\n");

            //UtilityService.IdGen();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetSearchPlaceholder();

            viewModel.sysDirectoryService.UpdateSysDirectoriesFiles(viewModel.SysDirectoryList);

            SetExesSelection();

            //SetSelectedDatabase(selectedDatabaseId);

            SetDabaseCopyDbs();

            SetLoadingState();
        }

        private void SetDabaseCopyDbs()
        {
            databasesCopy.Clear();

            foreach (var db in viewModel.Databases)
            {
                DatabaseService.SetDisplayName(db, db.DisplayName);
                databasesCopy.Add(db);
            }

            viewModel.DbService.SortDatabasesByName(databasesCopy);

            SetDatabaseListFiltered(databasesCopy);
        }

        private void SetDatabaseListFiltered(ObservableCollection<DatabaseModel> db)
        {
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
                DatabaseList.ItemsSource = viewModel.EnvironmentFilter(environment, db);
                viewModel.DbService.SortDatabasesByName(db);
                return;
            }

            ObservableCollection<DatabaseModel> bases = viewModel.EnvironmentFilter(environment, db);
            DatabaseList.ItemsSource = viewModel.DbTypeFilter(type, bases);
            viewModel.DbService.SortDatabasesByName(bases);
        }

        public void SetExesSelection()
        {
            secondaryExe = exesList.IsNullOrEmpty() || !exesList.Any(exe => exe.StartsWith("frentecaixa", StringComparison.OrdinalIgnoreCase)) ? 
                string.Empty :
                exesList.FirstOrDefault(exe => exe.StartsWith("frentecaixa", StringComparison.OrdinalIgnoreCase)).ToLower();

            //secondaryExe = !string.IsNullOrEmpty(secondaryExe) && secondaryExe.Contains("client", StringComparison.OrdinalIgnoreCase) ?
            //    secondaryExe.Replace("client", "") : secondaryExe;

            //string mainExecutable = !string.IsNullOrEmpty(mainExe) && mainExe.EndsWith("client", StringComparison.OrdinalIgnoreCase) ? mainExe.Replace("client", "") : mainExe;

            OpenMainExeButtonText.Text = string.IsNullOrWhiteSpace(mainExe) ?
                "Selecione um executável" : $"Iniciar \n{Utils.UtilityService.ToCapitalize(mainExe)}";

            OpenSecondaryExeButtonText.Text = exesList.Count <= 0 || string.IsNullOrEmpty(secondaryExe) ?
                "Selecione um executável" : $"Iniciar \n{Utils.UtilityService.ToCapitalize(secondaryExe)}";
        }

        public void SetSelectedDatabase(string database)
        {
            viewModel.SelectedDatabase = !string.IsNullOrEmpty(database) ?
                viewModel.Databases.FirstOrDefault(db => db.Id.Equals(database)) : new DatabaseModel();
        }

        private void TrocarBase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;

            if (listBox.SelectedItem == null) return;

            if (!String.IsNullOrEmpty(mainExe))
            {
                if(string.IsNullOrEmpty(selectedSysDirectory.SysDatabase))
                {
                    var del = MessageBox.Show("Ao selecionar uma base pela primeira vez em cada diretório, o conteúdo do arquivo de conexão será substituído pelo conteúdo gerado no programa.\n\nVocê deseja continuar?", "Selecionar base",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning)
                                .ToString().ToLower();

                    if (del.Equals("yes"))
                        viewModel.SelectDatabase(viewModel.Databases, viewModel.SelectedDatabase.Id, selectedSysDirectory);

                    return;
                }
                    
                viewModel.SelectDatabase(viewModel.Databases, viewModel.SelectedDatabase.Id, selectedSysDirectory);
            }
            else
            {
                MessageBox.Show("Nenhum diretório selecionado.\nSelecione um diretório.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;

            if(rb.IsLoaded)
            {
                SetDatabaseListFiltered(databasesCopy);
                SetLoadingState();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
                tabSelected = tabControl.SelectedIndex;

            SetDatabaseListFiltered(databasesCopy);
            SetSearchPlaceholder();
            SetLoadingState();
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

            if (selectedItem == null) {
                mainExe = string.Empty;
                MainExesList.ItemsSource = new List<string>();
                ExesList.ItemsSource = new ObservableCollection<string>();
                selectedDatabaseId = string.Empty;
                SetExesSelection();
                MainExesList.Visibility = Visibility.Hidden;
                return;
            }

            exesList = selectedItem.ExeFiles;
            ExesList.ItemsSource = exesList;


            if(selectedItem.MainExeFiles.Count > 1)
            {
                MainExesList.Visibility = Visibility.Visible;
                OpenMainExeButtonText.Margin = new Thickness(0,0,10,0);
                MainExesList.ItemsSource = selectedItem.MainExeFiles;

                mainExe = selectedItem.MainExeFiles[0];
            }
            else
            {
                MainExesList.Visibility = Visibility.Hidden;
                OpenMainExeButtonText.Margin = new Thickness(0, 0, 0, 0);
                mainExe = selectedItem.MainExeFiles[0];
            }

            SetExesSelection();  

            //Revisar necessidade dessa variável
            selectedSysDirectory = selectedItem;

            SetSelectedDatabase(selectedItem.SysDatabase);
            DatabaseService.SetSelection(viewModel.Databases, selectedItem.SysDatabase);

            //Revisar a necessidade de um serviço para conexaoaddress.
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

        private void MainExeFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as string;

            OpenMainExeButtonText.Text = $"Iniciar \n{selectedItem}";
            mainExe = selectedItem;
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
            //if (string.IsNullOrEmpty(selectedSysDirectory.Path) || string.IsNullOrEmpty(secondaryExe)) return;
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
                    SetDatabaseListFiltered(viewModel.Databases);
                }
            }
        }

        private void CopyStringClick_Click(object sender, RoutedEventArgs e)
        {
            string connString = viewModel.SelectedDatabase.DbType.ToLower().StartsWith("s")
                ? viewModel.SqlService.CreateSQLServerConnectionString(viewModel.SelectedDatabase.Environment, viewModel.SelectedDatabase.Name, viewModel.SelectedDatabase.Server)
                : viewModel.OracleService.CreateOracleConnectionString(viewModel.SelectedDatabase.Environment, viewModel.SelectedDatabase.Server, viewModel.SelectedDatabase.Instance, viewModel.SelectedDatabase.Name);

            Clipboard.SetText(connString);

            //Debug.WriteLine($"\n\nCopyStringClick_Click - DbId: {viewModel.SelectedDatabase.Environment}, {viewModel.SelectedDatabase.Name}, {viewModel.SelectedDatabase.Server}\n\n");
        }

        private void DbSearchPlaceholder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dbSearch.Text) || dbSearch.Text.Equals("Pesquisar Bases...", StringComparison.CurrentCultureIgnoreCase))
            {
                dbSearch.Text = string.Empty;
                dbSearch.Foreground = (Brush)new BrushConverter().ConvertFromString("#37194D");
            }
        }

        private void DbSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dbSearch.Text))
            {
                SetSearchPlaceholder();
            }
        }

        public void SetSearchPlaceholder()
        {
            dbSearch.Text = "Pesquisar Bases...";
            dbSearch.Foreground = (Brush)new BrushConverter().ConvertFromString("#999897");
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid.Focus();
        }

        private void DbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dbSearch.Text.Equals("Pesquisar Bases...")) return;

            string environment = tabSelected == 0 ? "local" : "server";

            if (viewModel.Databases.Count > 0)
                DatabaseList.ItemsSource = databasesCopy.Where(db => db.Name.StartsWith(dbSearch.Text, StringComparison.OrdinalIgnoreCase)
                                                && db.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase));

        }

        private async void RefreshDbListButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.isDbListLoading = true;

            SetLoadingState();

            List<Task> tasks;

            if (tabSelected == 0)
            {
                tasks = new List<Task>
                {
                    //Local
                    viewModel.OpenSqlConn(viewModel.SqlService, viewModel.LocalSQLServerConnection, true, false),
                    viewModel.OpenOracleConn(viewModel.OracleService, viewModel.LocalOracleConnection, true, false),
                };
            }
            else
            {
                tasks = new List<Task>
                {
                    //Server
                    viewModel.OpenSqlConn(viewModel.SqlService, viewModel.ServerSQLServerConnection, true, false),
                    viewModel.OpenOracleConn(viewModel.OracleService, viewModel.ServerOracleConnection, true, false)
                };
            }

            //var tasks = new List<Task>
            //    {
            //        //Local
            //        viewModel.OpenSqlConn(viewModel.SqlService, viewModel.LocalSQLServerConnection, true, false),
            //        viewModel.OpenOracleConn(viewModel.OracleService, viewModel.LocalOracleConnection, true, false),
            //        //Server
            //        viewModel.OpenSqlConn(viewModel.SqlService, viewModel.ServerSQLServerConnection, true, false),
            //        viewModel.OpenOracleConn(viewModel.OracleService, viewModel.ServerOracleConnection, true, false)
            //    };

            await Task.WhenAll(tasks);

            //foreach (var item in viewModel.Databases)
            //    Debug.WriteLine($"\n\ndb: {item.Id}\n\n");

            viewModel.isDbListLoading = false;
            SetLoadingState();

            //MessageBox.Show("Atualização Finalizada.");
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SortName_Header(object sender, MouseButtonEventArgs e)
        {
            orderBy = orderBy.StartsWith("a") ? orderBy : string.Empty;

            if (string.IsNullOrEmpty(orderBy) || orderBy.Equals("ad"))
            {
                viewModel.DbService.SortDatabasesByName(databasesCopy);
                orderBy = "aa";
            } else if(orderBy.Equals("aa"))
            {
                viewModel.DbService.SortDatabasesByNameDesc(databasesCopy);
                orderBy = "ad";
            }

            SetDatabaseListFiltered(databasesCopy);
            Debug.WriteLine($"\n\n{orderBy}\n\n");
        }

        private void SortDate_Header(object sender, MouseButtonEventArgs e)
        {
            orderBy = orderBy.StartsWith("d") ? orderBy : string.Empty;

            if (string.IsNullOrEmpty(orderBy) || orderBy.Equals("dd"))
            {
                viewModel.DbService.SortDatabasesByDate(databasesCopy);
                orderBy = "da";
            }
            else if (orderBy.Equals("da"))
            {
                viewModel.DbService.SortDatabasesByDateDesc(databasesCopy);
                orderBy = "dd";
            }

            SetDatabaseListFiltered(databasesCopy);
            Debug.WriteLine($"\n\n{orderBy}\n\n");
        }

        private void SetLoadingState()
        {
            if (viewModel.isDbListLoading)
            {
                DatabaseList.ItemsSource = null;
                LoadingCircle.Visibility = Visibility.Visible;
                RefreshDbListButton.IsEnabled = false;
                SettingsButton.IsEnabled = false;
                TabControl.IsEnabled = false;
            }
            else
            {
                LoadingCircle.Visibility = Visibility.Hidden;
                RefreshDbListButton.IsEnabled = true;
                SettingsButton.IsEnabled = true;
                TabControl.IsEnabled = true;
                SetDabaseCopyDbs();
            }
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    //DateTime date = new("dd/mm/yyyy");
        //    DateTime date = Convert.ToDateTime("01/03/2024");
        //    Debug.WriteLine(date);
        //}
    }
}
