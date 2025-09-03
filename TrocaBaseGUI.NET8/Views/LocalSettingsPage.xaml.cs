using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
//using System.Reactive;
//using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TrocaBaseGUI.Services;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class LocalSettingsPage : Page
    {

        public MainViewModel _viewModel;
        public List<double> SqlMedT = new List<double>();
        public List<double> OraMedT = new List<double>();

        public LocalSettingsPage()
        {
            InitializeComponent();

            var mainWindow = (SettingsWindow)Application.Current.MainWindow;
            _viewModel = mainWindow.viewModel;
            this.DataContext = _viewModel;

            OraclePassword.Password = _viewModel.LocalOracleConnection.Password;
            SqlServerPassword.Password = _viewModel.LocalSQLServerConnection.Password;

        }
        public void SetSqlServerSettings(string server)
        {
            if(string.IsNullOrEmpty(server))
            {
                //MessageBox.Show("O campo do servidor SQL Server não pode estar vazio.");
                return;
            }
            else
            {
                _viewModel.LocalSQLServerConnection.Server = server;
            }
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
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            SqlServerConnectionModel sqlServerConnection = new SqlServerConnectionModel() { Server = sqlServerServer.Text };
            if (await _viewModel.SqlService.ValidateConnection(sqlServerConnection))
            {
                SetSqlServerSettings(sqlServerServer.Text);
                await _viewModel.openSqlConn(_viewModel.SqlService, _viewModel.LocalSQLServerConnection);
                //sw.Stop();
                //TimeSpan elapsed = sw.Elapsed;
                //SqlMedT.Add(elapsed.TotalMilliseconds);
                //Console.WriteLine("\n\n\nTUDO CERTO");
                //Console.WriteLine($"\nSQL Elapsed time: {elapsed.TotalMilliseconds} ms\n");
                //Console.WriteLine($"SQL Average time: {AverageCalc(SqlMedT)}ms\n");
                MessageBox.Show("Conexão com o SQL Server estabelecida.");
            }
            else
            {
                
                //sw.Stop();
                //TimeSpan elapsed = sw.Elapsed;
                //SqlMedT.Add(elapsed.TotalMilliseconds);
                //Console.WriteLine("\n\n\nNADA CERTO");
                //Console.WriteLine($"\nSQL Elapsed time: {elapsed.TotalMilliseconds} ms\n");
                //Console.WriteLine($"SQL Average time: {AverageCalc(SqlMedT)}ms\n");
                MessageBox.Show("Falha ao conectar ao SQL Server. Verifique o servidor informado.");
            }

        }

        private async void OracleTestConn_Click(object sender, RoutedEventArgs e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            OracleConnectionModel oracleConnection = new OracleConnectionModel() 
                { Server = OracleServer.Text, Password = OraclePassword.Password, Port = OraclePort.Text, Environment = _viewModel.LocalOracleConnection.Environment };

            if (_viewModel.OracleService.GetRunningInstances().Count > 0 &&
                    await _viewModel.OracleService.ValidateConnection(oracleConnection, _viewModel.OracleService.GetRunningInstances()[0]))
            {
                SetOracleSettings(OracleServer.Text, OraclePassword.Password, OracleInstance.Text, OraclePort.Text);
                await _viewModel.openOracleConn(_viewModel.OracleService, _viewModel.LocalOracleConnection);
                //sw.Stop();
                //TimeSpan elapsed = sw.Elapsed;
                //OraMedT.Add(elapsed.TotalMilliseconds);
                //Console.WriteLine("\n\n\nTUDO CERTO");
                //Console.WriteLine($"\nORA Elapsed time: {elapsed.TotalMilliseconds} ms\n");
                //Console.WriteLine($"ORA Average time: {AverageCalc(OraMedT)}ms\n");
                MessageBox.Show("Conexão com o Oracle estabelecida.");
            }
            else
            {
                //sw.Stop();
                //TimeSpan elapsed = sw.Elapsed;
                //OraMedT.Add(elapsed.TotalMilliseconds);
                //Console.WriteLine("\n\n\nNADA CERTO");
                //Console.WriteLine($"\nORA Elapsed time: {elapsed.TotalMilliseconds} ms\n");
                //Console.WriteLine($"ORA Average time: {AverageCalc(OraMedT)}ms\n");
                MessageBox.Show("Falha ao conectar ao Oracle. Verifique a string informada.");
            }
        }

        public void SetOracleSettings(string server, string password, string instance, string port)
        {
            _viewModel.LocalOracleConnection.Server = server;
            _viewModel.LocalOracleConnection.Password = password;
            _viewModel.LocalOracleConnection.Instance = instance;
            _viewModel.LocalOracleConnection.Port = port;
        }

        private void SalvarButton_Click(object sender, RoutedEventArgs e)
        {
            SetSqlServerSettings(sqlServerServer.Text);
            SetOracleSettings(OracleServer.Text, OraclePassword.Password, OracleInstance.Text, OraclePort.Text);
        }
    }
}
