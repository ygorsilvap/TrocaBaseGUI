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
        public ObservableCollection<DatabaseModel> dbList { get; set; } = new ObservableCollection<DatabaseModel>();
        public ObservableCollection<SysDirectoryModel> hist { get; set; }
        private MainViewModel viewModel;
        public int tabSelected;
        public string rbSelected;
        public string sysSelected;
        public string exeSelected;
        //public string dbSearch;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage(MainViewModel vm)
        {
            InitializeComponent();

            viewModel = vm;
            this.DataContext = viewModel;

            hist = new ObservableCollection<SysDirectoryModel>(viewModel.History);

            RadioButton_Checked(rbTodos, null);
            tabSelected = TabControl.SelectedIndex;
            //dirSys.SelectedValue = hist.Count > 0 ? hist.First().Folder : "";

            //Fazer Binding com esses campos de exe
            OpenSysButtonText.Text = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Selecione um executável" : $"Iniciar \n{StringUtils.ToCapitalize(MainViewModel.exeFile)}";
            OpenExeButtonText.Text = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Selecione um executável" : $"Iniciar \n{StringUtils.ToCapitalize(MainViewModel.exeFile)}";

            IsThereSysDirectory.Text = string.IsNullOrWhiteSpace(MainViewModel.exeFile) ? "Nenhum executável encontrado.\nSelecione um executável." : "";
            GetFilter(dbList);

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
            if(viewModel.Databases == null || viewModel.Databases.Count == 0)
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

            dbList.Clear();

            foreach (var db in viewModel.Databases)
            {
                DatabaseModel.SetDisplayName(db, db.DisplayName);
                dbList.Add(db);
            }

            viewModel.DbService.SortDatabases(dbList);

            GetFilter(dbList);
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

        private void TrocarBase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm && !String.IsNullOrEmpty(MainViewModel.exeFile))
            {
                Debug.WriteLine($"\n\nIDSelDb: {viewModel.SelDatabase.Id}\n\n");
                vm.SelectBase(vm.Databases, viewModel.SelDatabase.Id, dirSys.SelectedValue.ToString());
            } else
            {
                MessageBox.Show("Nenhum executável encontrado.\nSelecione um executável.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Refresh();
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            GetFilter(dbList);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
                tabSelected = tabControl.SelectedIndex;

            GetFilter(dbList);
        }

        //Refatorar
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
                string folder = $"\\{System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(dialog.FileName))}";
                string path = System.IO.Path.GetDirectoryName(dialog.FileName);
                string exeName = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);

                ObservableCollection<string> exeList = new ObservableCollection<string>(Directory.GetFiles(path, "*.exe").Select(f => System.IO.Path.GetFileNameWithoutExtension(f)).ToList());

                viewModel.AddDirectory(folder, path, exeName, exeList);

                viewModel.conexaoFileService.SetConexaoAddress(path);

                viewModel.appState.SelectedFolder = folder;

                dirSys.SelectedItem = viewModel.History.FirstOrDefault();

                Refresh();
            }
        }

        //Refatorar
        private void dirSys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as SysDirectoryModel;
            if (selectedItem != null) 
            { 
                viewModel.appState.SelectedFolder = selectedItem.Folder;
            } else
            {
                viewModel.appState.SelectedFolder = hist.LastOrDefault().Folder;
            }

            string selectedDir = comboBox.SelectedValue as string;

            if (string.IsNullOrEmpty(selectedDir)) return;

            viewModel.ExeFilesList = SysDirectoryModel.GetDir(viewModel.History, selectedDir)?.ExeList;
            exeSys.ItemsSource = viewModel.ExeFilesList;

            int selectedBaseDir = SysDirectoryModel.GetDir(viewModel.History, selectedDir).SelectedBase;

            if (selectedItem != null)
            {
                viewModel.conexaoFileService.SetConexaoAddress(selectedItem.Path);
                MainViewModel.exeFile = selectedItem.MainExeFile;

                if (dbList.Count() > 0 && viewModel.History.Any(i => i.SelectedBase >= 0) && selectedBaseDir > 0)
                {
                    DatabaseModel.SetSelection(dbList, selectedBaseDir);
                    viewModel.SelDatabase = dbList[selectedBaseDir];
                }
            }

            Refresh();
            Debug.Write($"\nviewModel.conexaoFile: {viewModel.conexaoFile}\n");
        }

        private void exeSys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as string;

            exeSelected = selectedItem;
            OpenExeButtonText.Text = $"Iniciar \n{exeSelected}";

            Debug.WriteLine($"\nExeListSelected: {selectedItem}\n");
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

        private void OpenMainExeButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start($@"{System.IO.Path.GetDirectoryName(viewModel.conexaoFile)}\{MainViewModel.exeFile}.exe");
        }

        private void OpenSecondaryExeButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start($@"{System.IO.Path.GetDirectoryName(viewModel.conexaoFile)}\{exeSelected}.exe");
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
                lstTodosBancos.ItemsSource = dbList.Where(db => db.Name.ToLower().Contains(dbSearch.Text.ToLower()) 
                && db.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase));
            Debug.WriteLine($"\n\nSearch Text: {dbSearch.Text}\n\n");
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine($"\n\nTier: {viewModel.Conexao2Camadas.Tier}");
            //Debug.WriteLine($"\nDefaultLogin: {viewModel.Conexao2Camadas.DefaultLogin}");
            //Debug.WriteLine($"\nDefaultPassword: {viewModel.Conexao2Camadas.DefaultPassword}");
            //Debug.WriteLine($"\nTextEditorPath: {viewModel.Conexao2Camadas.TextEditorPath}");
            //Debug.WriteLine($"\nUpdateFolder: {viewModel.Conexao2Camadas.UpdateFolder}");
            //Debug.WriteLine($"\nUseWebMenu: {viewModel.Conexao2Camadas.UseWebMenu}\n\n");
            //Debug.WriteLine($"\n\nUseRedirect: {viewModel.Conexao3Camadas.UseRedirect}\n\n");
            //Debug.WriteLine($"\n\nRedirectPort: {viewModel.Conexao3Camadas.RedirectPort}\n\n");
            //viewModel.conexaoFileService.Create3CConnectionClientFileSettings(viewModel.Conexao3Camadas, viewModel.appState.ServerParams);
            //viewModel.conexaoFileService.CreatePortsSettings(viewModel.Conexao3Camadas);
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
    }
}
