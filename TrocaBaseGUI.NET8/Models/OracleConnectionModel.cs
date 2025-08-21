using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net;
using System;
using System.Windows.Forms;
using System.Diagnostics;

public class OracleConnectionModel : INotifyPropertyChanged
{
    private string server;
    public string Server
    {
        get => server;
        set { server = value; OnPropertyChanged(); }
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

    private string environment;
    public string Environment
    {
        get => environment;
        set { environment = value; OnPropertyChanged(); }
    }

    //public string GetLocalConnectionString(string server, string password, string port, string instance)
    //{
    //    if(string.IsNullOrEmpty(password) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(instance))
    //    {
    //        Console.WriteLine("GetConnectionString INVALID PARAMS");
    //        return "";
    //    } else
    //    { 
    //      return $"User Id=sys;Password={password};Data Source={server}:{port}/{instance};DBA Privilege=SYSDBA;";
    //    }
    //}
    //public string GetServerConnectionString(string server, string password, string port, string instance)
    //{
    //    if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(instance))
    //    {
    //        Console.WriteLine("GetConnectionString INVALID PARAMS");
    //        return "";
    //    }
    //    else
    //    {
    //        //Rever o User ID=LINX
    //        return $"User Id=LINX;Password={password};Data Source={server}:{port}/{instance};";
    //    }
    //}

    public string GetConnectionString(string server, string password, string port, string instance, string environment)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(instance))
        {
            Debug.WriteLine("GetConnectionString INVALID PARAMS");
            return "";
        }
        //Rever o User ID=LINX
        return environment == "local"
            ? $"User Id=sys;Password={password};Data Source={server}:{port}/{instance};DBA Privilege=SYSDBA;"
            : $"User Id=LINX;Password={password};Data Source={server}:{port}/{instance};";
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
