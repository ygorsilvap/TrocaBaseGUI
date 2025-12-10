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
        string unmaskedSqlPassword;
        string unmaskedOraclePassword;

        //ChatGPT
        private string realSqlPassword = "";
        private bool ignoreSql = false;
        private string realOraclePassword = "";
        private bool ignoreOracle = false;

        public LocalSettingsPage()
        {
            InitializeComponent();

            var mainWindow = (SettingsWindow)Application.Current.MainWindow;
            _viewModel = mainWindow.viewModel;
            DataContext = _viewModel;
        }

        private void LocalPage_Loaded(object sender, RoutedEventArgs e)
        {
            //OraclePasswordMask.Text = string.IsNullOrEmpty(_viewModel.LocalOracleConnection.Password) ?
            //    new string('•', _viewModel.LocalOracleConnection.Password.Length) : string.Empty;

            OraclePasswordMask.Text = new string('•', _viewModel.LocalOracleConnection.Password.Length);

            //SqlPasswordMask.Text = string.IsNullOrEmpty(_viewModel.LocalSQLServerConnection.Password) ?
            //    new string('•', _viewModel.LocalSQLServerConnection.Password.Length) : string.Empty;
            SqlPasswordMask.Text = new string('•', _viewModel.LocalSQLServerConnection.Password.Length);
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
            string instance = !string.IsNullOrEmpty(_viewModel.LocalOracleConnection.Instance) ? 
                _viewModel.LocalOracleConnection.Instance : _viewModel.OracleService.GetRunningInstances()[0];

            if (_viewModel.OracleService.GetRunningInstances().Count > 0 &&
                    await _viewModel.OracleService.ValidateConnection(_viewModel.LocalOracleConnection, instance))
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

            int pwdLength = SqlPasswordMask.Text.Length;
            SqlPasswordMask.Text = new string('•', pwdLength);

            Debug.WriteLine($"\n\nsqlpwd: {_viewModel.LocalSQLServerConnection.Password}\n\n");
        }

        private void OraclePassword_TextChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.LocalOracleConnection.Password = OraclePassword.Text;

            int pwdLength = OraclePassword.Text.Length;
            OraclePasswordMask.Text = new string('•', pwdLength);

            Debug.WriteLine($"\n\nsqlpwd: {_viewModel.LocalOracleConnection.Password}\n\n");
        }
    }
}
