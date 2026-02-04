using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TrocaBaseGUI.Utils;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{

    //Classe, funcionalidade e tela feito as pressas. REFAZER TODA A CLASSE. TWO WAY BINDING.
    public partial class EditDbPage : Page
    {
        public DatabaseModel _db;
        public MainViewModel _viewModelDbs;
        bool editMode;

        //Construtor para edição de base
        public EditDbPage(MainViewModel viewModelDbs, DatabaseModel db)
        {
            InitializeComponent();
            _db = db;
            EditDbPage_Loaded();

            //dbType.Text = _db.DbType;
            _viewModelDbs = viewModelDbs;
        }

        //Construtor para adição de base
        public EditDbPage(MainViewModel viewModelDbs)
        {
            InitializeComponent();
            _viewModelDbs = viewModelDbs;
            _db = new DatabaseModel() { Environment = "local", IsManualAdded = true };
        }

        public void EditDbPage_Loaded()
        {
            editMode = true;
            nameInput.Text = _db.DisplayName;
            serverInput.Text = _db.Server;

            //serverInput.IsEnabled = false;

            if (!string.IsNullOrEmpty(_db.DbType) && _db.DbType.Equals("oracle", StringComparison.OrdinalIgnoreCase))
            {
                rbOracle.IsChecked = true;
                rbOracle.IsHitTestVisible = false;
                rbSqlServer.IsHitTestVisible = false;

                if (!_db.IsManualAdded)
                {
                    userInput.IsEnabled = false;
                    instDbNameInputTXT.Text = "Instância";
                    userInput.Text = _db.Name;
                    instDbNameInput.Text = _db.Instance;
                    instDbNameInput.IsEnabled = false;
                    serverInput.IsEnabled = false;
                } else
                {
                    instDbNameInputTXT.Text = "Instância";
                    userInput.Text = _db.Name;
                    instDbNameInput.Text = _db.Instance;
                }
            }
            else
            {
                rbSqlServer.IsChecked = true;
                rbOracle.IsHitTestVisible = false;
                rbSqlServer.IsHitTestVisible = false;

                if (!_db.IsManualAdded)
                {
                    userInput.IsEnabled = false;
                    instDbNameInputTXT.Text = "Nome da base";
                    instDbNameInput.Text = _db.Name;
                    instDbNameInput.IsEnabled = false;
                    serverInput.IsEnabled = false;
                }
                else
                {
                    instDbNameInputTXT.Text = "Nome da base";
                    instDbNameInput.Text = _db.Name;
                }
            }

            ClearButton.Foreground = Brushes.Gray;
            ClearButton.IsEnabled = false;
        }   

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void nameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            _db.DisplayName = nameInput.Text;
        }

        private void serverInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            _db.Server = serverInput.Text;
        }

        private void userInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_db.DbType.Equals("oracle", StringComparison.OrdinalIgnoreCase))
                _db.Name = userInput.Text;
        }

        private void instDbNameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_db.DbType) && _db.DbType.Equals("oracle", StringComparison.OrdinalIgnoreCase))
            {
                _db.Instance = instDbNameInput.Text;
                return;
            }

            _db.Name = instDbNameInput.Text;
        }

        private void rb_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;

            if (rb.Content.ToString().ToLower().StartsWith("ora"))
            {
                userInput.IsEnabled = true;
                instDbNameInputTXT.Text = "Instância Oracle";
                _db.DbType = "Oracle";
            }
            else
            {
                userInput.IsEnabled = false;
                instDbNameInputTXT.Text = "Nome da base SQL Server";
                _db.DbType = "SQL Server";
            }

            instDbNameInput.IsEnabled = true;
        }

        public bool IsDbValid()
        {
            if(string.IsNullOrEmpty(_db.DbType))
                return false;

            if(_db.DbType.Equals("oracle", StringComparison.OrdinalIgnoreCase))
            {
                if (String.IsNullOrEmpty(_db.DisplayName) || String.IsNullOrEmpty(_db.Instance) || String.IsNullOrEmpty(_db.Name))
                    return false;
            }

            if (_db.DbType.Equals("sql server", StringComparison.OrdinalIgnoreCase))
            {
                if (String.IsNullOrEmpty(_db.DisplayName) || String.IsNullOrEmpty(_db.Server) || String.IsNullOrEmpty(_db.Name))
                    return false;
            }

            return true;
        }

        public bool IsDbEmpty()
        {
            if (String.IsNullOrEmpty(_db.DisplayName) && String.IsNullOrEmpty(_db.DbType) && String.IsNullOrEmpty(_db.Server) &&
                String.IsNullOrEmpty(_db.Name) && String.IsNullOrEmpty(_db.Instance))
                return true;
            return false;
        }

        public bool IsDbIn()
        {
            foreach (var db in _viewModelDbs.Databases)
            {
                if(_db.DbType.Equals("oracle", StringComparison.OrdinalIgnoreCase))
                {
                    if (_db.DbType.Equals(db.DbType, StringComparison.OrdinalIgnoreCase) &&
                       _db.Instance.Equals(db.Instance, StringComparison.OrdinalIgnoreCase) &&
                       _db.Server.Equals(db.Server, StringComparison.OrdinalIgnoreCase) &&
                       _db.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase))
                        return true;
                } else
                {
                    if (_db.DbType.Equals(db.DbType, StringComparison.OrdinalIgnoreCase) &&
                       _db.Server.Equals(db.Server, StringComparison.OrdinalIgnoreCase) &&
                       _db.Name.Equals(db.Name, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

            }

            return false;
        }

        private void SaveDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsDbEmpty()) //|| _viewModelDbs.Databases.Contains(_db))
                return;

            if (!_db.IsManualAdded)
            {
                NavigationService.GoBack();
                return;
            } 

            if (!IsDbValid())
            {
                MessageBox.Show("Informações Inválidas.", "Base inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsDbIn())
            {
                MessageBox.Show("Base Repetida.", "Base inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (editMode)
            {
                NavigationService.GoBack();
                return;
            }

            if (!_viewModelDbs.Databases.Contains(_db) && IsDbValid())
            {
                _db.Id = UtilityService.IdGen();
                _viewModelDbs.Databases.Add(_db);
                //_viewModelDbs.Databases[_viewModelDbs.Databases.Count - 1].Id = _viewModelDbs.Databases.Count - 1;
                NavigationService.GoBack();
                return;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            //if (!_db.IsManualAdded) return;

            _db = new DatabaseModel();

            nameInput.Text = String.Empty;
            serverInput.Text = String.Empty;
            userInput.Text = String.Empty;
            instDbNameInput.Text = String.Empty;
            rbOracle.IsChecked = false;
            rbSqlServer.IsChecked = false;

            instDbNameInputTXT.Text = "Instância/Nome da Base";
            userInput.IsEnabled = false;
        }
    }
}
