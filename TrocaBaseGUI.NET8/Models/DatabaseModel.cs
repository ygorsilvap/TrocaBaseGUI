using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using TrocaBaseGUI.Utils;

public class DatabaseModel : INotifyPropertyChanged
{
    private string name;
    public string Name
    {
        get => name;
        set
        {
            name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    private string displayName;
    public string DisplayName
    {
        get => displayName;
        set
        {
            displayName = value;
            OnPropertyChanged(nameof(DisplayName));
        }
    }

    private string dbType;
    public string DbType
    {
        get => dbType;
        set
        {
            dbType = value;
            OnPropertyChanged(nameof(DbType));
        }
    }
    private string instance;
    public string Instance
    {
        get => instance;
        set
        {
            instance = value;
            OnPropertyChanged(nameof(Instance));
        }
    }

    private string environment;
    public string Environment
    {
        get => environment;
        set
        {
            environment = value;
            OnPropertyChanged(nameof(Environment));
        }
    }

    private bool isSelected = false;
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public static void SetDisplayName(DatabaseModel db, string newDisplayName = "")
    {
        db.DisplayName = String.IsNullOrEmpty(db.DisplayName) || String.IsNullOrEmpty(newDisplayName) ? 
            StringUtils.ToCapitalize(db.Name) : StringUtils.ToCapitalize(newDisplayName);
    }

    public static void SetSelection(ObservableCollection<DatabaseModel> dbs, string db)
    {
        if (dbs.Any(b => b.IsSelected == true)) 
            dbs.FirstOrDefault(b => b.IsSelected == true).IsSelected = false;

        if(dbs.Any(dbs => dbs.Name.Equals(db)))
            dbs.FirstOrDefault(b => b.Name.Equals(db)).IsSelected = true;
        else Debug.WriteLine($"\n\n\nDatabase {db} not found in the collection.\n\n\n");
    }

    public override string ToString()
    {
        return Name;
    }
}
