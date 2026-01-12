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

        public MainViewModel viewModel;

        public LocalSettingsPage()
        {
            InitializeComponent();

            var mainWindow = (SettingsWindow)Application.Current.MainWindow;
            viewModel = mainWindow.viewModel;
            DataContext = viewModel;
        }

        private void LocalPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(viewModel.LocalOracleConnection.Password))
                OraclePasswordMask.Text = new string('•', viewModel.LocalOracleConnection.Password.Length);

            if (!string.IsNullOrEmpty(viewModel.LocalSQLServerConnection.Password))
                SqlPasswordMask.Text = new string('•', viewModel.LocalSQLServerConnection.Password.Length);

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
                    viewModel.OpenSqlConn(viewModel.SqlService, viewModel.LocalSQLServerConnection)
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
                    viewModel.OpenOracleConn(viewModel.OracleService, viewModel.LocalOracleConnection)
                };

            await Task.WhenAll(tasks);

            viewModel.isOracleLoading = false;
            SetLoadingState(dbType.Name);
        }

        private void SqlServerPassword_TextChanged(object sender, RoutedEventArgs e)
        {
            viewModel.LocalSQLServerConnection.Password = SqlServerPassword.Text;

            int pwdLength = SqlServerPassword.Text.Length;
            SqlPasswordMask.Text = new string('•', pwdLength);
        }

        private void OraclePassword_TextChanged(object sender, RoutedEventArgs e)
        {
            viewModel.LocalOracleConnection.Password = OraclePassword.Text;

            int pwdLength = OraclePassword.Text.Length;
            OraclePasswordMask.Text = new string('•', pwdLength);
        }

        private void SetLoadingState(string dbType)
        {
            //sql server
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
            //oracle
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
