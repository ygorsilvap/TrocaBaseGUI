using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

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

    private string username = "CNP";
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

    private string environment;
    public string Environment
    {
        get => environment;
        set { environment = value; OnPropertyChanged(); }
    }

    private bool useIntegratedSecurity = true;
    public bool UseIntegratedSecurity
    {
        get => useIntegratedSecurity;
        set { useIntegratedSecurity = value; OnPropertyChanged(); }
    }
    public bool IsValid()
    {
        return string.IsNullOrEmpty(Server);// || string.IsNullOrEmpty(Password);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
