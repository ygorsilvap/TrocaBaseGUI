using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TrocaBaseGUI.ViewModels;


namespace TrocaBaseGUI.Views
{
    public partial class MainWindow : Window
    {
        public MainViewModel MainVM { get; private set; }
        public Frame MainFramePublic => MainFrame;
        public MainWindow()
        {
            InitializeComponent();

            MainVM = new MainViewModel();
            MainFrame.Navigate(new MainPage(MainVM));
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainVM.SaveState();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}