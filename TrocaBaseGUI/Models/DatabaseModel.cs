using System;
using System.ComponentModel;
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
        if (String.IsNullOrEmpty(db.DisplayName))
        {
            db.DisplayName = StringUtils.ToCapitalize(db.Name);
        }
        else
        {
            db.DisplayName = newDisplayName;
        }
    }

    public override string ToString()
    {
        return Name;
    }
}
