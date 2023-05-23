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
            System.Windows.Media.Effects.BlurEffect objBlur = new System.Windows.Media.Effects.BlurEffect();
            objBlur.Radius = 10;
            Application.Current.MainWindow.Effect = objBlur;
            Data = data;
            this.DataContext = Data;
            this.Closing += (o, s) => 
            {
                Application.Current.MainWindow.Effect = null;
            };
        }

        public MeasUnit Data { get; }

        void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        
    }
}
