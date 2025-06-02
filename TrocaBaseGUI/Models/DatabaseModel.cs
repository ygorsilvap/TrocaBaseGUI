using System.ComponentModel;

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

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public override string ToString()
    {
        return Name;
    }
}
