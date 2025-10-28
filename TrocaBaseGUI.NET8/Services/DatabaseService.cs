using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Services
{
    public class DatabaseService
    {
        public ObservableCollection<DatabaseModel> Databases { get; set; } = new ObservableCollection<DatabaseModel>();

        public DatabaseModel GetDatabases()
        {
            DatabaseModel databases = new DatabaseModel();
            return databases;
        }

        public void AddDatabase(DatabaseModel database)
        {
            //gerenciar IDs
            int lastIdSet = Databases.LastOrDefault().Id;



            Databases.Add(database);
        }

        public void SortDatabases(ObservableCollection<DatabaseModel> databases)
        {
            var sortedDatabases = Databases.OrderBy(db => db.DisplayName).ToList();
            Databases.Clear();
            foreach (var db in sortedDatabases)
            {
                Databases.Add(db);
            }
        }
    }
}

