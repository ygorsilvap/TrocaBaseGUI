using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Models
{
    public class AppConn : INotifyPropertyChanged
    {
        public AppConn() { }
        public AppConn(string app = "", string port = "", string server = "")
        {
            App = app;
            Port = port;
            Server = server;
        }

        private string app;
        public string App
        {
            get => app;
            set
            { app = value; OnPropertyChanged(); }
        }
        private string port;
        public string Port
        {
            get => port;
            set
            { port = value; OnPropertyChanged(); }
        }

        private string server;
        public string Server
        {
            get => server;
            set
            { server = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
