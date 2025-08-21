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

        public void SetSelectedDb(ObservableCollection<SysDirectory> hist, int id)
        {
            if (hist.Any(i => i.SelectedBase >= 0))
                hist.FirstOrDefault(i => i.SelectedBase >= 0).selectedBase = -1;

            SelectedBase = id;
            OnPropertyChanged(nameof(SelectedBase));
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
