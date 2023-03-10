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

namespace IDensity.Core.Views.UserControls.Parameters
{
    /// <summary>
    /// Interaction logic for DateTimeParameter.xaml
    /// </summary>
    public partial class DateTimeParameter : CommandExtention
    {
        public DateTimeParameter()
        {
            InitializeComponent();
        }



        public string DataTimeFormat
        {
            get { return (string)GetValue(DataTimeFormatProperty); }
            set { SetValue(DataTimeFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataTimeFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataTimeFormatProperty =
            DependencyProperty.Register("DataTimeFormat", typeof(string), typeof(DateTimeParameter), new PropertyMetadata(null));


    }
}
