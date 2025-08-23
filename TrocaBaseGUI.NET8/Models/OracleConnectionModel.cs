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

    public bool IsValid()
    {
        return string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Port);
    }

    public string GetConnectionString(OracleConnectionModel oracleConnection, string instance)
    {
        if (string.IsNullOrEmpty(oracleConnection.Password) || string.IsNullOrEmpty(oracleConnection.Server) || string.IsNullOrEmpty(oracleConnection.Port) || string.IsNullOrEmpty(instance))
        {
            Debug.WriteLine("GetConnectionString INVALID PARAMS");
            return "";
        }
        //Rever o User ID=LINX
        return environment == "local"
            ? $"User Id=sys;Password={oracleConnection.Password};Data Source={oracleConnection.Server}:{oracleConnection.Port}/{instance};DBA Privilege=SYSDBA;"
            : $"User Id=LINX;Password={oracleConnection.Password};Data Source={oracleConnection.Server}:{oracleConnection.Port}/{instance};";
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
