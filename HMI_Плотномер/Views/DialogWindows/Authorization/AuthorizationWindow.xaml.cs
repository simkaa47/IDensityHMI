using IDensity.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace IDensity.Core.Views.DialogWindows.Authorization
{
    /// <summary>
    /// Interaction logic for AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        VM vM;
        LoginRequest _loginRequest;
        public AuthorizationWindow(VM vM)
        {
            InitializeComponent();
            if (vM == null) return;
            else this.vM = vM;
            this.DataContext = vM;
            _loginRequest = (LoginRequest)Resources["LoginRequest"];
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
                _loginRequest.IsError = true;
            }
        }



        
        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.Pword.Tag = this.Pword.Password;
            _loginRequest.IsError = false;
        }



        private void Pword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Check();
        }

        private void CloseWindowHandler(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Login_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _loginRequest.IsError = false;
        }
    }
}
