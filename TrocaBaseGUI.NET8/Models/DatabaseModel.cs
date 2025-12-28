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

    private string importDate;
    public string ImportDate
    {
        get => importDate;
        set { importDate = value; OnPropertyChanged(); }
    }

    private bool isManualAdded = false;
    public bool IsManualAdded
    {
        get => isManualAdded;
        set
        {
            isManualAdded = value;
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
}
