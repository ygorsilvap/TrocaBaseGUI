using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Models
{
    public interface IConexaoFileModel : INotifyCollectionChanged
    {
        string Path { get; set; }
        int Tier { get; set; }
        string DefaultLogin { get; set; }
        string DefaultPassword { get; set; }
        string TextEditorPath { get; set; }
        string UpdateFolder { get; set; }
        bool UseWebMenu { get; set; }
        bool UseRedirect { get; set; }
        string RedirectPort { get; set; }
        ObservableCollection<AppConn> ServerPorts { get; set; }
        ObservableCollection<AppConn> ClientPorts { get; set; }
        ObservableCollection<AppConn> RedirectPorts { get; set; }
    }
}
