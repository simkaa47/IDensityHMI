﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using IDensity.ViewModels;
using IDensity.Views;
using Microsoft.Win32;

namespace IDensity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public  partial class MainWindow : Window
    {        
        public MainWindow()
        {            
            InitializeComponent();                        
        }

        private void Logout_click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as VM;
            Password password = new Password(vm);
            password.Show();            
        }    

        
        private void FileDialogOpen(TextBlock tb)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                Multiselect = true,
                Title = "Выберите файл"
            };
            fileDialog.Filter = "  Текстовые файлы (*.txt)|*.txt";
            Nullable<bool> dialogOK = fileDialog.ShowDialog();


            if (dialogOK == true)
            {
                tb.Text = fileDialog.FileName;

            }
        }
        private void BrowseLogPath(object sender, RoutedEventArgs e)
        {
            FileDialogOpen(LogPath);
        }

        private void SpectrLogPathShow(object sender, RoutedEventArgs e)
        {
            FileDialogOpen(SpectrLogPath);
        }

        private void SdCardWritePathShow(object sender, RoutedEventArgs e)
        {
            FileDialogOpen(SdCardWritePath);
        }
    }
}
