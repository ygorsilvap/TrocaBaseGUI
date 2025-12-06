using System.Diagnostics;
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (WindowStartupLocation.Equals(WindowStartupLocation.CenterScreen))
            {
                Properties.Settings.Default.WindowLeft = Left;
                Properties.Settings.Default.WindowTop = Top;
                //Properties.Settings.Default.Save();

                WindowStartupLocation = WindowStartupLocation.Manual;
            } else
            {
                Left = Properties.Settings.Default.WindowLeft;
                Top = Properties.Settings.Default.WindowTop;
            }
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
                this.DragMove();
                Properties.Settings.Default.WindowLeft = this.Left;
                Properties.Settings.Default.WindowTop = this.Top;
            }
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