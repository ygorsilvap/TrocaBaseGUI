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
            //sqlServerServer.Text = "MTZNOTFS058680";
        }

        public void sqlServerSettings(string server)
        {
            _viewModel.SQLServerConnection.Server = server;
        }

        private void SqlServerTestConn_Click(object sender, RoutedEventArgs e)
        {
            if(sqlServerService.ValidateConnection(sqlServerServer.Text))
            {
                sqlServerSettings(sqlServerServer.Text);
                _viewModel.SQLServerConnection.Server = sqlServerServer.Text;
                MessageBox.Show("Conexão com o SQL Server estabelecida.");
            }
            else
            {
                MessageBox.Show("Falha ao conectar ao SQL Server. Verifique o servidor informado.");
            }
        }
        private void VoltarButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }


    }
}
