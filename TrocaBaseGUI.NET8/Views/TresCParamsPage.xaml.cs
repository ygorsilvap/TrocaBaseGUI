using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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
            //SetParams(_viewModel.Conexao3Camadas);
        }

        private void loginCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.Settings.DefaultLoginCheckbox = (bool)loginCheckbox.IsChecked;
            loginPadrao.IsEnabled = _viewModel.appState.Settings.DefaultLoginCheckbox;
        }

        private void senhaCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.Settings.DefaultPasswordCheckbox = (bool)senhaCheckbox.IsChecked;
            senhaPadrao.IsEnabled = _viewModel.appState.Settings.DefaultPasswordCheckbox;
        }

        private void editorCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.Settings.EditorCheckbox = (bool)editorCheckbox.IsChecked;
            editorTexto.IsEnabled = _viewModel.appState.Settings.EditorCheckbox;
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

                _viewModel.appState.Settings.UpdateFolder = textEditorPath;

            }
        }

        private void updateFolderCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.Settings.DirUpdateCheckbox = (bool)updateFolderCheckbox.IsChecked;
            updateFolder.IsEnabled = _viewModel.appState.Settings.DirUpdateCheckbox;
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

                _viewModel.appState.Settings.UpdateFolder = updateFolderPath;
            }
        }

        private void SetParams()
        {
            //Debug.WriteLine($"\n\n3STGloginPadrao: {_viewModel.appState.DefaultLoginCheckbox}\n\n");
            loginCheckbox.IsChecked = _viewModel.appState.Settings.DefaultLoginCheckbox || !string.IsNullOrEmpty(_viewModel.appState.Settings.DefaultLogin);
            loginPadrao.IsEnabled = (bool)loginCheckbox.IsChecked;

            senhaCheckbox.IsChecked = _viewModel.appState.Settings.DefaultPasswordCheckbox || !string.IsNullOrEmpty(_viewModel.appState.Settings.DefaultPassword);
            senhaPadrao.IsEnabled = (bool)senhaCheckbox.IsChecked;

            editorCheckbox.IsChecked = _viewModel.appState.Settings.EditorCheckbox || !string.IsNullOrEmpty(_viewModel.appState.Settings.TextEditorPath);
            editorTexto.IsEnabled = (bool)editorCheckbox.IsChecked;

            updateFolderCheckbox.IsChecked = _viewModel.appState.Settings.DirUpdateCheckbox || !string.IsNullOrEmpty(_viewModel.appState.Settings.UpdateFolder);
            updateFolder.IsEnabled = (bool)updateFolderCheckbox.IsChecked;

            usaConciliadorCheckbox.IsChecked = _viewModel.appState.Settings.UsaConciliador;

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
