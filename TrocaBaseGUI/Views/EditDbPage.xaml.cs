using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class EditDbPage : Page
    {
        private MainViewModel _mainViewModel;
        public EditDbPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _mainViewModel = viewModel;
        }

        private void SaveName_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFramePublic.Navigate(new MainPage());
        }
    }
}
