using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TrocaBaseGUI.Services;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class ServerSettingsPage : Page
    {

        public MainViewModel _viewModel;

        public ServerSettingsPage()
        {
            InitializeComponent();

            var mainWindow = (SettingsWindow)Application.Current.MainWindow;
            _viewModel = mainWindow.viewModel;
            this.DataContext = _viewModel;

            SqlServerPassword.Password = _viewModel.ServerSQLServerConnection.Password;
            OraclePassword.Password = _viewModel.ServerOracleConnection.Password;
        }

        private async void SqlServerTestConn_Click(object sender, RoutedEventArgs e)
        {
            if (await _viewModel.SqlService.ValidateConnection(_viewModel.ServerSQLServerConnection))
            {
                await _viewModel.openSqlConn(_viewModel.SqlService, _viewModel.ServerSQLServerConnection);
                MessageBox.Show("Conexão do servidor com o SQL Server estabelecida.");
            }
            else
            {
                MessageBox.Show("Falha ao conectar ao SQL Server. Verifique o servidor informado.");
            }
        }

        private async void OracleTestConn_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.OracleService.GetRunningInstances().Count > 0 &&
                    await _viewModel.OracleService.ValidateConnection(_viewModel.ServerOracleConnection, _viewModel.OracleService.GetRunningInstances()[0]))
            {
                await _viewModel.openOracleConn(_viewModel.OracleService, _viewModel.ServerOracleConnection);
                MessageBox.Show("Conexão do servidor com o Oracle estabelecida.");
            }
            else
            {
                MessageBox.Show("Falha ao conectar ao Oracle. Verifique a string informada.");
            }
        }

        private void SqlServerServer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(SqlServerServer.Text))
                _viewModel.ServerSQLServerConnection.Server = SqlServerServer.Text;
        }

        private void SqlServerPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(SqlServerPassword.Password))
                _viewModel.ServerSQLServerConnection.Password = SqlServerPassword.Password;
        }

        private void SqlServerUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(SqlServerUser.Text))
                _viewModel.ServerSQLServerConnection.Username = SqlServerUser.Text;
        }

        private void OracleServer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(OracleServer.Text))
                _viewModel.ServerOracleConnection.Server = OracleServer.Text;
        }

        private void OraclePassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(OraclePassword.Password))
                _viewModel.ServerOracleConnection.Password = OraclePassword.Password;
        }

        private void OracleInstance_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(OracleInstance.Text))
                _viewModel.ServerOracleConnection.Instance = OracleInstance.Text;
        }

        private void OraclePort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(OraclePort.Text))
                _viewModel.ServerOracleConnection.Port = OraclePort.Text;
        }
    }
}
