using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

        private string domain = "MTZNOTFS058680.linx-inves.com.br";
        public string Domain
        {
            get => domain;
            set
            {
                domain = value;
                OnPropertyChanged(nameof(Domain));
            }
        }

        public string CreateConnectionString(string dbType, string domain, string db)
        {
            if (dbType.ToLower().Contains("sqlserver"))
            {
                return $"[BANCODADOS]=SQLSERVER\n[DATABASE]={domain}:{db.ToUpper()}";
            }
            else
            {
                return $"[BANCODADOS]=ORACLE\n[DATABASE]={domain}/LINX\n[USUARIO_ORACLE]={db.ToUpper()}";
            }
        }

        public Boolean ValidateSystemPath(string path)
        {
            return File.Exists(path + "\\conexao.dat") ? true : false;
        }

        public void SetConexaoAddress(string add)
        {
            if (String.IsNullOrEmpty(add)) return;
            if (ValidateSystemPath(add))
            {
                ConexaoFile = Path.Combine(add, "conexao.dat");
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
