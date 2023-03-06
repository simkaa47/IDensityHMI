using IDensity.DataAccess.Models;
using System.Windows;

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
