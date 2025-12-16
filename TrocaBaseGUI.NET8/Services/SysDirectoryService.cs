using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.Properties.Constants;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace TrocaBaseGUI.Services
{
    public class SysDirectoryService
    {
        public static Boolean IsSysDirectoryValid(string path)
        {
            bool conexaoFilesValid = IsDirectoryConexaoFilesValid(path);
            bool exeFilesValid = IsDirectoryExeFilesValid(path);

            return conexaoFilesValid && exeFilesValid && !string.IsNullOrEmpty(path);
        }

        public static Boolean IsDirectoryExeFilesValid(string path) {
            bool bravos = File.Exists(Path.Combine(path, GlobalStrings.Bravos2C)) || File.Exists(Path.Combine(path, GlobalStrings.Bravos3C));
            bool linxdms = File.Exists(Path.Combine(path, GlobalStrings.LinxDMS2C)) || File.Exists(Path.Combine(path, GlobalStrings.LinxDMS3C));
            bool autoshop = File.Exists(Path.Combine(path, GlobalStrings.Autoshop));

            return bravos || linxdms || autoshop;
        }

        public static Boolean IsDirectoryConexaoFilesValid(string path)
        {
            bool conexao2C = File.Exists(Path.Combine(path, GlobalStrings.ConexaoDat));
            bool conexao3C = File.Exists(Path.Combine(path, GlobalStrings.ConexaoServidorDat)) &&
                             File.Exists(Path.Combine(path, GlobalStrings.ConexaoRedirecionaDat)) &&
                             File.Exists(Path.Combine(path, GlobalStrings.ConexaoClienteDat));
            
            return conexao2C || conexao3C;
        }

        public int GetTier(string path) 
        {
            bool conexao2C = File.Exists(Path.Combine(path, GlobalStrings.ConexaoDat));
            bool conexao3C = File.Exists(Path.Combine(path, GlobalStrings.ConexaoServidorDat)) &&
                             File.Exists(Path.Combine(path, GlobalStrings.ConexaoRedirecionaDat)) &&
                             File.Exists(Path.Combine(path, GlobalStrings.ConexaoClienteDat));

            return conexao2C ? 2 : conexao3C ? 3 : 0;
        }

        public SysDirectoryModel GetDir(ObservableCollection<SysDirectoryModel> directoryList, string path)
        {
            if (string.IsNullOrEmpty(path) || directoryList.Count < 1)
                return null;

            return directoryList.FirstOrDefault(d => d.Path.EndsWith(path));
        }

        //public SysDirectoryModel GetDirByFolder(ObservableCollection<SysDirectoryModel> directoryList, string folder)
        //{
        //    if (string.IsNullOrEmpty(folder) || directoryList.Count < 1)
        //        return null;

        //    return directoryList.FirstOrDefault(d => d.Folder.EndsWith(folder));
        //}

        public string GetSysExeFile(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                return string.Empty;
            string erp = File.Exists(Path.Combine(directory, GlobalStrings.Bravos2C)) ? GlobalStrings.Bravos2C :
                         File.Exists(Path.Combine(directory, GlobalStrings.Bravos3C)) ? GlobalStrings.Bravos3C :
                         File.Exists(Path.Combine(directory, GlobalStrings.LinxDMS2C)) ? GlobalStrings.LinxDMS2C :
                         File.Exists(Path.Combine(directory, GlobalStrings.LinxDMS3C)) ? GlobalStrings.LinxDMS3C :
                         File.Exists(Path.Combine(directory, GlobalStrings.Autoshop)) ? GlobalStrings.Autoshop : string.Empty;
            return string.IsNullOrEmpty(erp) ? string.Empty : $"{Path.GetFileNameWithoutExtension(erp)}";
        }

        public ObservableCollection<string> GetExeList(string directory)
        {
            ObservableCollection<string> exeList = new ObservableCollection<string>(Directory.GetFiles(directory, "*.exe").Select(f => Path.GetFileNameWithoutExtension(f)).ToList());
           
            string mainExe = GetSysExeFile(directory);

            exeList.Remove(exeList.Where(exe => exe.Equals(mainExe)).ToString());

            return exeList;
        }

        public List<string> GetSysFound(string path)
        {
            var systemsFound = new List<string>();

            bool bravos2c = File.Exists($"{path}\\{GlobalStrings.Bravos2C}");
            bool bravos3c = File.Exists($"{path}\\{GlobalStrings.Bravos3C}");
            bool apollo2c = File.Exists($"{path}\\{GlobalStrings.LinxDMS2C}");
            bool apollo3c = File.Exists($"{path}\\{GlobalStrings.LinxDMS3C}");
            bool autoshop = File.Exists($"{path}\\{GlobalStrings.Autoshop}");

            if (bravos2c)
                systemsFound.Add(GlobalStrings.Bravos2C.Remove(GlobalStrings.Bravos2C.Length-4));
            if(bravos3c)
                systemsFound.Add(GlobalStrings.Bravos3C.Remove(GlobalStrings.Bravos3C.Length - 4));
            if (apollo2c)
                systemsFound.Add(GlobalStrings.LinxDMS2C.Remove(GlobalStrings.LinxDMS2C.Length - 4));
            if (apollo3c)
                systemsFound.Add(GlobalStrings.LinxDMS3C.Remove(GlobalStrings.LinxDMS3C.Length - 4));
            if (autoshop)
                systemsFound.Add(GlobalStrings.Autoshop.Remove(GlobalStrings.Autoshop.Length - 4));

            return systemsFound;
        }

        public void AddDirectory(ObservableCollection<SysDirectoryModel> directoryList, string directory)
        {
            string sysFolder = $"{Path.GetFileName(directory)}";
            var mainExeFiles = GetSysFound(directory);
            ObservableCollection<string> exeList = GetExeList(directory);
            int id = directoryList.Count < 1 ? 0 : directoryList.Count;

            SysDirectoryModel dir = new SysDirectoryModel
            {
                Id = id,
                Folder = sysFolder,
                Path = directory,
                MainExeFiles = mainExeFiles,
                ExeFiles = exeList,
                Tier = GetTier(directory)
            };

            bool exists = GetDir(directoryList, dir.Path) != null;

            if (!IsSysDirectoryValid(dir.Path) || exists)
                return;

            directoryList.Add(dir);
        }

        public void EditDirectory(ObservableCollection<SysDirectoryModel> directoryList, int id, string directory)
        {
            SysDirectoryModel dir = directoryList.FirstOrDefault(d => d.Id == id);

            if (!directory.ToLower().Equals(dir.Path))
            {
                dir.Id = id;
                dir.Folder = $"{Path.GetFileName(directory)}";
                dir.Path = directory;
                dir.MainExeFiles = GetSysFound(directory);
                dir.ExeFiles = GetExeList(directory);
                dir.Tier = GetTier(directory);
            }
        }

        public void DeleteDirectory(ObservableCollection<SysDirectoryModel> directoryList, int id)
        {
            if(directoryList.Any(directoryList => directoryList.Id == id))
                directoryList.Remove(directoryList.FirstOrDefault(d => d.Id == id));
        }

        //public void ClearDirectories(ObservableCollection<SysDirectoryModel> directoryList)
        //{
        //    directoryList.Clear();
        //}

        public void UpdateSysDirectoriesFiles(ObservableCollection<SysDirectoryModel> directoryList)
        {
            foreach (var dir in directoryList)
            {
                dir.MainExeFiles = GetSysFound(dir.Path);
                dir.ExeFiles = GetExeList(dir.Path);
            }

        }
    }
}
