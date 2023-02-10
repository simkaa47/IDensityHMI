using IDensity.ViewModels;
using IDensity.Views;
using System.Windows;
using System.Windows.Controls;

namespace IDensity.Core.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для HighBar.xaml
    /// </summary>
    public partial class HighBar : UserControl
    {
        public HighBar()
        {
            InitializeComponent();
        }

        private void Logout_click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as VM;
            Password password = new Password(vm);
            password.Show();
        }
    }
}
