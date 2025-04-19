using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.ViewModels;
using Path = System.IO.Path;

namespace TrocaBaseGUI
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Banco> listaBancos { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainViewModel();
            this.DataContext = viewModel;
            listaBancos = new ObservableCollection<Banco>(viewModel.dbFiles);
            lstTodosBancos.ItemsSource = viewModel.dbFiles;
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

        }


    }
}