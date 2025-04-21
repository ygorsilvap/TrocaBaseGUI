using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Banco> listaBancos { get; set; }
        private MainViewModel viewModel;
        public int tabSelected;
        public string rbSelected;
        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel();
            this.DataContext = viewModel;
            listaBancos = new ObservableCollection<Banco>(viewModel.dbFiles);
            lstTodosBancos.ItemsSource = listaBancos;
            RadioButton_Checked(rbTodos, null);
            tabSelected = TabControl.SelectedIndex;
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
    }
}