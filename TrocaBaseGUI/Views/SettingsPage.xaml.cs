using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class SettingsPage : Page
    {
        public MainViewModel _viewModel;
        public SettingsPage(MainViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            sqlServerServer.Text = _viewModel.SQLServerConnection.Server;
        }
        private void VoltarButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
