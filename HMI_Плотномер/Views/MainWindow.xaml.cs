using System;
using System.Collections.Generic;
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
using HMI_Плотномер.ViewModels;
using HMI_Плотномер.Views;

namespace HMI_Плотномер
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public  partial class MainWindow : Window
    {
        VM vm;
        internal MainWindow(VM vM )
        {            
            InitializeComponent();
            this.vm = vM;
            this.DataContext = vM;            
        }

        private void Logout_click(object sender, RoutedEventArgs e)
        {
            Password password = new Password(vm);
            password.Show();
            this.Close();
        }
    }
}
