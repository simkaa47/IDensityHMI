using IDensity.AddClasses;
using IDensity.AddClasses.Settings;
using IDensity.ViewModels.Commands;

namespace IDensity.ViewModels
{
    class StandarisationViewModel:PropertyChangedBase
    {
        private readonly VM _vM;

        public StandarisationViewModel(VM vM)
        {
            _vM = vM;
        }
        #region Commands

        #region Write Stand Duration Command
        /// <summary>
        /// Write Stand Duration Command
        /// </summary>
        RelayCommand _durationWriteCommand;
        /// <summary>
        /// Write Stand Duration Command
        /// </summary>
        public RelayCommand DurationWriteCommand => _durationWriteCommand ?? (_durationWriteCommand = new RelayCommand(execPar =>
        {

        }, canExecPar => _vM.mainModel.Connecting.Value));
        #endregion

        #endregion




        


    }
}
