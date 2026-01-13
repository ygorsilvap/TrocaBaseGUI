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

        public MainViewModel viewModel;

        public ServerSettingsPage()
        {
            InitializeComponent();

            var mainWindow = (SettingsWindow)Application.Current.MainWindow;
            viewModel = mainWindow.viewModel;
            DataContext = viewModel;
        }

        private void ServerPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(viewModel.ServerOracleConnection.Password))
                OraclePasswordMask.Text = new string('•', viewModel.ServerOracleConnection.Password.Length);

            if (!string.IsNullOrEmpty(viewModel.ServerSQLServerConnection.Password))
                SqlPasswordMask.Text = new string('•', viewModel.ServerSQLServerConnection.Password.Length);

            if (viewModel.isSqlLoading)
                SetLoadingState("s");

            if (viewModel.isOracleLoading)
                SetLoadingState("o");
        }

        private async void SqlServerTestConn_Click(object sender, RoutedEventArgs e)
        {
            var dbType = sender as Button;

            viewModel.isSqlLoading = true;
            SetLoadingState(dbType.Name);

            var tasks = new List<Task>
                {
                    viewModel.OpenSqlConn(viewModel.SqlService, viewModel.ServerSQLServerConnection)
                };

            await Task.WhenAll(tasks);

            viewModel.isSqlLoading = false;
            SetLoadingState(dbType.Name);
        }

        private async void OracleTestConn_Click(object sender, RoutedEventArgs e)
        {
            var dbType = sender as Button;

            viewModel.isOracleLoading = true;
            SetLoadingState(dbType.Name);

            var tasks = new List<Task>
                {
                    viewModel.OpenOracleConn(viewModel.OracleService, viewModel.ServerOracleConnection)
                };

            await Task.WhenAll(tasks);

            viewModel.isOracleLoading = false;
            SetLoadingState(dbType.Name);
        }
        private void SqlServerPassword_TextChanged(object sender, RoutedEventArgs e)
        {
            viewModel.ServerSQLServerConnection.Password = SqlServerPassword.Text;

            int pwdLength = SqlServerPassword.Text.Length;
            SqlPasswordMask.Text = new string('•', pwdLength);
        }

        private void OraclePassword_TextChanged(object sender, RoutedEventArgs e)
        {
            viewModel.ServerOracleConnection.Password = OraclePassword.Text;

            int pwdLength = OraclePassword.Text.Length;
            OraclePasswordMask.Text = new string('•', pwdLength);
        }

        private void SetLoadingState(string dbType)
        {
            if (dbType.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                if (viewModel.isSqlLoading)
                {
                    SqlServerConnectionTestButton.IsEnabled = false;
                    SqlServerConnectionTestButton.Content = string.Empty;
                    SqlServerLoadingCircle.Visibility = Visibility.Visible;
                }
                else
                {
                    SqlServerConnectionTestButton.IsEnabled = true;
                    SqlServerLoadingCircle.Visibility = Visibility.Hidden;
                    SqlServerConnectionTestButton.Content = "Testar Conexão";
                }
            }
            else
            {
                if (viewModel.isOracleLoading)
                {
                    OracleConnectionTestButton.IsEnabled = false;
                    OracleConnectionTestButton.Content = string.Empty;
                    OracleLoadingCircle.Visibility = Visibility.Visible;
                }
                else
                {
                    OracleConnectionTestButton.IsEnabled = true;
                    OracleLoadingCircle.Visibility = Visibility.Hidden;
                    OracleConnectionTestButton.Content = "Testar Conexão";
                }
            }
        }

    }
}
