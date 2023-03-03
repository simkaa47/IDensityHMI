using IDensity.AddClasses;
using IDensity.Models;
using IDensity.ViewModels.Commands;
using System.Collections.Generic;
using System.Linq;

namespace IDensity.ViewModels
{
    public class AnalogVm:PropertyChangedBase
    {
        public AnalogVm(VM vM)
        {
            VM = vM;           

        }

        #region Команды

        #region Switch power
        /// <summary>
        /// Switch power
        /// </summary>
        RelayCommand _swithPowerCommand;
        /// <summary>
        /// Switch power
        /// </summary>
        public RelayCommand SwithPowerCommand => _swithPowerCommand ?? (_swithPowerCommand = new RelayCommand(execPar => 
        {
            if (!(execPar is AnalogData analog)) return;
            VM.CommService.SwitchAm(analog.GroupNum, analog.ModulNum, !analog.CommState.Value);
        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion

        #region AI switch activity
        /// <summary>
        /// AI switch activity
        /// </summary>
        RelayCommand _aiSwitchActivityCommand;
        /// <summary>
        /// AI switch activity
        /// </summary>
        public RelayCommand AiSwitchActivityCommand => _aiSwitchActivityCommand ?? (_aiSwitchActivityCommand = new RelayCommand(execPar => 
        {
            if (!(execPar is AnalogInput input)) return;
            VM.CommService.ChangeAdcAct(input.GroupNum, input.ModulNum, input);
            input.Activity.IsWriting = true;

        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion

        #region AO switch activity
        /// <summary>
        /// AI switch activity
        /// </summary>
        RelayCommand _aoSwitchActivityCommand;
        /// <summary>
        /// AI switch activity
        /// </summary>
        public RelayCommand AoSwitchActivityCommand => _aoSwitchActivityCommand ?? (_aoSwitchActivityCommand = new RelayCommand(execPar =>
        {
            if (!(execPar is AnalogOutput output)) return;
            VM.CommService.ChangeDacAct(output.GroupNum, output.ModulNum, output);
            output.Activity.IsWriting = true;

        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion


        #region Send Test value
        /// <summary>
        /// Send Test value
        /// </summary>
        RelayCommand _sendTestValueCommand;
        /// <summary>
        /// Send Test value
        /// </summary>
        public RelayCommand SendTestValueCommand => _sendTestValueCommand ?? (_sendTestValueCommand = new RelayCommand(execPar => 
        {
            if (!(execPar is AnalogOutput output)) return;
            if (!output.AmTestValue.ValidationOk) return;
            output.AmTestValue.Value = output.AmTestValue.Value;
            VM.CommService.SetTestValueAm(output.GroupNum, output.ModulNum, output.AmTestValue.WriteValue);
            
        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion

        #endregion

        public VM VM { get; }

       

        #region Аналоговые входы
        public List<AnalogInput> AnalogInputs => VM.mainModel.AnalogGroups.Select(g => g.AI).ToList();
        #endregion

        #region Аналоговые выходы
        public List<AnalogOutput> AnalogOutputs => VM.mainModel.AnalogGroups.Select(g => g.AO).ToList();
        #endregion
    }
}
