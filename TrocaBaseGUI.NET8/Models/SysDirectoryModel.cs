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
            SysDatabase = selectedBase;
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

        private int tier;
        public int Tier
        {
            get => tier;
            set
            { tier = value; OnPropertyChanged(); }
        }

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

        private int sysDatabase = -1;
        public int SysDatabase
        {
            get => sysDatabase;
            set
            { sysDatabase = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public override string ToString()
        {
            return Folder ?? Path;
        }
    }
}
