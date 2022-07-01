using IDensity.AddClasses;
using IDensity.ViewModels.Commands;
using IDensity.Views.SettingsMaster.ConnectionMaster;

namespace IDensity.ViewModels.MasrerSettings
{
    public class MasterSettingsViewModel : PropertyChangedBase
    {
        public MasterSettingsViewModel(VM vM)
        {
            VM = vM;
        }

        public VM VM { get; }        

        public MasterConnectViewModel MasterConnect { get; private set; }


        #region Старт мастера настроек соединения с плотномером
        /// <summary>
        /// Старт мастера настроек соединения с плотномером
        /// </summary>
        RelayCommand _startMasterConnectCommand;
        /// <summary>
        /// Старт мастера настроек соединения с плотномером
        /// </summary>
        public RelayCommand StartMasterConnectCommand => _startMasterConnectCommand ?? (_startMasterConnectCommand = new RelayCommand(execPar => 
        {
            MasterConnect = new MasterConnectViewModel(VM);
            ConnectionWindowMain masterConWindow = new ConnectionWindowMain(MasterConnect);
            masterConWindow.Show();
        }, canExecPar => true));
        #endregion






    }
}
