using IDensity.ViewModels.MasrerSettings;
using System.Windows;

namespace IDensity.Views.CheckMaster
{
    /// <summary>
    /// Interaction logic for CheckMasterWindow.xaml
    /// </summary>
    public partial class CheckMasterWindow : Window
    {
        private readonly CheckMasterVm _checkMasterVm;        

        public CheckMasterWindow(CheckMasterVm checkMasterVm)
        {
            _checkMasterVm = checkMasterVm;
            InitializeComponent();
            this.DataContext = checkMasterVm;
            this.Content.Content = new StartMasterPage();
            _checkMasterVm.Stage = CheckMasterStates.Start;
            Describe();


        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            switch (_checkMasterVm.Stage)
            {                
                case CheckMasterStates.Success:
                case CheckMasterStates.CancelByCommunicate:
                case CheckMasterStates.CancelByUser:
                    this.Close();
                    break;
                default:
                    break;
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void Describe()
        {
            _checkMasterVm.PropertyChanged += (o, e) => 
            {
                if (e.PropertyName == nameof(_checkMasterVm.Stage))
                {
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        switch (_checkMasterVm.Stage)
                        {
                            case CheckMasterStates.Start:
                                break;
                            case CheckMasterStates.Process:
                                this.Content.Content = new MainProcessPage(_checkMasterVm);
                                break;
                            case CheckMasterStates.CancelByUser:
                                Content.Content = new CancelByUserPage(_checkMasterVm);
                                break;
                            case CheckMasterStates.CancelByCommunicate:
                                Content.Content = new CancelByCommPage(_checkMasterVm);
                                break;
                            case CheckMasterStates.CancelByDelay:
                                break;
                            case CheckMasterStates.Success:
                                Content.Content = new SuccessPage(_checkMasterVm);
                                break;
                            default:
                                break;
                        }
                    });                    
                }
            };
        }
    }
}
