using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{

    //Classe, funcionalidade e tela feito as pressas. REFAZER TODA A CLASSE. TWO WAY BINDING.
    public partial class EditDbPage : Page
    {
        public DatabaseModel _db;
        public MainViewModel _viewModelDbs;

        public EditDbPage(MainViewModel viewModelDbs, DatabaseModel db)
        {
            InitializeComponent();
            _db = db;
            EditDbPage_Loaded();

            //dbType.Text = _db.DbType;
            _viewModelDbs = viewModelDbs;
        }

        public EditDbPage(MainViewModel viewModelDbs)
        {
            InitializeComponent();
            _viewModelDbs = viewModelDbs;
            _db = new DatabaseModel() { Environment = "local", IsManualAdded = true };
        }

        public void EditDbPage_Loaded()
        {
            nameInput.Text = _db.DisplayName;
            serverInput.Text = _db.Server;
            serverInput.IsEnabled = false;

            if (!string.IsNullOrEmpty(_db.DbType) && _db.DbType.Equals("oracle", StringComparison.OrdinalIgnoreCase))
            {
                rbOracle.IsChecked = true;
                rbOracle.IsHitTestVisible = false;

                rbSqlServer.IsHitTestVisible = false;

                userInput.IsEnabled = false;
                instDbNameInputTXT.Text = "Instância";
                userInput.Text = _db.Name;
                instDbNameInput.Text = _db.Instance;
                instDbNameInput.IsEnabled = false;
            }
            else
            {
                rbSqlServer.IsChecked = true;
                rbOracle.IsHitTestVisible = false;
                rbSqlServer.IsHitTestVisible = false;
                userInput.IsEnabled = false;
                instDbNameInputTXT.Text = "Nome da base";
                instDbNameInput.Text = _db.Name;
                instDbNameInput.IsEnabled = false;
            }
        }   


        private void SaveAndReturn_Click(object sender, RoutedEventArgs e)
        {

            if (isDbEmpty() || _viewModelDbs.Databases.Contains(_db))
            {
                NavigationService.GoBack();
                return;
            }

            if (!_viewModelDbs.Databases.Contains(_db) && isDbValid())
            {
                //Debug.WriteLine($"\n Id: {_db.Id}, Database: {_db.Name}, Type: {_db.DbType}, Environment: {_db.Environment}, Server: {_db.Server}, Date: {_db.ImportDate}\n");

                _viewModelDbs.Databases.Add(_db);
                _viewModelDbs.Databases[_viewModelDbs.Databases.Count - 1].Id = _viewModelDbs.Databases.Count - 1;
                NavigationService.GoBack();
                return;
            }
            MessageBox.Show("Base duplicada ou inválida.\nRevise os dados inseridos.");

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
                instDbNameInputTXT.Text = "Instância";
                _db.DbType = "Oracle";
            }
            else
            {
                userInput.IsEnabled = false;
                instDbNameInputTXT.Text = "Nome da base";
                _db.DbType = "SQL Server";
            }
        }

        public bool isDbValid()
        {
            if (String.IsNullOrEmpty(_db.DisplayName) || String.IsNullOrEmpty(_db.DbType) || String.IsNullOrEmpty(_db.Server) || String.IsNullOrEmpty(_db.Name))
            {
                if ((!String.IsNullOrEmpty(_db.DbType) && _db.DbType.Equals("oracle", StringComparison.OrdinalIgnoreCase)) && String.IsNullOrEmpty(_db.Instance))
                    return false;

                return false;
            }
            return true;
        }

        public bool isDbEmpty()
        {
            if (String.IsNullOrEmpty(_db.DisplayName) && String.IsNullOrEmpty(_db.DbType) && String.IsNullOrEmpty(_db.Server) &&
                String.IsNullOrEmpty(_db.Name) && String.IsNullOrEmpty(_db.Instance))
                return true;
            return false;
        }
    }
}
