using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TrocaBaseGUI.ViewModels;
using TrocaBaseGUI.Views;


namespace TrocaBaseGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string mutexName = "TrocaBaseGUI";

            bool createdNew;
            mutex = new Mutex(true, mutexName, out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("A aplicação já está em execução.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }
    }
}
