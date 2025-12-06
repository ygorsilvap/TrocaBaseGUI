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

        private void senhaCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.ServerParams.DefaultPasswordCheckbox = (bool)senhaCheckbox.IsChecked;
            senhaPadrao.IsEnabled = _viewModel.appState.ServerParams.DefaultPasswordCheckbox;
        }

        private void editorCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.ServerParams.EditorCheckbox = (bool)editorCheckbox.IsChecked;
            editorTexto.IsEnabled = _viewModel.appState.ServerParams.EditorCheckbox;
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

                _viewModel.Conexao3Camadas.UpdateFolder = textEditorPath;
            }
        }

        private void updateFolderCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.ServerParams.DirUpdateCheckbox = (bool)updateFolderCheckbox.IsChecked;
            updateFolder.IsEnabled = _viewModel.appState.ServerParams.DirUpdateCheckbox;
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

                _viewModel.Conexao3Camadas.UpdateFolder = updateFolderPath;
            }
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

        private void UpdateRedirectorFile_Click(object sender, RoutedEventArgs e)
        {
            var selectedDirectory = _viewModel.appState.SelectedFolder;
            if (string.IsNullOrEmpty(_viewModel.Conexao3Camadas.RedirectPort))
            {
                MessageBox.Show("O campo 'Porta Redirecionador' não pode ficar vazio.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            } else if (!_viewModel.appState.SelectedFolder.Tier.Equals(3))
            {
                MessageBox.Show("A pasta selecionada não contém um sistema 3 Camadas.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
                _viewModel.conexaoFileService.UpdateRedirectorFile(_viewModel.conexaoRedirecionadorFile, _viewModel.Conexao3Camadas);
        }
    }
}
