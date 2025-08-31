using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
            _viewModel.appState.DefaultLoginCheckbox = (bool)loginCheckbox.IsChecked;
            loginPadrao.IsEnabled = _viewModel.appState.DefaultLoginCheckbox;
        }
        private void loginPadrao_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao2Camadas.DefaultLogin = loginPadrao.Text;
        }

        private void senhaCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.DefaultPasswordCheckbox = (bool)senhaCheckbox.IsChecked;
            senhaPadrao.IsEnabled = _viewModel.appState.DefaultPasswordCheckbox;
        }
        private void senhaPadrao_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao2Camadas.DefaultPassword = senhaPadrao.Text;
        }

        private void editorCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.EditorCheckbox = (bool)editorCheckbox.IsChecked;
            editorTexto.IsEnabled = _viewModel.appState.EditorCheckbox;
        }
        private void editorTexto_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao2Camadas.TextEditorPath = editorTexto.Text;
        }

        private void dirCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.appState.DirUpdateCheckbox = (bool)dirCheckbox.IsChecked;
            dirAtualizacao.IsEnabled = _viewModel.appState.DirUpdateCheckbox;
        }
        private void dirAtualizacao_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Conexao2Camadas.UpdateFolder = dirAtualizacao.Text;
        }

        private void ultMenuWebCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            //SOLUÇÃO TEMPORÁRIA TALVEZ MUDAR O WEBMENU PARA BOOLEAN
            _viewModel.Conexao2Camadas.UseWebMenu = (bool)ultMenuWebCheckbox.IsChecked ? "S" : "N";
        }


        //Possível redundância a tratar já que o binding está funcionando
        private void SetParams()
        {
            //Debug.WriteLine($"\n\n3STGloginPadrao: {_viewModel.appState.DefaultLoginCheckbox}\n\n");
            loginCheckbox.IsChecked = _viewModel.appState.DefaultLoginCheckbox || !string.IsNullOrEmpty(_viewModel.Conexao2Camadas.DefaultLogin);
            loginPadrao.IsEnabled = (bool)loginCheckbox.IsChecked;

            senhaCheckbox.IsChecked = _viewModel.appState.DefaultPasswordCheckbox || !string.IsNullOrEmpty(_viewModel.Conexao2Camadas.DefaultPassword);
            senhaPadrao.IsEnabled = (bool)senhaCheckbox.IsChecked;

            editorCheckbox.IsChecked = _viewModel.appState.EditorCheckbox || !string.IsNullOrEmpty(_viewModel.Conexao2Camadas.TextEditorPath);
            editorTexto.IsEnabled = (bool)editorCheckbox.IsChecked;

            dirCheckbox.IsChecked = _viewModel.appState.DirUpdateCheckbox || !string.IsNullOrEmpty(_viewModel.Conexao2Camadas.UpdateFolder);
            dirAtualizacao.IsEnabled = (bool)dirCheckbox.IsChecked;

            //SOLUÇÃO TEMPORÁRIA TALVEZ MUDAR O WEBMENU PARA BOOLEAN
            ultMenuWebCheckbox.IsChecked = _viewModel.Conexao2Camadas.UseWebMenu == "S" ? true : false;
        }
    }
}
