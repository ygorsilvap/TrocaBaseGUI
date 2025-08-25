using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TrocaBaseGUI.Models
{
    [Serializable]
    public class SysDirectory : INotifyPropertyChanged
    {
        private string folder;
        public string Folder
        {
            get => folder;
            set
            {
                folder = value;
                OnPropertyChanged(nameof(Folder));
            }
        }

        private string path;
        public string Path
        {
            get => path;
            set
            {
                path = value;
                OnPropertyChanged(nameof(Path));
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

        private ObservableCollection<string> exeList;
        public ObservableCollection<string> ExeList
        {
            get => exeList;
            set
            {
                exeList = value;
                OnPropertyChanged(nameof(ExeList));
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


        public SysDirectory(string address, string fullPathAddress, string exeFile, ObservableCollection<string> exeList, int selectedBase = -1)
        {
            Folder = address;
            Path = fullPathAddress;
            ExeFile = exeFile;
            ExeList = exeList;
            SelectedBase = selectedBase;
        }

        public static SysDirectory GetDir(ObservableCollection<SysDirectory> hist, string addr)
        {
            if (string.IsNullOrEmpty(addr) || hist.Count < 1)
                return null;

            return hist.FirstOrDefault(d => d.Path.EndsWith(addr));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString()
        {
            return Folder ?? Path;
        }
    }
}
