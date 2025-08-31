using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class TresCParamsPage : Page
    {
        public MainViewModel _viewModel;
        public TresCParamsPage()
        {
            InitializeComponent();

            var mainWindow = (SettingsWindow)Application.Current.MainWindow;
            _viewModel = mainWindow.viewModel;
            SetParams();
        }

        private void loginCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            loginPadrao.IsEnabled = (bool)loginCheckbox.IsChecked;
        }

        private void senhaCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            senhaPadrao.IsEnabled = (bool)senhaCheckbox.IsChecked;
        }

        private void editorCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            editorTexto.IsEnabled = (bool)editorCheckbox.IsChecked;
        }

        private void dirCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            dirAtualizacao.IsEnabled = (bool)dirCheckbox.IsChecked;
        }
        private void ultMenuWebCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            ///////////////////////////////////////////////////////////
        }

        private void SetParams()
        {
            loginPadrao.IsEnabled = (bool)loginCheckbox.IsChecked;
            senhaPadrao.IsEnabled = (bool)senhaCheckbox.IsChecked;
            editorTexto.IsEnabled = (bool)editorCheckbox.IsChecked;
            dirAtualizacao.IsEnabled = (bool)dirCheckbox.IsChecked;
        }

        private void ServerPortEditButton_Click(object sender, RoutedEventArgs e)
        {
            var owner = Window.GetWindow(this) ?? Application.Current.MainWindow;

            var dlg = new ServerPortsWindow(_viewModel.Conexao3Camadas.ServerPorts)
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dlg.ShowDialog();
        }

        private void ClientPortEditButton_Click(object sender, RoutedEventArgs e)
        {
            var owner = Window.GetWindow(this) ?? Application.Current.MainWindow;

            var dlg = new ServerPortsWindow(_viewModel.Conexao3Camadas.ClientPorts)
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dlg.ShowDialog();
        }
    }
}
