using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Models
{
    public class IISServiceModel : INotifyPropertyChanged
    {
        public IISServiceModel(string serviceName, string port) 
        {
            ServiceName = serviceName;
            Port = port;
        }

        private string serviceName;
        public string ServiceName
        {
            get => serviceName;
            set
            { serviceName = value; OnPropertyChanged(); }
        }

        private string port;
        public string Port
        {
            get => port;
            set
            { port = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
