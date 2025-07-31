using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
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
        public List<double> SqlMedT = new List<double>();
        public List<double> OraMedT = new List<double>();

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

        public double AverageCalc(List<double> arr)
        {
            double sum = 0;
            foreach (var n in arr)
            {
                sum += n;
            }
            return sum = sum / arr.Count;
        }

        private async void SqlServerTestConn_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if(await _viewModel.SqlService.ValidateConnection(sqlServerServer.Text))
            {
                SetSqlServerSettings(sqlServerServer.Text);
                sw.Stop();
                TimeSpan elapsed = sw.Elapsed;
                SqlMedT.Add(elapsed.TotalMilliseconds);
                Console.WriteLine($"\n\n\nSQL Elapsed time: {elapsed.TotalMilliseconds} ms\n");
                Console.WriteLine($"SQL Average time: {AverageCalc(SqlMedT)}ms\n");
                //MessageBox.Show("Conexão com o SQL Server estabelecida.");
            }
            else
            {
                MessageBox.Show("Falha ao conectar ao SQL Server. Verifique o servidor informado.");
            }

        }

        private async void OracleTestConn_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (await _viewModel.OracleService.ValidateConnection(_viewModel.OracleConnection.GetConnectionString(OracleUser.Text, OraclePassword.Password, OraclePort.Text)))
            {
                SetOracleSettings(OracleUser.Text, OraclePassword.Password, OraclePort.Text);
                sw.Stop();
                TimeSpan elapsed = sw.Elapsed;
                OraMedT.Add(elapsed.TotalMilliseconds);
                Console.WriteLine($"\n\n\nORA Elapsed time: {elapsed.TotalMilliseconds} ms\n");
                Console.WriteLine($"ORA Average time: {AverageCalc(OraMedT)}ms\n");
                //MessageBox.Show("Conexão com o Oracle estabelecida.");
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
