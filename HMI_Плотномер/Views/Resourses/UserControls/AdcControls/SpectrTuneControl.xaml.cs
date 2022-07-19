﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IDensity.Views.Resourses.UserControls.AdcControls
{
    /// <summary>
    /// Interaction logic for SpectrTuneControl.xaml
    /// </summary>
    public partial class SpectrTuneControl : UserControl
    {
        public SpectrTuneControl()
        {
            InitializeComponent();
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

        private void SpectrLogPathShow(object sender, RoutedEventArgs e)
        {
            FileDialogOpen(SpectrLogPath);
        }


    }
}
