using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Models
{
    public interface IAppConn
    {
        string App { get; }
        string Port { get; set; }
        string Server { get; set; }
    }
}
