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
            output.AmTestValue.Value = output.AmTestValue.WriteValue;
            VM.CommService.SetTestValueAm(output.GroupNum, output.ModulNum, output.AmTestValue.WriteValue);
            
        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion

        #region Write value type(test or measure value)
        /// <summary>
        /// Write value type(test or measure value)
        /// </summary>
        RelayCommand _writeValueTypeCommand;
        /// <summary>
        /// Write value type(test or measure value)
        /// </summary>
        public RelayCommand WriteValueTypeCommand => _writeValueTypeCommand ?? (_writeValueTypeCommand = new RelayCommand(execPar => 
        {
            if (!(execPar is AnalogOutput output)) return;
            if (!output.DacType.ValidationOk) return;
            VM.CommService.Tcp.SendAnalogOutSwttings(output.GroupNum, output.ModulNum, output);
            output.DacType.IsWriting = true;
        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion

        #region Write meas num
        /// <summary>
        /// Write meas num
        /// </summary>
        RelayCommand _measNumWriteCommand;
        /// <summary>
        /// Write meas num
        /// </summary>
        public RelayCommand MeasNumWriteCommand => _measNumWriteCommand ?? (_measNumWriteCommand = new RelayCommand(execPar => 
        {
            if (!(execPar is AnalogOutput output)) return;
            if (!output.AnalogMeasProcNdx.ValidationOk) return;
            VM.CommService.Tcp.SendAnalogOutSwttings(output.GroupNum, output.ModulNum, output);
            output.AnalogMeasProcNdx.IsWriting = true;
        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion


        #region Write low limit value
        /// <summary>
        /// Write low limit value
        /// </summary>
        RelayCommand _lowLimitValueWriteCommand;
        /// <summary>
        /// Write low limit value
        /// </summary>
        public RelayCommand LowLimitValueWriteCommand => _lowLimitValueWriteCommand ?? (_lowLimitValueWriteCommand = new RelayCommand(execPar => 
        {
            if (!(execPar is AnalogOutput output)) return;
            if (!output.DacLowLimit.ValidationOk) return;
            VM.CommService.Tcp.SendAnalogOutSwttings(output.GroupNum, output.ModulNum, output);
            output.DacLowLimit.IsWriting = true;
        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion


        #region Write high limi value
        /// <summary>
        /// Write high limi value
        /// </summary>
        RelayCommand _highLimitValueWriteCommand;
        /// <summary>
        /// Write high limi value
        /// </summary>
        public RelayCommand HighLimitValueWriteCommand => _highLimitValueWriteCommand ?? (_highLimitValueWriteCommand = new RelayCommand(execPar => 
        {
            if (!(execPar is AnalogOutput output)) return;
            if (!output.DacHighLimit.ValidationOk) return;
            VM.CommService.Tcp.SendAnalogOutSwttings(output.GroupNum, output.ModulNum, output);
            output.DacHighLimit.IsWriting = true;
        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion

        #region Write low limit Ma
        /// <summary>
        /// Write low limit Ma
        /// </summary>
        RelayCommand _lowLimitMaWriteCommand;
        /// <summary>
        /// Write low limit Ma
        /// </summary>
        public RelayCommand LowLimitMaWriteCommand => _lowLimitMaWriteCommand ?? (_lowLimitMaWriteCommand = new RelayCommand(execPar => 
        {
            if (!(execPar is AnalogOutput output)) return;
            if (!output.DacLowLimitMa.ValidationOk) return;
            VM.CommService.Tcp.SendAnalogOutSwttings(output.GroupNum, output.ModulNum, output);
            output.DacLowLimitMa.IsWriting = true;
        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion

        #region Write high limit ma
        /// <summary>
        /// Write high limit ma
        /// </summary>
        RelayCommand _highLimitMaWriteCommand;
        /// <summary>
        /// Write high limit ma
        /// </summary>
        public RelayCommand HighLimitMaWriteCommand => _highLimitMaWriteCommand ?? (_highLimitMaWriteCommand = new RelayCommand(execPar => 
        {
            if (!(execPar is AnalogOutput output)) return;
            if (!output.DacHighLimitMa.ValidationOk) return;
            VM.CommService.Tcp.SendAnalogOutSwttings(output.GroupNum, output.ModulNum, output);
            output.DacHighLimitMa.IsWriting = true;
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
