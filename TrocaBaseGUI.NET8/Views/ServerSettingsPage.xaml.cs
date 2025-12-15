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
            if (!string.IsNullOrEmpty(_viewModel.ServerOracleConnection.Password))
                OraclePasswordMask.Text = new string('•', _viewModel.ServerOracleConnection.Password.Length);

            if (!string.IsNullOrEmpty(_viewModel.ServerSQLServerConnection.Password))
                SqlPasswordMask.Text = new string('•', _viewModel.ServerSQLServerConnection.Password.Length);
        }

        private async void SqlServerTestConn_Click(object sender, RoutedEventArgs e)
        {
            SqlServerConnectionTestButton.IsEnabled = false;
            SqlServerConnectionTestButton.Content = string.Empty;
            SqlServerLoadingCircle.Visibility = Visibility.Visible;

            var tasks = new List<Task>
                {
                    _viewModel.OpenSqlConn(_viewModel.SqlService, _viewModel.ServerSQLServerConnection)
                };

            await Task.WhenAll(tasks);

            SqlServerConnectionTestButton.IsEnabled = true;
            SqlServerLoadingCircle.Visibility = Visibility.Hidden;
            SqlServerConnectionTestButton.Content = "Testar Conexão";
        }

        private async void OracleTestConn_Click(object sender, RoutedEventArgs e)
        {
            OracleConnectionTestButton.IsEnabled = false;
            OracleConnectionTestButton.Content = string.Empty;
            OracleLoadingCircle.Visibility = Visibility.Visible;

            var tasks = new List<Task>
                {
                    _viewModel.OpenOracleConn(_viewModel.OracleService, _viewModel.ServerOracleConnection)
                };

            await Task.WhenAll(tasks);

            OracleConnectionTestButton.IsEnabled = true;
            OracleLoadingCircle.Visibility = Visibility.Hidden;
            OracleConnectionTestButton.Content = "Testar Conexão";
        }
        private void SqlServerPassword_TextChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ServerSQLServerConnection.Password = SqlServerPassword.Text;

            int pwdLength = SqlServerPassword.Text.Length;
            SqlPasswordMask.Text = new string('•', pwdLength);

            Debug.WriteLine($"\n\nsqlpwd: {_viewModel.ServerSQLServerConnection.Password}\n\n");
        }

        private void OraclePassword_TextChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ServerOracleConnection.Password = OraclePassword.Text;

            int pwdLength = OraclePassword.Text.Length;
            OraclePasswordMask.Text = new string('•', pwdLength);

            Debug.WriteLine($"\n\nsqlpwd: {_viewModel.ServerOracleConnection.Password}\n\n");

        }
    }
}
