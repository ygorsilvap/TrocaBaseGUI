using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TrocaBaseGUI.Models;

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

        public int GetTier(string path)
        {
            if (File.Exists(Path.Combine(path, "ConexaoServidor.dat")))
            {
                return 3;
            }
            else if(File.Exists(Path.Combine(path, "conexao.dat")))
            {
                return 2;
            }
            else
            {
                return 0;
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

            //Debug.WriteLine($"Caminho do arquivo de conexão: {ConexaoFile}");
        }

        public string CreatePortsSettings(ConexaoFileModel conexao)
        {
            string settings = string.Empty;

            foreach(var appPorts in conexao.Ports)
            {
                settings += $"[PORTA]={appPorts.App.ToUpper().Replace(" ", "")}:{appPorts.Port}\n";
            }
            //Debug.WriteLine($"\n\n'{settings}'\n\n");
            return settings;
        }

        public string CreateConnectionFileSettings(ConexaoFileModel conexao, AppParams appParams)
        {
            if (conexao.Tier == 2)
            {
                string loginPadrao = string.IsNullOrEmpty(conexao.DefaultLogin) || !appParams.DefaultLoginCheckbox ? string.Empty : $"[LOGINPADRAO]={conexao.DefaultLogin}\n";
                string senhaPadrao = string.IsNullOrEmpty(conexao.DefaultPassword) || !appParams.DefaultPasswordCheckbox ? string.Empty : $"[SENHAPADRAO]={conexao.DefaultPassword}\n";
                string editorTexto = string.IsNullOrEmpty(conexao.TextEditorPath) || !appParams.EditorCheckbox ? string.Empty : $"[DIRATUALIZACAO]={conexao.TextEditorPath}\n";
                string updateFolder = string.IsNullOrEmpty(conexao.UpdateFolder) || !appParams.DirUpdateCheckbox ? string.Empty : $"[EDITOR]={conexao.UpdateFolder}\n";
                string useWebMenu = conexao.UseWebMenu ? $"[ABRIR_MENUSWEB_NODESKTOP]=S" : $"[ABRIR_MENUSWEB_NODESKTOP]=N";
                string settings = string.Concat(loginPadrao, senhaPadrao, editorTexto, updateFolder, useWebMenu);

                Debug.WriteLine($"\n\n'2: {settings}'\n\n");

                return settings;
            }
            else
            {
                string ports = CreatePortsSettings(conexao);
                string verifierPort = string.IsNullOrEmpty(conexao.VerifierPort) ? string.Empty : $"[PORTA_VERIFICADOR]={conexao.VerifierPort}\n";

                string loginPadrao = string.IsNullOrEmpty(conexao.DefaultLogin) || !appParams.DefaultLoginCheckbox ? string.Empty : $"[LOGINPADRAO]={conexao.DefaultLogin}\n";
                string senhaPadrao = string.IsNullOrEmpty(conexao.DefaultPassword) || !appParams.DefaultPasswordCheckbox ? string.Empty : $"[SENHAPADRAO]={conexao.DefaultPassword}\n";
                string editorTexto = string.IsNullOrEmpty(conexao.TextEditorPath) || !appParams.EditorCheckbox ? string.Empty : $"[DIRATUALIZACAO]={conexao.TextEditorPath}\n";
                string updateFolder = string.IsNullOrEmpty(conexao.UpdateFolder) || !appParams.DirUpdateCheckbox ? string.Empty : $"[EDITOR]={conexao.UpdateFolder}\n";
                string useWebMenu = conexao.UseWebMenu ? $"[ABRIR_MENUSWEB_NODESKTOP]=S" : $"[ABRIR_MENUSWEB_NODESKTOP]=N";


                string settings = string.Concat(ports, verifierPort, loginPadrao, senhaPadrao, editorTexto, updateFolder, useWebMenu);

                Debug.WriteLine($"\n\n'3: {settings}'\n\n");

                return settings;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
