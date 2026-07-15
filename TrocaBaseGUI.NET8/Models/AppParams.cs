using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Models
{
    public class AppParams : INotifyPropertyChanged
    {
        private bool defaultLoginCheckbox;
        public bool DefaultLoginCheckbox
        {
            get => defaultLoginCheckbox;
            set
            { defaultLoginCheckbox = value; OnPropertyChanged(); }
        }

        private string defaultLogin;
        public string DefaultLogin
        {
            get => defaultLogin;
            set
            { defaultLogin = value; OnPropertyChanged(); }
        }

        private bool defaultPasswordCheckbox;
        public bool DefaultPasswordCheckbox
        {
            get => defaultPasswordCheckbox;
            set
            { defaultPasswordCheckbox = value; OnPropertyChanged(); }
        }

        private string defaultPassword;
        public string DefaultPassword
        {
            get => defaultPassword;
            set
            { defaultPassword = value; OnPropertyChanged(); }
        }

        private bool editorCheckbox;
        public bool EditorCheckbox
        {
            get => editorCheckbox;
            set
            { editorCheckbox = value; OnPropertyChanged(); }
        }

        private string textEditorPath;
        public string TextEditorPath
        {
            get => textEditorPath;
            set
            { textEditorPath = value; OnPropertyChanged(); }
        }

        private bool dirUpdateCheckbox;
        public bool DirUpdateCheckbox
        {
            get => dirUpdateCheckbox;
            set
            { dirUpdateCheckbox = value; OnPropertyChanged(); }
        }

        private string updateFolder;
        public string UpdateFolder
        {
            get => updateFolder;
            set
            { updateFolder = value; OnPropertyChanged(); }
        }

        private bool usaConciliador;
        public bool UsaConciliador
        {
            get => usaConciliador;
            set
            { usaConciliador = value; OnPropertyChanged(); }
        }

        //private bool useWebMenuCheckbox;
        //public bool UseWebMenuCheckbox
        //{
        //    get => useWebMenuCheckbox;
        //    set
        //    { useWebMenuCheckbox = value; OnPropertyChanged(); }
        //}

        //private bool useRedirectCheckbox;
        //public bool UseRedirectCheckbox
        //{
        //    get => useRedirectCheckbox;
        //    set
        //    { useRedirectCheckbox = value; OnPropertyChanged(); }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
