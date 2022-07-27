using IDensity.AddClasses;
using IDensity.ViewModels.Commands;
using IDensity.Views.CheckMaster;
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


        #region Старт мастера проверк прибора
        /// <summary>
        /// Старт мастера проверк прибора
        /// </summary>
        RelayCommand _startCheckMasterCommand;
        /// <summary>
        /// Старт мастера проверк прибора
        /// </summary>
        public RelayCommand StartCheckMasterCommand => _startCheckMasterCommand ?? (_startCheckMasterCommand = new RelayCommand(execPar => 
        { 
            CheckMasterVm checkMasterVm = new CheckMasterVm(VM);
            CheckMasterWindow checkMasterWindow = new CheckMasterWindow(checkMasterVm);
            checkMasterWindow.Show();
        
        }, canExecPar => true));
        #endregion







    }
}
