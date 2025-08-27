using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using TrocaBaseGUI.Utils;

public class DatabaseModel : INotifyPropertyChanged
{
    private int id;
    public int Id
    {
        get => id;
        set
        {
            id = value;
            OnPropertyChanged(nameof(Id));
        }
    }
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

    //Oracle only
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

    private string server;
    public string Server
    {
        get => server;
        set
        {
            server = value;
            OnPropertyChanged(nameof(Server));
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

    public static void SetSelection(ObservableCollection<DatabaseModel> dbs, int id)
    {
        if (id < 0)
            return;

        if (dbs.Any(b => b.IsSelected == true))
            dbs.FirstOrDefault(b => b.IsSelected == true).IsSelected = false;

        dbs[id].IsSelected = true;
    }

    public static int GetSelection(ObservableCollection<DatabaseModel> dbs)
    {
        return dbs.FirstOrDefault(b => b.IsSelected == true).Id;
    }

    public override string ToString()
    {
        return Name;
    }
}
