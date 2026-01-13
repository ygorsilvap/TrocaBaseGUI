using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class SettingsWindow : Window
    {
        public MainViewModel viewModel;
        public int tabSelected;
        public Frame MainFramePublic => MainFrame;
        public SettingsWindow()
        {
            InitializeComponent();

            viewModel = MainViewModel.Instance;
            DataContext = viewModel;

            //solução do chatgpt
            viewModel.PropertyChanged += SetLoadingState;
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowStartupLocation = WindowStartupLocation.Manual;

            Left = Properties.Settings.Default.WindowLeft;
            Top = Properties.Settings.Default.WindowTop;
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            viewModel.appStateService.SaveState(viewModel);
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewModel.appStateService.SaveState(viewModel);
            Properties.Settings.Default.Save();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
                Properties.Settings.Default.WindowLeft = Left;
                Properties.Settings.Default.WindowTop = Top;
                //Properties.Settings.Default.Save();
            }
        }

        private void MinApp_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(Application.Current.MainWindow);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
                tabSelected = tabControl.SelectedIndex;

            if (!(Application.Current.MainWindow is SettingsWindow)) return;
            switch (tabSelected)
            {
                case 0:
                    ((SettingsWindow)Application.Current.MainWindow).MainFramePublic.Navigate(new LocalSettingsPage());
                    break;
                case 1:
                    ((SettingsWindow)Application.Current.MainWindow).MainFramePublic.Navigate(new ServerSettingsPage());
                    break;
                case 2:
                    ((SettingsWindow)Application.Current.MainWindow).MainFramePublic.Navigate(new DoisCParamsPage());
                    break;
                case 3:
                    ((SettingsWindow)Application.Current.MainWindow).MainFramePublic.Navigate(new TresCParamsPage());
                    break;
            }
        }

        private void SalvarVoltar_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow();

            Application.Current.MainWindow = main;
            main.Show();

            Window.GetWindow(this)?.Close();
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            var del = MessageBox.Show("Isso fará com que todos os dados da aplicação sejam deletados.\n\nDesejar continuar?", "Hard Reset",
                MessageBoxButton.YesNo, MessageBoxImage.Warning)
                .ToString().ToLower();

            if (del.Equals("yes"))
            {
                viewModel.ClearApp();
            }
        }

        //lógica minha, função acessada por código sugerido pelo gpt
        private void SetLoadingState(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.isSqlLoading) || e.PropertyName == nameof(MainViewModel.isOracleLoading))
            {
                if (viewModel.isSqlLoading || viewModel.isOracleLoading)
                {
                    VoltarButton.IsEnabled = false;
                    ClearButton.IsEnabled = false;
                    ConLocalTab.IsEnabled = false;
                    ConServerTab.IsEnabled = false;
                    DoisCTab.IsEnabled = false;
                    TresCTab.IsEnabled = false;
                }
                else if (!viewModel.isSqlLoading || !viewModel.isOracleLoading)
                {
                    VoltarButton.IsEnabled = true;
                    ClearButton.IsEnabled = true;
                    ConLocalTab.IsEnabled = true;
                    ConServerTab.IsEnabled = true;
                    DoisCTab.IsEnabled = true;
                    TresCTab.IsEnabled = true;
                }
            }
        }

    }
}
