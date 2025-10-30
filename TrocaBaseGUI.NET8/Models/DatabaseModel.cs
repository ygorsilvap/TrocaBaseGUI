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
            OnPropertyChanged();
        }
    }
    private string name;
    public string Name
    {
        get => name;
        set
        {
            name = value;
            OnPropertyChanged();
        }
    }

    private string displayName;
    public string DisplayName
    {
        get => displayName;
        set
        {
            displayName = value;
            OnPropertyChanged();
        }
    }

    private string dbType;
    public string DbType
    {
        get => dbType;
        set
        {
            dbType = value;
            OnPropertyChanged();
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
            OnPropertyChanged();
        }
    }

    private string environment;
    public string Environment
    {
        get => environment;
        set
        {
            environment = value;
            OnPropertyChanged();
        }
    }

    private string server;
    public string Server
    {
        get => server;
        set
        {
            server = value;
            OnPropertyChanged();
        }
    }

    private bool isSelected = false;
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string prop = null)
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
        var selectedDb = dbs.FirstOrDefault(db => db.Id.Equals(id));

        if (id < 0)
            return;

        if (dbs.Any(b => b.IsSelected == true))
            dbs.FirstOrDefault(b => b.IsSelected == true).IsSelected = false;

        selectedDb.IsSelected = true;
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
