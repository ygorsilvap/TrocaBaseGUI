using System.ComponentModel;

namespace TrocaBaseGUI.Models
{
    public class Banco : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string dbType;
        public string DbType
        {
            get => dbType;
            set
            {
                dbType = value;
                OnPropertyChanged(nameof(DbType));
            }
        }

        private string fileName;
        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        private string instance;
        public string Instance
        {
            get => instance;
            set
            {
                instance = value;
                OnPropertyChanged(nameof(Instance));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString()
        {
             return Name; // Para mostrar o nome no ListBox diretamentes
        }
    }
}
