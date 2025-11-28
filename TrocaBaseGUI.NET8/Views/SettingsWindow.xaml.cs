using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
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
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            viewModel.appStateService.SaveState(viewModel);
            Application.Current.Shutdown();
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
    }
}
