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

    private bool useIntegratedSecurity = true;
    public bool UseIntegratedSecurity
    {
        get => useIntegratedSecurity;
        set { useIntegratedSecurity = value; OnPropertyChanged(); }
    }

    public string GetConnectionString(SqlServerConnectionModel sqlServerConnection)
    {
        if (String.IsNullOrEmpty(sqlServerConnection.Password))
        {
            return $"Server={sqlServerConnection.Server};Integrated Security=True;TrustServerCertificate=True;";
        }
        else
        {
            return $"Server={sqlServerConnection.Server};User Id={sqlServerConnection.Username};Password={sqlServerConnection.Password};TrustServerCertificate=True;";
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
