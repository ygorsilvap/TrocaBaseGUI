using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Services
{
    public class ConexaoFileService : INotifyPropertyChanged
    {
        private string conexaoFile;
        public string ConexaoFile
        {
            get => conexaoFile;
            set
            {
                conexaoFile = value;
                OnPropertyChanged(nameof(ConexaoFile));
            }
        }

        private string conexaoAddress;
        public string ConexaoAddress
        {
            get => conexaoAddress;
            set
            {
                conexaoAddress = value;
                OnPropertyChanged(nameof(ConexaoAddress));
            }
        }

        private string domain = Dns.GetHostEntry(string.Empty).HostName;
        public string Domain
        {
            get => domain;
            set
            {
                domain = value;
                OnPropertyChanged(nameof(Domain));
            }
        }

        public Boolean ValidateSystemPath(string path)
        {
            return File.Exists(path + "\\conexao.dat") || File.Exists(path + "\\ConexaoServidor.dat") ? true : false;
        }

        public void SetConexaoAddress(string add)
        {
            if (String.IsNullOrEmpty(add)) return;
            if (ValidateSystemPath(add))
            {
                ConexaoFile = Path.Combine(add, "conexao.dat");
                ConexaoAddress = add;
            }
            else
            {
                Console.WriteLine("caminho inválido");
            }
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
