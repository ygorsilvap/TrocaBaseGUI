using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
            { conexaoFile = value; OnPropertyChanged();
            }
        }

        private string conexaoServidorFile;
        public string ConexaoServidorFile
        {
            get => conexaoServidorFile;
            set
            { conexaoServidorFile = value; OnPropertyChanged(); }
        }

        private string conexaoRedirecionadorFile;
        public string ConexaoRedirecionadorFile
        {
            get => conexaoRedirecionadorFile;
            set
            { conexaoRedirecionadorFile = value; OnPropertyChanged(); }
        }

        private string conexaoClienteFile;
        public string ConexaoClienteFile
        {
            get => conexaoClienteFile;
            set
            { conexaoClienteFile = value; OnPropertyChanged(); }
        }

        private string conexaoFilePath;
        public string ConexaoFilePath
        {
            get => conexaoFilePath;
            set
            { conexaoFilePath = value; OnPropertyChanged();
            }
        }

        public Boolean ValidateSystemPath(string path)
        {
            //Tratar do case das strings para ignorar case
            return File.Exists(path + "\\conexao.dat") || 
                   File.Exists(path + "\\ConexaoServidor.dat") &&
                   File.Exists(path + "\\ConexaoRedireciona.dat") &&
                   File.Exists(path + "\\ConexaoCliente.dat");
        }

        //Refatorar 
        //public string GetConexaoType(string path)
        //{
        //    if (File.Exists($"{path}\\conexao.dat"))
        //    {
        //        return "conexao.dat";
        //    }
        //    else if (File.Exists(Path.Combine(path, "ConexaoServidor.dat")))
        //    {
        //        return "ConexaoServidor.dat";
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

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
                int tier = GetTier(path);
                if (tier > 2) 
                {
                    //ConexaoFile = Path.Combine(path, GetConexaoType(path));
                    ConexaoClienteFile = Path.Combine(path, "ConexaoCliente.dat");
                    ConexaoRedirecionadorFile = Path.Combine(path, "ConexaoRedireciona.dat");
                    ConexaoServidorFile = Path.Combine(path, "ConexaoServidor.dat");
                    ConexaoFilePath = path;
                }
                else { 

                    //ConexaoFile = Path.Combine(path, GetConexaoType(path));
                    ConexaoFile = Path.Combine(path, "conexao.dat");
                }

            }
            else
            {
                Console.WriteLine("caminho inválido");
            }
    
            //Debug.WriteLine($"Caminho do arquivo de conexão: {ConexaoFile}");
        }

        public string CreateServerPortsSettings(ConexaoFileModel conexao)
        {
            string settings = string.Empty;

            foreach (var appPorts in conexao.Ports)
            {
                settings += $"[PORTA]={appPorts.App.ToUpper().Replace(" ", "")}:{appPorts.Port}\n";
            }
            //Debug.WriteLine($"\n\n'{settings}'\n\n");
            return settings;
        }

        public string CreateClientPortsSettings(ConexaoFileModel conexao)
        {
            string settings = string.Empty;

            if (conexao.UseRedirect)
            {
                foreach (var appPorts in conexao.Ports)
                {
                    settings += $"[DATABASE]={appPorts.App.ToUpper().Replace(" ", "")}:{conexao.DbServer}:{conexao.RedirectPort}\n";
                }
                //Debug.WriteLine($"\n\n'{settings}'\n\n");
                return settings;
            }

            foreach (var appPorts in conexao.Ports)
            {
                settings += $"[DATABASE]={appPorts.App.ToUpper().Replace(" ", "")}:{conexao.DbServer}:{appPorts.Port}\n";
            }
            //Debug.WriteLine($"\n\n'{settings}'\n\n");
            return settings;
        }

        public string CreateRedirectPortsSettings(ConexaoFileModel conexao)
        {
            string settings = string.Empty;

            foreach (var appPorts in conexao.Ports)
            {
                settings += $"[DATABASE]={appPorts.App.ToUpper().Replace(" ", "")}:{conexao.DbServer}:{appPorts.Port}\n";
            }
            //Debug.WriteLine($"\n\n'{settings}'\n\n");
            return settings;
        }

        public string Create2CConnectionFileSettings(ConexaoFileModel conexao, AppParams appParams)
        {
            string loginPadrao = string.IsNullOrEmpty(conexao.DefaultLogin) || !appParams.DefaultLoginCheckbox ? string.Empty : $"[LOGINPADRAO]={conexao.DefaultLogin}\n";
            string senhaPadrao = string.IsNullOrEmpty(conexao.DefaultPassword) || !appParams.DefaultPasswordCheckbox ? string.Empty : $"[SENHAPADRAO]={conexao.DefaultPassword}\n";
            string editorTexto = string.IsNullOrEmpty(conexao.TextEditorPath) || !appParams.EditorCheckbox ? string.Empty : $"[DIRATUALIZACAO]={conexao.TextEditorPath}\n";
            string updateFolder = string.IsNullOrEmpty(conexao.UpdateFolder) || !appParams.DirUpdateCheckbox ? string.Empty : $"[EDITOR]={conexao.UpdateFolder}\n";
            string useWebMenu = conexao.UseWebMenu ? $"[ABRIR_MENUSWEB_NODESKTOP]=S" : $"[ABRIR_MENUSWEB_NODESKTOP]=N";
            string settings = string.Concat(loginPadrao, senhaPadrao, editorTexto, updateFolder, useWebMenu);

            //Debug.WriteLine($"\n\n'2: {settings}'\n\n");

            return settings;
        }

        public string Create3CConnectionServerFileSettings(ConexaoFileModel conexao, AppParams appParams)
        {
            string ports = CreateServerPortsSettings(conexao);
            string verifierPort = string.IsNullOrEmpty(conexao.VerifierPort) ? string.Empty : $"[PORTA_VERIFICADOR]={conexao.VerifierPort}\n";

            string settings = string.Concat(ports, verifierPort);

            //Debug.WriteLine($"\n\n'3: {settings}'\n\n");

            return settings;
        }

        public string Create3CConnectionClientFileSettings(ConexaoFileModel conexao, AppParams appParams)
        {

            string ports = CreateClientPortsSettings(conexao);
            string defaultSettings = "[VERSAOEXE]=1.0.0.0\n[BANCODADOS]=DATASNAP\n\n";
            string redirector = conexao.UseRedirect ? $"[REDIRECIONADOR]=S\n\n" : $"[REDIRECIONADOR]=N\n\n";
            string loginPadrao = string.IsNullOrEmpty(conexao.DefaultLogin) || !appParams.DefaultLoginCheckbox ? string.Empty : $"[LOGINPADRAO]={conexao.DefaultLogin}\n";
            string senhaPadrao = string.IsNullOrEmpty(conexao.DefaultPassword) || !appParams.DefaultPasswordCheckbox ? string.Empty : $"[SENHAPADRAO]={conexao.DefaultPassword}\n";
            string editorTexto = string.IsNullOrEmpty(conexao.TextEditorPath) || !appParams.EditorCheckbox ? string.Empty : $"[DIRATUALIZACAO]={conexao.TextEditorPath}\n";
            string updateFolder = string.IsNullOrEmpty(conexao.UpdateFolder) || !appParams.DirUpdateCheckbox ? string.Empty : $"[EDITOR]={conexao.UpdateFolder}\n";
            string useWebMenu = conexao.UseWebMenu ? $"[ABRIR_MENUSWEB_NODESKTOP]=S" : $"[ABRIR_MENUSWEB_NODESKTOP]=N";


            string settings = string.Concat(defaultSettings, ports, redirector, loginPadrao, senhaPadrao, editorTexto, updateFolder, useWebMenu);

            //Debug.WriteLine($"\n\n'3Client: {settings}'\n\n");

            return settings;
        }

        public void UpdateRedirectorFile(string redirect, ConexaoFileModel conexao)
        {
            string redirectorPort = $"[PORTA_REDIRECIONADOR]={conexao.RedirectPort}\n\n";
            string redirecionaSettings = string.Concat(redirectorPort, CreateRedirectPortsSettings(conexao));

            File.WriteAllText(redirect, redirecionaSettings);
            //Debug.WriteLine($"\n\n'{redirecionaSettings}'\n\n");
        }

        public bool isVerifierPortSet(ConexaoFileModel conexao)
        {
            return !string.IsNullOrEmpty(conexao.VerifierPort);
        }            
        public bool istRedirectorPortSet(ConexaoFileModel conexao) 
        {
            return !string.IsNullOrEmpty(conexao.RedirectPort);
        }
        public bool isPortsSet(ConexaoFileModel conexao)
        {
            return conexao.Ports.Count(p => string.IsNullOrEmpty(p.Port)) == 0 && !conexao.Ports.Any(p => string.IsNullOrEmpty(p.Port));
        }

        public bool is3CSettingsValid(ConexaoFileModel conexao)
        {
            if (!isVerifierPortSet(conexao))
            {
                MessageBox.Show("Preencha o campo 'Porta Verificador'.", "Campo obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                //return false;
            }
            if (!istRedirectorPortSet(conexao))
            {
                MessageBox.Show("Preencha o campo 'Porta Redirecionador'.", "Campo obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                //return false;
            }
            Debug.WriteLine($"\n\nports: {isPortsSet(conexao)}\n\n");
            if (!isPortsSet(conexao))
            {
                if(conexao.Ports.Any(p => string.IsNullOrEmpty(p.Port)))
                {
                    int appsWithNoPortCount = conexao.Ports.Count(p => string.IsNullOrEmpty(p.Port));
                    string appsWithNoPort = string.Join(", ", conexao.Ports.Where(p => string.IsNullOrEmpty(p.Port)).Select(p => p.App));
                    if (appsWithNoPortCount > 1)
                    {
                        MessageBox.Show($"Adicione as portas dos campos: {appsWithNoPort}.", "Campo obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    MessageBox.Show($"Adicione a porta do campo: {appsWithNoPort}.", "Campo obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                MessageBox.Show("Adicione as portas dos executáveis.", "Campo obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
