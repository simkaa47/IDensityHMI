using IDensity.ViewModels.MasrerSettings;
using System.Windows;

namespace IDensity.Views.SettingsMaster.ConnectionMaster
{
    /// <summary>
    /// Interaction logic for ConnectionWindowMain.xaml
    /// </summary>
    public partial class ConnectionWindowMain : Window
    {
        private readonly MasterConnectViewModel _masterConnectViewModel;

        public ConnectionWindowMain(MasterConnectViewModel masterConnectViewModel)
        {
            this.DataContext = masterConnectViewModel;

            InitializeComponent();
            _masterConnectViewModel = masterConnectViewModel;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _masterConnectViewModel.CancelMaster();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
