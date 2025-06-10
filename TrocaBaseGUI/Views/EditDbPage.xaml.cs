using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class EditDbPage : Page
    {
        public DatabaseModel _db;
        public MainViewModel _vmDbs;
        public EditDbPage(MainViewModel vmDbs, DatabaseModel db)
        {
            InitializeComponent();
            _db = db;
            renameInput.Text = _db.DisplayName;
            _vmDbs = vmDbs;
        }

        private void SaveName_Click(object sender, RoutedEventArgs e)
        {
            DatabaseModel.SetDisplayName(_db, renameInput.Text);

            NavigationService.GoBack();
        }
    }
}
