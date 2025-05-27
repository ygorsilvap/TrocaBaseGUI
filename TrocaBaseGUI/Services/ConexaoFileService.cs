using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Services
{
    public class ConexaoFileService
    {
        private readonly string domain = "MTZNOTFS058680.linx-inves.com.br";
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
        public void WriteConnectionToFile(string conexaoPath, string connectionString)
        {
            var lines = File.ReadAllLines(conexaoPath).ToList();
            int bancoIndex = lines.FindIndex(line => line.IndexOf("[BANCODADOS]", StringComparison.OrdinalIgnoreCase) >= 0);

            if (bancoIndex >= 0)
            {
                lines.RemoveRange(bancoIndex, lines.Count - bancoIndex);
            }
            else if (!string.IsNullOrWhiteSpace(lines.LastOrDefault()))
            {
                lines.Add(""); lines.Add("");
            }

            lines.AddRange(connectionString.Split('\n'));
            File.WriteAllLines(conexaoPath, lines);
        }

        public bool ValidateConexaoFile(string path)
        {
            return File.Exists(Path.Combine(path, "conexao.dat"));
        }
    }
}
