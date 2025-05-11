using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net;

public class OracleConnectionModel : INotifyPropertyChanged
{
    private string userId;
    public string UserId
    {
        get => userId;
        set { userId = value; OnPropertyChanged(); }
    }

    private string password;
    public string Password
    {
        get => password;
        set { password = value; OnPropertyChanged(); }

    }

    private string hostname = Dns.GetHostName();
    public string Hostname
    {
        get => hostname;
        set { hostname = value; OnPropertyChanged(); }
    }

    private string porta;
    public string Porta
    {
        get => porta;
        set { porta = value; OnPropertyChanged(); }
    }

    public string GetConnectionString()
    {
        return $"User Id={UserId};Password={Password};Data Source={Hostname}:1521/LINX;DBA Privilege=SYSDBA;";
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
