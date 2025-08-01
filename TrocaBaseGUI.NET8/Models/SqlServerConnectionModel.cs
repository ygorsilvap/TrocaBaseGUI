using System.ComponentModel;
using System.Runtime.CompilerServices;

public class SqlServerConnectionModel : INotifyPropertyChanged
{
    private string server;
    public string Server
    {
        get => server;
        set { server = value; OnPropertyChanged(); }
    }

    private string database;
    public string Database
    {
        get => database;
        set { database = value; OnPropertyChanged(); }
    }

    private string username;
    public string Username
    {
        get => username;
        set { username = value; OnPropertyChanged(); }
    }

    private string password;
    public string Password
    {
        get => password;
        set { password = value; OnPropertyChanged(); }
    }

    private bool useIntegratedSecurity = true;
    public bool UseIntegratedSecurity
    {
        get => useIntegratedSecurity;
        set { useIntegratedSecurity = value; OnPropertyChanged(); }
    }

    private bool _sqlLoaded = false;
    public bool SqlLoaded
    {
        get => _sqlLoaded;
        set { _sqlLoaded = value; OnPropertyChanged(); }
    }

    public string GetConnectionString(string server)
    {
        if (UseIntegratedSecurity)
        {
            return $"Server={server};Integrated Security=True;";
        }
        else
        {
            return $"Server={server};User Id={Username};Password={Password};";
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
