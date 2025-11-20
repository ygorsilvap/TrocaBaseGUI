using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using TrocaBaseGUI.Models;
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
            DataContext = _viewModel;
            SetParams();
        }

        private void loginCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.ServerParams.DefaultLoginCheckbox = (bool)loginCheckbox.IsChecked;
            loginPadrao.IsEnabled = _viewModel.appState.ServerParams.DefaultLoginCheckbox;
        }
        private void loginPadrao_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao3Camadas.DefaultLogin = loginPadrao.Text;
        }

        private void senhaCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.ServerParams.DefaultPasswordCheckbox = (bool)senhaCheckbox.IsChecked;
            senhaPadrao.IsEnabled = _viewModel.appState.ServerParams.DefaultPasswordCheckbox;
        }
        private void senhaPadrao_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao3Camadas.DefaultPassword = senhaPadrao.Text;
        }

        private void editorCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.ServerParams.EditorCheckbox = (bool)editorCheckbox.IsChecked;
            editorTexto.IsEnabled = _viewModel.appState.ServerParams.EditorCheckbox;
        }
        private void editorTexto_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao3Camadas.TextEditorPath = editorTexto.Text;
        }
        private void SelectTextEditorPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Selecione o executável do editor de texto.",
                InitialDirectory = @"C:\",
                Filters = { new CommonFileDialogFilter("Executáveis", "*.exe") }
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok && File.Exists(dialog.FileName))
            {
                string textEditorPath = Path.GetFullPath(dialog.FileName);

                editorTexto.Text = textEditorPath;
            }
        }

        private void updateFolderCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.ServerParams.DirUpdateCheckbox = (bool)updateFolderCheckbox.IsChecked;
            updateFolder.IsEnabled = _viewModel.appState.ServerParams.DirUpdateCheckbox;
        }
        private void updateFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao3Camadas.UpdateFolder = updateFolder.Text;
        }

        private void SelectUpdateFolderPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Selecione a pasta de atualização do sistema.",
                InitialDirectory = @"C:\",
                IsFolderPicker = true
            };


            if (dialog.ShowDialog() == CommonFileDialogResult.Ok && Path.Exists(dialog.FileName))
            {
                string updateFolderPath = dialog.FileName;

                updateFolder.Text = updateFolderPath;
            }
        }

        private void ultMenuWebCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.Conexao3Camadas.UseWebMenu = (bool)ultMenuWebCheckbox.IsChecked;
        }

        private void useRedirectCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.Conexao3Camadas.UseRedirect = (bool)useRedirectCheckbox.IsChecked;
        }

        private void redirectPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao3Camadas.RedirectPort = redirectPort.Text;
        }

        private void verifierPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao3Camadas.VerifierPort = verifierPort.Text;
        }

        private void dbServer_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao3Camadas.DbServer = dbServer.Text;
        }

        private void SetParams()
        {
            //Debug.WriteLine($"\n\n3STGloginPadrao: {_viewModel.appState.DefaultLoginCheckbox}\n\n");
            loginCheckbox.IsChecked = _viewModel.appState.ServerParams.DefaultLoginCheckbox || !string.IsNullOrEmpty(_viewModel.Conexao3Camadas.DefaultLogin);
            loginPadrao.IsEnabled = (bool)loginCheckbox.IsChecked;

            senhaCheckbox.IsChecked = _viewModel.appState.ServerParams.DefaultPasswordCheckbox || !string.IsNullOrEmpty(_viewModel.Conexao3Camadas.DefaultPassword);
            senhaPadrao.IsEnabled = (bool)senhaCheckbox.IsChecked;

            editorCheckbox.IsChecked = _viewModel.appState.ServerParams.EditorCheckbox || !string.IsNullOrEmpty(_viewModel.Conexao3Camadas.TextEditorPath);
            editorTexto.IsEnabled = (bool)editorCheckbox.IsChecked;

            updateFolderCheckbox.IsChecked = _viewModel.appState.ServerParams.DirUpdateCheckbox || !string.IsNullOrEmpty(_viewModel.Conexao3Camadas.UpdateFolder);
            updateFolder.IsEnabled = (bool)updateFolderCheckbox.IsChecked;

            ultMenuWebCheckbox.IsChecked = _viewModel.Conexao3Camadas.UseWebMenu;

            useRedirectCheckbox.IsChecked = _viewModel.Conexao3Camadas.UseRedirect || !string.IsNullOrEmpty(_viewModel.Conexao3Camadas.RedirectPort);
        }

        private void SetPortsButton_Click(object sender, RoutedEventArgs e)
        {
            var owner = Window.GetWindow(this) ?? Application.Current.MainWindow;

            var dlg = new PortsWindow(_viewModel.Conexao3Camadas.Ports)
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dlg.ShowDialog();
        }

        private void updateRedirectorFile_Click(object sender, RoutedEventArgs e)
        {
            

            if(string.IsNullOrEmpty(_viewModel.Conexao3Camadas.RedirectPort))
            {
                MessageBox.Show("O campo 'Porta Redirecionador' não pode ficar vazio.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _viewModel.conexaoFileService.UpdateRedirectorFile(_viewModel.conexaoRedirecionadorFile, _viewModel.Conexao3Camadas);
        }
    }
}
