using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.Services;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class LocalSettingsPage : Page
    {

        public MainViewModel _viewModel;

        public LocalSettingsPage()
        {
            InitializeComponent();

            var mainWindow = (SettingsWindow)Application.Current.MainWindow;
            _viewModel = mainWindow.viewModel;
            DataContext = _viewModel;
        }

        private void LocalPage_Loaded(object sender, RoutedEventArgs e)
        {
            //OraclePassword.Password = _viewModel.LocalOracleConnection.Password;
            //SqlServerPassword.Password = _viewModel.LocalSQLServerConnection.Password;
            //Debug.WriteLine($"\n\npassword: {_viewModel.LocalSQLServerConnection.Password}\n\n");
        }

        private async void SqlServerTestConn_Click(object sender, RoutedEventArgs e)
        {
            if (await _viewModel.SqlService.ValidateConnection(_viewModel.LocalSQLServerConnection))
            {
                await _viewModel.openSqlConn(_viewModel.SqlService, _viewModel.LocalSQLServerConnection);
                MessageBox.Show("Conexão local com o SQL Server estabelecida.");
            }
            else
            {
                MessageBox.Show("Falha ao conectar ao SQL Server. Verifique o servidor informado.");
            }
        }

        private async void OracleTestConn_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.OracleService.GetRunningInstances().Count > 0 &&
                    await _viewModel.OracleService.ValidateConnection(_viewModel.LocalOracleConnection, _viewModel.OracleService.GetRunningInstances()[0]))
            {
                await _viewModel.openOracleConn(_viewModel.OracleService, _viewModel.LocalOracleConnection);
                MessageBox.Show("Conexão local com o Oracle estabelecida.");
            }
            else
            {
                MessageBox.Show("Falha ao conectar ao Oracle. Verifique a string informada.");
            }
        }

        private void SqlServerPassword_TextChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.LocalSQLServerConnection.Password = SqlServerPassword.Text;
            int pwdLength = SqlServerPassword.Text.Length;
            SqlServerPassword.Text = new string('•', pwdLength);
            SqlServerPassword.CaretIndex = SqlServerPassword.Text.Length;
        }

        private void OraclePassword_TextChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.LocalOracleConnection.Password = OraclePassword.Text;
            int pwdLength = OraclePassword.Text.Length;
            OraclePassword.Text = new string('•', pwdLength);
            OraclePassword.CaretIndex = OraclePassword.Text.Length;
        }
    }
}
