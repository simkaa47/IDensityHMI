using IDensity.AddClasses;
using IDensity.AddClasses.Settings;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.ViewModels
{
    public class MeasProcessVm:PropertyChangedBase
    {
        public MeasProcessVm(VM vM)
        {
            VM = vM;
            Describe();
        }
        public VM VM { get; }

        #region Выбраннный процесс
        /// <summary>
        /// Выбраннный процесс
        /// </summary>
        private MeasProcSettings _selectedProcess;
        /// <summary>
        /// Выбраннный процесс
        /// </summary>
        public MeasProcSettings SelectedProcess
        {
            get => _selectedProcess;
            set => Set(ref _selectedProcess, value);
        }
        #endregion

        #region Записать настройки компенсации температуры
        RelayCommand _writeTempCompensationCommand;
        public RelayCommand WriteTempCompensationCommand => _writeTempCompensationCommand ?? (_writeTempCompensationCommand = new RelayCommand(exec =>
        {
            if (!(exec is int index)) return;
            if (SelectedProcess is null) return;
            string cmd = ($"*SETT,meas_proc={SelectedProcess.Num},comp_temp={index},{(SelectedProcess.TempCompensations[index].Activity.WriteValue ? 1 : 0)},0");
            foreach (var coeff in SelectedProcess.TempCompensations[index].Coeffs)
            {
                cmd += $",{coeff.WriteValue.ToStringPoint()}";
            }
            cmd += "#";
            VM.CommService.WriteMeasProcSettings(cmd, SelectedProcess.Num);


        }, canExec => VM.mainModel.Connecting.Value));
        #endregion

        #region Записать-кты ослабления
        RelayCommand _writeAttenuationCommand;
        public RelayCommand WriteAttenuationCommand => _writeAttenuationCommand ?? (_writeAttenuationCommand = new RelayCommand(exec =>
        {            
            if (SelectedProcess is null) return;
            string cmd = ($"*SETT,meas_proc={SelectedProcess.Num},att_coeffs={(SelectedProcess.AttCoeffs[0].WriteValue.ToStringPoint())},{(SelectedProcess.AttCoeffs[1].WriteValue.ToStringPoint())}");           
            cmd += "#";
            VM.CommService.WriteMeasProcSettings(cmd, SelectedProcess.Num);


        }, canExec => VM.mainModel.Connecting.Value));
        #endregion

        #region Записать- тип расчета
        RelayCommand _writeCalcTypeCommand;
        public RelayCommand WriteCalcTypeCommand => _writeCalcTypeCommand ?? (_writeCalcTypeCommand = new RelayCommand(exec =>
        {
            
            if (SelectedProcess is null) return;
            string cmd = ($"*SETT,meas_proc={SelectedProcess.Num},calc_type={SelectedProcess.CalculationType.WriteValue}");
            cmd += "#";
            VM.CommService.WriteMeasProcSettings(cmd, SelectedProcess.Num);


        }, canExec => VM.mainModel.Connecting.Value));
        #endregion

        #region Записать к-ты расчета обьема
        RelayCommand _writeVolumeCoeffsCommand;
        public RelayCommand WriteVolumeCoeffsCommand => _writeVolumeCoeffsCommand ?? (_writeVolumeCoeffsCommand = new RelayCommand(exec =>
        {

            if (SelectedProcess is null) return;
            string cmd = ($"*SETT,meas_proc={SelectedProcess.Num}");
            cmd += ",volume_coeffs=";
            foreach (var volume in SelectedProcess.VolumeCoeefs)
            {
                cmd += $"{volume.WriteValue.ToStringPoint()},";
            }
            cmd = cmd.Remove(cmd.Length - 1);
            cmd += "#";
            VM.CommService.WriteMeasProcSettings(cmd, SelectedProcess.Num);

        }, canExec => VM.mainModel.Connecting.Value));
        #endregion
        void Describe()
        {
            foreach (var mp in VM.mainModel.MeasProcSettings)
            {
                mp.NeedWriteEvent += VM.CommService.WriteMeasProcSettings;
                mp.IsActive.CommandEcecutedEvent += (s) => VM.CommService.SetMeasProcActivity();
                mp.NeedMakeStand += VM.CommService.MakeStand;
                mp.StandFinishEvent += (num) => VM.CommService.Tcp.GetMeasSettingsExternal(num);
                mp.NeedMakeSingleMeasEvent += VM.CommService.MakeSingleMeasure;
                mp.SingleMeasEventFinishedEvent += (num) => VM.CommService.Tcp.GetMeasSettingsExternal(num);
            }
        }
    }
}
