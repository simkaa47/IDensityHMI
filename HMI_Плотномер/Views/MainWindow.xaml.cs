using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using Microsoft.Win32;

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

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            var compobox = sender as ComboBox;
            if (compobox != null)
            {
                var arr = SerialPort.GetPortNames();
                compobox.ItemsSource = arr;
            }            
        }
        private void FileDialogOpen(TextBlock tb)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                Multiselect = true,
                Title = "Выберите файл"
            };
            fileDialog.Filter = "  Текстовые файлы (*.txt)|*.txt";
            Nullable<bool> dialogOK = fileDialog.ShowDialog();


            if (dialogOK == true)
            {
                tb.Text = fileDialog.FileName;

            }
        }
        private void BrowseLogPath(object sender, RoutedEventArgs e)
        {
            FileDialogOpen(LogPath);
        }
    }
}
