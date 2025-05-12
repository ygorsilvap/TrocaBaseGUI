using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TrocaBaseGUI.Models;
using TrocaBaseGUI.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Threading;


namespace TrocaBaseGUI.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        public Frame MainFramePublic => MainFrame;
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new MainPage());
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //viewModel.SaveState(); // Salva os dados antes de fechar
        }
    }
}