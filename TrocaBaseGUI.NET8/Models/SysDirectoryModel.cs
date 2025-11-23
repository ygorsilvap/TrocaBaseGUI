using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TrocaBaseGUI.Models
{
    [Serializable]
    public class SysDirectoryModel : INotifyPropertyChanged
    {
        public SysDirectoryModel() { }

        public SysDirectoryModel(string address, string fullPathAddress, string exeFile, ObservableCollection<string> exeList, int selectedBase = -1)
        {
            Folder = address;
            Path = fullPathAddress;
            MainExeFile = exeFile;
            ExeList = exeList;
            SelectedBase = selectedBase;
        }

        private int id;
        public int Id
        {
            get => id;
            set
            { id = value; OnPropertyChanged(); }
        }

        private string folder;
        public string Folder
        {
            get => folder;
            set
            { folder = value; OnPropertyChanged(); }
        }

        private string path;
        public string Path
        {
            get => path;
            set
            { path = value; OnPropertyChanged(); }
        }

        //private int tier;
        //public int Tier
        //{
        //    get => tier;
        //    set
        //    { tier = value; OnPropertyChanged(); }
        //}

        private string mainExeFile;
        public string MainExeFile
        {
            get => mainExeFile;
            set
            { mainExeFile = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> exeList;
        public ObservableCollection<string> ExeList
        {
            get => exeList;
            set
            { exeList = value; OnPropertyChanged(); }
        }

        private int selectedBase = -1;
        public int SelectedBase
        {
            get => selectedBase;
            set
            { selectedBase = value; OnPropertyChanged(); }
        }

        //public static SysDirectoryModel GetDir(ObservableCollection<SysDirectoryModel> hist, string addr)
        //{
        //    if (string.IsNullOrEmpty(addr) || hist.Count < 1)
        //        return null;

        //    return hist.FirstOrDefault(d => d.Path.EndsWith(addr));
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public override string ToString()
        {
            return Folder ?? Path;
        }
    }
}
