using System;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TrocaBaseGUI.Services;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class SettingsPage : Page
    {

        public MainViewModel _viewModel;

        public SettingsPage()
        {
            InitializeComponent();

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            _viewModel = mainWindow.MainVM;
            this.DataContext = _viewModel;

            OraclePort.Text = _viewModel.OracleConnection.Port;
            OraclePassword.Password = _viewModel.OracleConnection.Password;
        }
        public void SetSqlServerSettings(string server)
        {
            _viewModel.SQLServerConnection.Server = server;
        }

        private async void SqlServerTestConn_Click(object sender, RoutedEventArgs e)
        {
            if(await _viewModel.SqlService.ValidateConnection(sqlServerServer.Text))
            {
                SetSqlServerSettings(sqlServerServer.Text);
                MessageBox.Show("Conexão com o SQL Server estabelecida.");
            }
            else
            {
                MessageBox.Show("Falha ao conectar ao SQL Server. Verifique o servidor informado.");
            }
        }

        private void OracleTestConn_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.OracleService.ValidateConnection(_viewModel.OracleConnection.GetConnectionString(OracleUser.Text, OraclePassword.Password, OraclePort.Text)))
            {
                SetOracleSettings(OracleUser.Text, OraclePassword.Password, OraclePort.Text);
                MessageBox.Show("Conexão com o Oracle estabelecida.");
            }
            else
            {
                MessageBox.Show("Falha ao conectar ao Oracle. Verifique a string informada.");
            }
        }

        public void SetOracleSettings(string user, string password, string port)
        {
            _viewModel.OracleConnection.User = user;
            _viewModel.OracleConnection.Password = password;
            _viewModel.OracleConnection.Port = port;
        }

        private void SalvarButton_Click(object sender, RoutedEventArgs e)
        {
            SetSqlServerSettings(sqlServerServer.Text);
            SetOracleSettings(OracleUser.Text, OraclePassword.Password, OraclePort.Text);
        }
        private void VoltarButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;

            mainWindow.MainFramePublic.Navigate(new MainPage(_viewModel));
        }
    }
}
