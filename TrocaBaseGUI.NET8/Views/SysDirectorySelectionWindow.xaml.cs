using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.Services;
using TrocaBaseGUI.ViewModels;

namespace TrocaBaseGUI.Views
{
    public partial class SysDirectorySelectionWindow : Window
    {
        public MainViewModel viewModel;
        public SysDirectorySelectionWindow()
        {
            InitializeComponent();
            viewModel = MainViewModel.Instance;

            MainBorder.Height = 150 + (viewModel?.SysDirectoryList.Count * 27 ?? 0);
            sysDirectorySelectionWindow.Top -= (viewModel?.SysDirectoryList.Count * 15 ?? 0);

            DataContext = viewModel;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SysDirectorySelectionBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Selecione a pasta do sistema.",
                InitialDirectory = @"C:\",
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = System.IO.Path.GetFullPath(dialog.FileName);

                if (!SysDirectoryService.IsSysDirectoryValid(path))
                {
                    MessageBox.Show("A pasta selecionada não é uma pasta válida de sistema Linx.\n\nPor favor, selecione uma pasta que contenha os arquivos de conexão e executáveis do sistema.", "Pasta Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (viewModel.SysDirectoryList.Any(d => d.Path.Equals(path)))
                {
                    MessageBox.Show("A pasta selecionada já existe na lista.", "Pasta Duplicada", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                SysDirectoryTextBox.Text = path;
            }
        }

        private void DeleteDirectory_Click(object sender, RoutedEventArgs e)
        {
            var directory = sender as Button;
            var item = directory.DataContext as SysDirectoryModel;

            var del = MessageBox.Show("Deseja mesmo excluir esta pasta da lista?", "Excluir Pasta",
                MessageBoxButton.YesNo, MessageBoxImage.Warning)
                .ToString().ToLower();

            if (del.Equals("yes"))
            {
                if(item.Id == viewModel.appState.SelectedFolder.Id)
                    viewModel.appState.SelectedFolder = null;

                viewModel.sysDirectoryService.DeleteDirectory(viewModel.SysDirectoryList, item.Id);

                MainBorder.Height -= 27;
                sysDirectorySelectionWindow.Top += 15;

                //Debug.WriteLine("\n\n\nDeletando pasta da lista...\n\n\n");
            }
        }

        private void AddDirectory_Click(object sender, RoutedEventArgs e)
        {
            if(viewModel.SysDirectoryList.Count >= 10 && !string.IsNullOrEmpty(SysDirectoryTextBox.Text))
            {
                if(!(DirectoryCount.Foreground == Brushes.Red))
                {
                    FontWeight dirCountWeight = DirectoryCount.FontWeight;
                    Brush dirCountColor = DirectoryCount.Foreground;

                    FontWeight dirMaxCountWeight = DirectoryMaxCount.FontWeight;
                    Brush dirMaxCountColor = DirectoryMaxCount.Foreground;

                    DirectoryCount.FontWeight = FontWeights.Bold;
                    DirectoryCount.Foreground = Brushes.Red;

                    DirectoryMaxCount.FontWeight = FontWeights.Bold;
                    DirectoryMaxCount.Foreground = Brushes.Red;

                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(3);
                    timer.Tick += (s, e) =>
                    {
                        timer.Stop();
                        DirectoryCount.FontWeight = dirCountWeight;
                        DirectoryCount.Foreground = dirCountColor;

                        DirectoryMaxCount.FontWeight = dirMaxCountWeight;
                        DirectoryMaxCount.Foreground = dirMaxCountColor;
                    };
                    timer.Start();
                }

                return;
            }

            if (!string.IsNullOrEmpty(SysDirectoryTextBox.Text))
            {
                int sysDirectoryCount = viewModel.SysDirectoryList.Count();

                viewModel.sysDirectoryService.AddDirectory(viewModel.SysDirectoryList, SysDirectoryTextBox.Text);

                viewModel.appState.SelectedFolder = viewModel.SysDirectoryList.LastOrDefault();

                if (viewModel.SysDirectoryList.Count > 1 && viewModel.SysDirectoryList.Count > sysDirectoryCount)
                {
                    MainBorder.Height += 27;
                    sysDirectorySelectionWindow.Top -= 15;
                }
                SysDirectoryTextBox.Text = string.Empty;
                //Debug.WriteLine("\n\n\nAdicionando pasta à lista...\n\n\n");
            }

        }

        private void SysDirectoryEditBtn_Click(object sender, RoutedEventArgs e)
        {
            var directory = sender as Button;
            var item = directory.DataContext as SysDirectoryModel;

            var dialog = new CommonOpenFileDialog
            {
                Title = "Selecione a pasta do sistema.",
                InitialDirectory = @"C:\Users\ygor\Desktop\TrocaBaseTestes",
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = System.IO.Path.GetFullPath(dialog.FileName);

                if (!SysDirectoryService.IsSysDirectoryValid(path))
                {
                    MessageBox.Show("A pasta selecionada não é uma pasta válida de sistema Linx.\n\nPor favor, selecione uma pasta que contenha os arquivos de conexão e executáveis do sistema.", "Pasta Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                } else if(viewModel.SysDirectoryList.Any(d => d.Path.Equals(path)))
                {
                    MessageBox.Show("A pasta selecionada já existe na lista.", "Pasta Duplicada", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var del = MessageBox.Show("Deseja mesmo alterar o caminho?", "Alterar Pasta",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    .ToString().ToLower();

                if (del.Equals("yes"))
                {
                    item.Path = path;
                    viewModel.sysDirectoryService.EditDirectory(viewModel.SysDirectoryList, item.Id, item.Path);
                }

            }
        }

        private void DeleteAllSysDirectoryList_Click(object sender, RoutedEventArgs e)
        {
            if(viewModel.SysDirectoryList.Count > 1)
            {
                var del = MessageBox.Show("Deseja mesmo excluir todas as pastas?", "Excluir Todas as Pastas",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    .ToString().ToLower();
                if (del.Equals("yes"))
                {
                    MainBorder.Height = 150;
                    sysDirectorySelectionWindow.Top += (viewModel?.SysDirectoryList.Count * 15 ?? 0);
                    viewModel.SysDirectoryList.Clear();
                }
            }
        }

        private void UpdateSysDirectoriesFiles_Click(object sender, RoutedEventArgs e)
        {
            viewModel.sysDirectoryService.UpdateSysDirectoriesFiles(viewModel.SysDirectoryList);
        }
    }
}
