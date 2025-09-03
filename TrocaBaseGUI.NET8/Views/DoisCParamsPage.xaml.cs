using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class DoisCParamsPage : Page
    {
        public MainViewModel _viewModel;
        //public bool defaultLoginChecked;

        public DoisCParamsPage()
        {
            InitializeComponent();
            var mainWindow = (SettingsWindow)Application.Current.MainWindow;
            _viewModel = mainWindow.viewModel;
            DataContext = _viewModel;

            SetParams();
            //Debug.WriteLine($"\n\n2STGloginPadrao: {loginCheckbox.IsChecked} - {_viewModel.appState.DefaultLoginCheckbox}\n\n");

        }

        private void loginCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.LocalParams.DefaultLoginCheckbox = (bool)loginCheckbox.IsChecked;
            loginPadrao.IsEnabled = _viewModel.appState.LocalParams.DefaultLoginCheckbox;
        }
        private void loginPadrao_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao2Camadas.DefaultLogin = loginPadrao.Text;
        }

        private void senhaCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.LocalParams.DefaultPasswordCheckbox = (bool)senhaCheckbox.IsChecked;
            senhaPadrao.IsEnabled = _viewModel.appState.LocalParams.DefaultPasswordCheckbox;
        }
        private void senhaPadrao_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao2Camadas.DefaultPassword = senhaPadrao.Text;
        }

        private void editorCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.LocalParams.EditorCheckbox = (bool)editorCheckbox.IsChecked;
            editorTexto.IsEnabled = _viewModel.appState.LocalParams.EditorCheckbox;
        }
        private void editorTexto_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao2Camadas.TextEditorPath = editorTexto.Text;
        }

        private void updateFolderCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.LocalParams.DirUpdateCheckbox = (bool)updateFolderCheckbox.IsChecked;
            updateFolder.IsEnabled = _viewModel.appState.LocalParams.DirUpdateCheckbox;
        }
        private void updateFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao2Camadas.UpdateFolder = updateFolder.Text;
        }

        private void ultMenuWebCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.Conexao2Camadas.UseWebMenu = (bool)ultMenuWebCheckbox.IsChecked;
        }


        //Redundância a tratar já que o binding está funcionando
        private void SetParams()
        {
            //Debug.WriteLine($"\n\n3STGloginPadrao: {_viewModel.appState.DefaultLoginCheckbox}\n\n");
            loginCheckbox.IsChecked = _viewModel.appState.LocalParams.DefaultLoginCheckbox || !string.IsNullOrEmpty(_viewModel.Conexao2Camadas.DefaultLogin);
            loginPadrao.IsEnabled = (bool)loginCheckbox.IsChecked;

            senhaCheckbox.IsChecked = _viewModel.appState.LocalParams.DefaultPasswordCheckbox || !string.IsNullOrEmpty(_viewModel.Conexao2Camadas.DefaultPassword);
            senhaPadrao.IsEnabled = (bool)senhaCheckbox.IsChecked;

            editorCheckbox.IsChecked = _viewModel.appState.LocalParams.EditorCheckbox || !string.IsNullOrEmpty(_viewModel.Conexao2Camadas.TextEditorPath);
            editorTexto.IsEnabled = (bool)editorCheckbox.IsChecked;

            updateFolderCheckbox.IsChecked = _viewModel.appState.LocalParams.DirUpdateCheckbox || !string.IsNullOrEmpty(_viewModel.Conexao2Camadas.UpdateFolder);
            updateFolder.IsEnabled = (bool)updateFolderCheckbox.IsChecked;

            ultMenuWebCheckbox.IsChecked = _viewModel.Conexao2Camadas.UseWebMenu;
        }

        private void SelectTextEditorPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Selecione o executável do editor de texto.",
                InitialDirectory = @"C:\",
                Filters = { new CommonFileDialogFilter("Executáveis", "*.exe") }
            };
            
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok && File.Exists(dialog.FileName)) {
                string textEditorPath = Path.GetFullPath(dialog.FileName);

                editorTexto.Text = textEditorPath;
            }
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
    }
}
