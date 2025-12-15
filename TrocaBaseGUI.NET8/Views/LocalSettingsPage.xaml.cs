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
            if(!string.IsNullOrEmpty(_viewModel.LocalOracleConnection.Password))
                OraclePasswordMask.Text = new string('•', _viewModel.LocalOracleConnection.Password.Length);

            if (!string.IsNullOrEmpty(_viewModel.LocalSQLServerConnection.Password))
                SqlPasswordMask.Text = new string('•', _viewModel.LocalSQLServerConnection.Password.Length);
        }

        private async void SqlServerTestConn_Click(object sender, RoutedEventArgs e)
        {
            //if (await _viewModel.SqlService.ValidateConnection(_viewModel.LocalSQLServerConnection))
            SqlServerConnectionTestButton.IsEnabled = false;
            SqlServerConnectionTestButton.Content = string.Empty;
            SqlServerLoadingCircle.Visibility = Visibility.Visible;
                
                var tasks = new List<Task>
                {
                    _viewModel.OpenSqlConn(_viewModel.SqlService, _viewModel.LocalSQLServerConnection)
                };

                await Task.WhenAll(tasks);

            SqlServerConnectionTestButton.IsEnabled = true;
            SqlServerLoadingCircle.Visibility = Visibility.Hidden;
            SqlServerConnectionTestButton.Content = "Testar Conexão";
        }

        private async void OracleTestConn_Click(object sender, RoutedEventArgs e)
        {
            //string instance = !string.IsNullOrEmpty(_viewModel.LocalOracleConnection.Instance) ? 
            //    _viewModel.LocalOracleConnection.Instance : _viewModel.OracleService.GetRunningInstances()[0];

            OracleConnectionTestButton.IsEnabled = false;
            OracleConnectionTestButton.Content = string.Empty;
            OracleLoadingCircle.Visibility = Visibility.Visible;

            var tasks = new List<Task>
                {
                    _viewModel.OpenOracleConn(_viewModel.OracleService, _viewModel.LocalOracleConnection)
                };

            await Task.WhenAll(tasks);

            OracleConnectionTestButton.IsEnabled = true;
            OracleLoadingCircle.Visibility = Visibility.Hidden;
            OracleConnectionTestButton.Content = "Testar Conexão";
        }

        private void SqlServerPassword_TextChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.LocalSQLServerConnection.Password = SqlServerPassword.Text;

            int pwdLength = SqlServerPassword.Text.Length;
            SqlPasswordMask.Text = new string('•', pwdLength);
        }

        private void OraclePassword_TextChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.LocalOracleConnection.Password = OraclePassword.Text;

            int pwdLength = OraclePassword.Text.Length;
            OraclePasswordMask.Text = new string('•', pwdLength);
        }
    }
}
