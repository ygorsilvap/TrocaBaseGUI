using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TrocaBaseGUI.Services;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class SettingsPage : Page
    {

        public SqlServerService sqlServerService { get; set; } = new SqlServerService(new SqlServerConnectionModel());
        public MainViewModel _viewModel;
        public SettingsPage(MainViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            sqlServerServer.Text = "DESKTOP-N8OLEBQ\\SQLExpress";
        }

        public void sqlServerSettings(string server)
        {
            _viewModel.SQLServerConnection.Server = server;
        }

        private void SqlServerTestConn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            sqlServerService.LoadSqlServerDatabases(sqlServerServer.Text);
            }
            catch
            {
                Console.WriteLine("FALHA NA CONEXAO");
            }
        }
        private void VoltarButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }


    }
}
