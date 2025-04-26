using System.ComponentModel;

namespace TrocaBaseGUI.Models
{
    public class SysDirectory : INotifyPropertyChanged
    {
        private string address;
        public string Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        private string fullPathAddress;
        public string FullPathAddress
        {
            get => fullPathAddress;
            set
            {
                fullPathAddress = value;
                OnPropertyChanged(nameof(FullPathAddress));
            }
        }

        
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
