﻿using IDensity.ViewModels.MasrerSettings;
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

namespace IDensity.Views.CheckMaster
{
    /// <summary>
    /// Interaction logic for SuccessPage.xaml
    /// </summary>
    public partial class SuccessPage : Page
    {
        private readonly CheckMasterVm _checkMasterVm;

        public SuccessPage(CheckMasterVm checkMasterVm)
        {
            InitializeComponent();
            _checkMasterVm = checkMasterVm;
            DataContext = checkMasterVm;
        }
    }
}
