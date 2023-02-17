using IDensity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IDensity.Core.Views.DialogWindows.Authorization
{
    /// <summary>
    /// Interaction logic for AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        VM vM;
        public AuthorizationWindow(VM vM)
        {
            InitializeComponent();
            if (vM == null) return;
            else this.vM = vM;
            this.DataContext = vM;
        }

        private void EnterClick(object sender, RoutedEventArgs e)
        {
            Check();
        }

        private void Check()
        {
            var user = vM.Users.Data.Where(user => user.Password == Pword.Password && user.Login == Login.Text)
                .FirstOrDefault();
            if (user != null)
            {
                vM.CurUser = user;
                this.Close();
            }
            else
            {
                Pword.Foreground = Brushes.Red;
                Login.Foreground = Brushes.Red;
            }
        }



        private void Login_MouseEnter(object sender, MouseEventArgs e)
        {
            var brush = (SolidColorBrush)Resources["InputTextColor"];
            Pword.Foreground = brush;
            Login.Foreground = brush;
        }
        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.Pword.Tag = this.Pword.Password;
            var brush = (SolidColorBrush)Resources["InputTextColor"];
            Pword.Foreground = brush;
            Login.Foreground = brush;
        }



        private void Pword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Check();
        }

        private void CloseWindowHandler(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
