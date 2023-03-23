using IDensity.Core.Views.DialogWindows.Authorization;
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
            AuthorizationWindow password = new AuthorizationWindow(vm);            
            password.ShowDialog();
        }

        private void Minimaze_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized) Application.Current.MainWindow.WindowState = WindowState.Minimized;
            else Application.Current.MainWindow.WindowState = WindowState.Maximized;
            
        }

        private void Normal_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private void Maximize_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }
    }
}
