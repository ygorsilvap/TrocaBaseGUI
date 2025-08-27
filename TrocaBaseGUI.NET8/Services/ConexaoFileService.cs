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

        private string conexaoFilePath;
        public string ConexaoFilePath
        {
            get => conexaoFilePath;
            set
            {
                conexaoFilePath = value;
                OnPropertyChanged(nameof(ConexaoFilePath));
            }
        }

        public Boolean ValidateSystemPath(string path)
        {
            //Tratar do case das strings para ignorar case
            return File.Exists(path + "\\conexao.dat") || File.Exists(path + "\\ConexaoServidor.dat");
        }

        public void SetConexaoAddress(string add)
        {
            //if (String.IsNullOrEmpty(add) || ValidateSystemPath(add)) return;

            //ConexaoFile = Path.Combine(add, "conexao.dat");
            //ConexaoFilePath = add;

            if (String.IsNullOrEmpty(add)) return;
            if (ValidateSystemPath(add))
            {
                ConexaoFile = Path.Combine(add, "conexao.dat");
                ConexaoFilePath = add;
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
