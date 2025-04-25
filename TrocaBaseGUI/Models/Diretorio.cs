using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Models
{
    public class SysDirectory : INotifyPropertyChanged
    {
        public string Address { get; set; }
        public string FullPathAddress { get; set; }

        public SysDirectory(string address, string fullPathAddress)
        {
            Address = address;
            FullPathAddress = fullPathAddress;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString()
        {
            return Address ?? FullPathAddress;
        }
    }
}
