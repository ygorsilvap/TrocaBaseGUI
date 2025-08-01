using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TrocaBaseGUI.Models;

namespace TrocaBaseGUI.Services
{


    internal class AppStateService
    {
        //public void SaveState(SysDirectory sysDir)
        //{
        //    List<SysDirectory> historyList = sysDir.ToList();
        //    string HistoricoSerialized = JsonSerializer.Serialize(historyList);
        //    if (HistoricoSerialized != null && !string.IsNullOrEmpty(HistoricoSerialized))
        //    {
        //        Properties.Settings.Default.historico = HistoricoSerialized;
        //    }

        //    Properties.Settings.Default.ExeFile = sysDir.ExeFile;
        //    Properties.Settings.Default.conexaoFile = sysDir.ConexaoFile;
        //    Properties.Settings.Default.Conexao = selectedBase;
        //    Properties.Settings.Default.Save();
        //}

        //public void LoadState(ObservableCollection<SysDirectory> sysDir)
        //{
        //    exeFile = Properties.Settings.Default.ExeFile;
        //    ConexaoFile = Properties.Settings.Default.conexaoFile;
        //    selectedBase = Properties.Settings.Default.Conexao;

        //    string HistoricoSerialized = Properties.Settings.Default.historico;
        //    if (HistoricoSerialized != null && !string.IsNullOrEmpty(HistoricoSerialized))
        //    {
        //        History =
        //        JsonSerializer.Deserialize<ObservableCollection<SysDirectory>>(HistoricoSerialized)
        //        ?? new ObservableCollection<SysDirectory>();
        //    }
        //}

        //public void ClearApp(ObservableCollection<SysDirectory> sysDir)
        //{
        //    ConexaoFile = "";
        //    selectedBase = "";
        //    exeFile = "";

        //    History.Clear();
        //    Properties.Settings.Default.historico = "";
        //    Properties.Settings.Default.Save();
        //}

        //public void AddDirectory(string endereco, string enderecoCompleto, string exe)
        //{
        //    // Verifica se já existe o diretório no histórico
        //    var existente = History.FirstOrDefault(d => d.Address == endereco);
        //    if (existente != null)
        //    {
        //        History.Remove(existente); // Remove para mover para o topo
        //    }

        //    // Adiciona no início da lista
        //    History.Insert(0, new SysDirectory(endereco, enderecoCompleto, exe));
        //    exeFile = exe;

        //    // Garante que só existam no máximo 5
        //    while (History.Count > MaxHistory)
        //    {
        //        History.RemoveAt(History.Count - 1); // Remove o mais antigo (último)
        //    }
        //}

        //public void AddDirectory(ObservableCollection<SysDirectory> sysDir)
        //{
        //    // Verifica se já existe o diretório no histórico
        //    var existente = History.FirstOrDefault(d => d.Address == endereco);
        //    if (existente != null)
        //    {
        //        History.Remove(existente); // Remove para mover para o topo
        //    }

        //    // Adiciona no início da lista
        //    History.Insert(0, new SysDirectory(endereco, enderecoCompleto, exe));
        //    exeFile = exe;

        //    // Garante que só existam no máximo 5
        //    while (History.Count > MaxHistory)
        //    {
        //        History.RemoveAt(History.Count - 1); // Remove o mais antigo (último)
        //    }
        //}
    }
}
