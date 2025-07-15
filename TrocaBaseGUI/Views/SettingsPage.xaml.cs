using System;
using System.Reactive;
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

        public SettingsPage()
        {
            InitializeComponent();

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            _viewModel = mainWindow.MainVM; 
            this.DataContext = _viewModel;

        }

        public void SetSqlServerSettings(string server)
        {
            _viewModel.SQLServerConnection.Server = server;
        }

        private void SqlServerTestConn_Click(object sender, RoutedEventArgs e)
        {
            if(_viewModel.SqlService.ValidateConnection(sqlServerServer.Text))
            {
                SetSqlServerSettings(sqlServerServer.Text);
                _viewModel.SQLServerConnection.Server = sqlServerServer.Text;
                MessageBox.Show("Conexão com o SQL Server estabelecida.");
            }
            else
            {
                MessageBox.Show("Falha ao conectar ao SQL Server. Verifique o servidor informado.");
            }
        }
        private void SalvarButton_Click(object sender, RoutedEventArgs e)
        {
            SetSqlServerSettings(sqlServerServer.Text);
        }
        private void VoltarButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;

            mainWindow.MainFramePublic.Navigate(new MainPage(_viewModel));

        }


    }
}
