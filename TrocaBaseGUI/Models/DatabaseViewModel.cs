using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Models
{
    public class DatabaseViewModel
    {
        public DatabaseModel Model { get; }

        public string DisplayName;

        public DatabaseViewModel(DatabaseModel model, string displayName)
        {
            model = Model;
            DisplayName = displayName ?? model.Name;
        }
    }
}
