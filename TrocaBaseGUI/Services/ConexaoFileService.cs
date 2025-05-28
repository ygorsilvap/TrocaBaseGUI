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
                domain = value;
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
            if (db.ToLower().Contains("sqlserver"))
            {
                return $"[BANCODADOS]=SQLSERVER\n[DATABASE]={domain}:{db.ToUpper()}";
            }
            else
            {
                return $"[BANCODADOS]=ORACLE\n[DATABASE]={domain}/LINX\n[USUARIO_ORACLE]={db.ToUpper()}";
            }
        }

        //public void WriteConnectionToFile(string conexaoPath, string connectionString)
        //{
        //    var lines = File.ReadAllLines(conexaoPath).ToList();
        //    int bancoIndex = lines.FindIndex(line => line.IndexOf("[BANCODADOS]", StringComparison.OrdinalIgnoreCase) >= 0);

        //    if (bancoIndex >= 0)
        //    {
        //        lines.RemoveRange(bancoIndex, lines.Count - bancoIndex);
        //    }
        //    else if (!string.IsNullOrWhiteSpace(lines.LastOrDefault()))
        //    {
        //        lines.Add(""); lines.Add("");
        //    }

        //    lines.AddRange(connectionString.Split('\n'));
        //    File.WriteAllLines(conexaoPath, lines);
        //}

        //public bool ValidateConexaoFile(string path)
        //{
        //    return File.Exists(Path.Combine(path, "conexao.dat"));
        //}

        public Boolean ValidateSystemPath(string path)
        {
            return File.Exists(path + "\\conexao.dat") ? true : false;
        }

        public void SetConexaoAddress(string add)
        {
            ConexaoFile = add + @"\conexao.dat";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
