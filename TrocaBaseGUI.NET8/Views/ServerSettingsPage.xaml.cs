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
            DataContext = _viewModel;
        }

        private void ServerPage_Loaded(object sender, RoutedEventArgs e)
        {
            //SqlServerPassword.Password = _viewModel.ServerSQLServerConnection.Password;
            //OraclePassword.Password = _viewModel.ServerOracleConnection.Password;
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
                    await _viewModel.OracleService.ValidateConnection(_viewModel.ServerOracleConnection, _viewModel.ServerOracleConnection.Instance))
            {
                await _viewModel.openOracleConn(_viewModel.OracleService, _viewModel.ServerOracleConnection);
                MessageBox.Show("Conexão do servidor com o Oracle estabelecida.");
            }
            else
            {
                MessageBox.Show("Falha ao conectar ao Oracle. Verifique a string informada.");
            }
        }
        private void SqlServerPassword_TextChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ServerSQLServerConnection.Password = SqlServerPassword.Text;
            int pwdLength = SqlServerPassword.Text.Length;
            SqlServerPassword.Text = new string('•', pwdLength);
            SqlServerPassword.CaretIndex = SqlServerPassword.Text.Length;
        }

        private void OraclePassword_TextChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ServerOracleConnection.Password = OraclePassword.Text;
            int pwdLength = OraclePassword.Text.Length;
            OraclePassword.Text = new string('•', pwdLength);
            OraclePassword.CaretIndex = OraclePassword.Text.Length;

        }
    }
}
