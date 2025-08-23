using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TrocaBaseGUI.Models
{
    [Serializable]
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

        private int selectedBase = -1;
        public int SelectedBase
        {
            get => selectedBase;
            set
            {
                selectedBase = value;
                OnPropertyChanged(nameof(SelectedBase));
            }
        }


        public SysDirectory(string address, string fullPathAddress, string exeFile, int selectedBase = -1)
        {
            Address = address;
            FullPathAddress = fullPathAddress;
            ExeFile = exeFile;
            SelectedBase = selectedBase;
        }

        public static SysDirectory GetDir(ObservableCollection<SysDirectory> hist, string addr)
        {
            if (string.IsNullOrEmpty(addr))
                return null;

            return hist.FirstOrDefault(d => d.FullPathAddress.EndsWith(addr));
        }

        //public void SetSelectedDb(ObservableCollection<SysDirectory> hist, string addr, int id)
        //{
        //    if (hist == null || string.IsNullOrEmpty(addr))
        //        return;

        //    SysDirectory.GetDir(hist, addr).SelectedBase = id;
        //    OnPropertyChanged(nameof(SelectedBase));
        //}

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
