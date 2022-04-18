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

namespace IDensity.Views.Resourses.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Parameter.xaml
    /// </summary>
    public partial class Parameter : CommandExtention
    {
        public Parameter()
        {
            InitializeComponent();            
        }

        public string StringFormat { get; set; }
        


    }
}
