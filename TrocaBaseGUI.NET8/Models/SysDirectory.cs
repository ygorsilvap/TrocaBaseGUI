using System.Collections.ObjectModel;
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

        private string exeFile;
        public string ExeFile
        {
            get => exeFile;
            set
            {
                exeFile = value;
                OnPropertyChanged(nameof(ExeFile));
            }
        }

        //private ObservableCollection<SysDirectory> history;
        //public ObservableCollection<SysDirectory> History
        //{
        //    get => history;
        //    set
        //    {
        //        history = value;
        //        OnPropertyChanged(nameof(History));
        //    }
        //}


        public SysDirectory(string address, string fullPathAddress, string exeFile)
        {
            Address = address;
            FullPathAddress = fullPathAddress;
            ExeFile = exeFile;
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
