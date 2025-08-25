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
    public partial class ServerSettingsPage : Page
    {

        public MainViewModel _viewModel;
        public List<double> SqlMedT = new List<double>();
        public List<double> OraMedT = new List<double>();
        public int tabSelected;

        public ServerSettingsPage()
        {
            InitializeComponent();

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            _viewModel = mainWindow.MainVM;
            this.DataContext = _viewModel;

            TabControl.SelectedIndex = 1;

            SqlServerPassword.Password = _viewModel.ServerSQLServerConnection.Password;
            OraclePassword.Password = _viewModel.ServerOracleConnection.Password;
        }
        public void SetSqlServerSettings(string server, string password, string usuario = null)
        {
            _viewModel.ServerSQLServerConnection.Server = server;
            _viewModel.ServerSQLServerConnection.Password = password;
            _viewModel.ServerSQLServerConnection.Username = usuario ?? "CNP";
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
            SqlServerConnectionModel sqlServerConnection = new SqlServerConnectionModel() { Server = sqlServerServer.Text, Username = sqlServerUser.Text, Password = SqlServerPassword.Password };

            if (await _viewModel.SqlService.ValidateConnection(sqlServerConnection))
            {
                
                SetSqlServerSettings(sqlServerServer.Text, SqlServerPassword.Password, sqlServerUser.Text);
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
                MessageBox.Show("Falha ao conectar ao SQL Server. Verifique os dados informados.");
            }

        }

        private async void OracleTestConn_Click(object sender, RoutedEventArgs e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            OracleConnectionModel oracleConnection = new OracleConnectionModel()
                { Server = oracleServer.Text, Password = OraclePassword.Password, Port = oraclePort.Text, 
                    Environment = _viewModel.ServerOracleConnection.Environment, Instance = oracleInstance.Text };

            if (await _viewModel.OracleService.ValidateConnection(oracleConnection, oracleConnection.Instance))
            {
                SetOracleSettings(oracleServer.Text, OraclePassword.Password, oraclePort.Text, oracleInstance.Text);
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
                MessageBox.Show("Falha ao conectar ao Oracle. Os dados informados.");
            }
        }

        public void SetOracleSettings(string server, string password, string port, string instance)
        {
            _viewModel.ServerOracleConnection.Server = server;
            _viewModel.ServerOracleConnection.Password = password;
            _viewModel.ServerOracleConnection.Port = port;
            _viewModel.ServerOracleConnection.Instance = instance;
        }

        private void SalvarButton_Click(object sender, RoutedEventArgs e)
        {
            SetSqlServerSettings(sqlServerServer.Text, SqlServerPassword.Password, sqlServerUser.Text);
            SetOracleSettings(oracleServer.Text, OraclePassword.Password, oraclePort.Text, oracleInstance.Text);
        }
        private void VoltarButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;

            mainWindow.MainFramePublic.Navigate(new MainPage(_viewModel));

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
                tabSelected = tabControl.SelectedIndex;
            if (tabSelected == 0)
            {
                ((MainWindow)Application.Current.MainWindow).MainFramePublic.Navigate(new LocalSettingsPage());
            }
        }
    }
}
