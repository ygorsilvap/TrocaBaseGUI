using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        //Refatorar 
        public string GetConexaoType(string path)
        {
            if (File.Exists($"{path}\\conexao.dat"))
            {
                return "conexao.dat";
            }
            else if (File.Exists(Path.Combine(path, "ConexaoServidor.dat")))
            {
                return "ConexaoServidor.dat";
            }
            else
            {
                return null;
            }
        }

        public void SetConexaoAddress(string path)
        {
            if (String.IsNullOrEmpty(path)) return;

            if (ValidateSystemPath(path))
            {
                //ConexaoFile = Path.Combine(path, "conexao.dat");
                ConexaoFile = Path.Combine(path, GetConexaoType(path));
                ConexaoFilePath = path;
            }
            else
            {
                Console.WriteLine("caminho inválido");
            }

            Debug.WriteLine($"Caminho do arquivo de conexão: {ConexaoFile}");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
