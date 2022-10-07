using IDensity.AddClasses;
using IDensity.Core.ViewModels.MeasUnits;
using IDensity.DataAccess.Models;
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
using System.Windows.Shapes;

namespace IDensity.Core.Views
{
    /// <summary>
    /// Interaction logic for MeasUnitDialog.xaml
    /// </summary>
    public partial class MeasUnitDialog : Window
    {

        public MeasUnitDialog(MeasUnit data)
        {
            InitializeComponent();
            Data = data;
            this.DataContext = Data;
        }

        public MeasUnit Data { get; }

        void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
