using IDensity.ViewModels.MasrerSettings;
using System.Windows.Controls;

namespace IDensity.Views.CheckMaster
{
    /// <summary>
    /// Interaction logic for MainProcessPage.xaml
    /// </summary>
    public partial class MainProcessPage : Page
    {
        public MainProcessPage(CheckMasterVm checkMasterVm)
        {
            InitializeComponent();
            DataContext = checkMasterVm;
        }
        
    }
}
