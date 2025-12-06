using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrocaBaseGUI.Utils;

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

        public void SortDatabasesByName(ObservableCollection<DatabaseModel> databases)
        {
            var sortedDatabases = databases.OrderBy(db => db.DisplayName).ToList();
            databases.Clear();
            foreach (var db in sortedDatabases)
            {
                databases.Add(db);
            }
        }

        public void SortDatabasesByNameDesc(ObservableCollection<DatabaseModel> databases)
        {
            var sortedDatabases = databases.OrderByDescending(db => db.DisplayName).ToList();
            databases.Clear();
            foreach (var db in sortedDatabases)
            {
                databases.Add(db);
            }
        }

        public void SortDatabasesByDate(ObservableCollection<DatabaseModel> databases)
        {
            var sortedDatabases = databases.OrderBy(db => Convert.ToDateTime(db.ImportDate)).ToList();
            databases.Clear();
            foreach (var db in sortedDatabases)
            {
                databases.Add(db);
            }
        }

        public void SortDatabasesByDateDesc(ObservableCollection<DatabaseModel> databases)
        {
            var sortedDatabases = databases.OrderByDescending(db => Convert.ToDateTime(db.ImportDate)).ToList();
            databases.Clear();
            foreach (var db in sortedDatabases)
            {
                databases.Add(db);
            }
        }

        public static void SetDisplayName(DatabaseModel db, string newDisplayName = "")
        {
            db.DisplayName = String.IsNullOrEmpty(db.DisplayName) || String.IsNullOrEmpty(newDisplayName) ?
                StringUtils.ToCapitalize(db.Name) : StringUtils.ToCapitalize(newDisplayName);
        }

        public static void SetSelection(ObservableCollection<DatabaseModel> dbs, int id)
        {
            var selectedDb = dbs.FirstOrDefault(db => db.Id.Equals(id));

            if (id < 0)
                return;

            if (dbs.Any(b => b.IsSelected == true))
                dbs.FirstOrDefault(b => b.IsSelected == true).IsSelected = false;

            selectedDb.IsSelected = true;
        }

        public static int GetSelection(ObservableCollection<DatabaseModel> dbs)
        {
            //return dbs.Any(b => b.IsSelected == true) ? dbs.FirstOrDefault(b => b.IsSelected == true).Id : -1;
            return dbs.FirstOrDefault(b => b.IsSelected)?.Id ?? -1;
        }

        public DatabaseModel GetDatabaseById(ObservableCollection<DatabaseModel> dbs, int id)
        {
            return dbs.FirstOrDefault(b => b.Id.Equals(id));
        }
    }
}

