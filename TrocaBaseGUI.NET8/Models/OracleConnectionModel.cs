using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net;
using System;

public class OracleConnectionModel : INotifyPropertyChanged
{
    private string user;
    public string User
    {
        get => user;
        set { user = value; OnPropertyChanged(); }
    }

    private string password;
    public string Password
    {
        get => password;
        set { password = value; OnPropertyChanged(); }

    }

    private string instance;
    public string Instance
    {
        get => instance;
        set { instance = value; OnPropertyChanged(); }
    }

    private string port = "1521";
    public string Port
    {
        get => port;
        set { port = value; OnPropertyChanged(); }
    }

    public string GetConnectionString(string hostname, string password, string port, string instance)
    {
        if(string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hostname) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(instance))
        {
            Console.WriteLine("GetConnectionString INVALID PARAMS");
            return "";
        } else
        {
          return $"User Id=sys;Password={password};Data Source={hostname}:{port}/{instance};DBA Privilege=SYSDBA;";
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
