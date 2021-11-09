using HMI_Плотномер.ViewModels;
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

namespace HMI_Плотномер.Views
{
    /// <summary>
    /// Логика взаимодействия для Password.xaml
    /// </summary>
    public partial class Password : Window
    {
        VM vM;       

        internal Password(VM vM)
        {
            InitializeComponent();
            if (vM == null) return;
            else this.vM = vM;
            this.DataContext = vM;
        }

        private void EnterClick(object sender, RoutedEventArgs e)
        {
            var user = vM.Users.Data.Where(user => user.Password == Pword.Password && user.Login == Login.Text)
                .FirstOrDefault();
            if (user != null)
            {
                vM.CurUser = user;                
            }
            else
            {
                Pword.Foreground = Brushes.Red;
                Login.Foreground = Brushes.Red;
            }
        }



        private void Login_MouseEnter(object sender, MouseEventArgs e)
        {
            Pword.Foreground = Brushes.Black;
            Login.Foreground = Brushes.Black;
        }
        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.Pword.Tag = this.Pword.Password;
            Pword.Foreground = Brushes.Black;
            Login.Foreground = Brushes.Black;
        }
    }
}
