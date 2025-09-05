using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using TrocaBaseGUI.ViewModels;


namespace TrocaBaseGUI.Views
{
    public partial class MainWindow : Window
    {
        //public MainViewModel MainVM { get; private set; }
        public MainViewModel viewModel;
        public Frame MainFramePublic => MainFrame;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = MainViewModel.Instance;
            DataContext = viewModel;

            MainFrame.Navigate(new MainPage(viewModel));
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewModel.appStateService.SaveState(viewModel);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            viewModel.appStateService.SaveState(viewModel);
            Application.Current.Shutdown();
        }

        private void MinApp_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(Application.Current.MainWindow);
        }
    }
}