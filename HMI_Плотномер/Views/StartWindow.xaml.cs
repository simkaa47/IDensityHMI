using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IDensity.Core.Views
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            SoftVersion.Text = "VERSION " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private async  void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowNameButtonText.Text = "Загрузка...";
            ShowNameButtonText.Foreground = Brushes.Black;
            ShowNameButton.IsEnabled = false;            
            await Task.Run(() => 
            {
                Thread.Sleep(200);
                Application.Current.Dispatcher.Invoke(new Action(() => 
                {
                    var main = new MainWindow();
                    main.Show();
                    Application.Current.MainWindow = main;
                    ShowNameButton.IsEnabled = true;
                    this.Close();
                }));                          

            });
           


        }
    }
}
