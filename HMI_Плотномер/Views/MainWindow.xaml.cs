using System;
using System.Collections.Generic;
using System.Globalization;
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
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU", true);
            CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = ",";

            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU", true);
            CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator = ",";

            InitializeComponent();
                                   
        }       

        
        

        

       
    }
}
