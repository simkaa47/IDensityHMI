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

        #region Write counter number
        /// <summary>
        /// Write counter number
        /// </summary>
        RelayCommand _writeCounterNumCommand;
        /// <summary>
        /// Write counter number
        /// </summary>
        public RelayCommand WriteCounterNumCommand => _writeCounterNumCommand ?? (_writeCounterNumCommand = new RelayCommand(execPar => 
        { 
            var str = $"*SETT,meas_proc={SelectedProcess.Num},cntr={SelectedProcess.MeasProcCounterNum.WriteValue}#";
            SelectedProcess.MeasProcCounterNum.IsWriting= true;
            VM.CommService.Tcp.WriteMeasProcSettings(str , SelectedProcess.Num);
        }, canExecPar => SelectedProcess.MeasProcCounterNum.ValidationOk));
        #endregion

        #region WriteMeasDuration
        /// <summary>
        /// WriteMeasDuration
        /// </summary>
        RelayCommand _writeMeasDurationCommand;
        /// <summary>
        /// WriteMeasDuration
        /// </summary>
        public RelayCommand WriteMeasDurationCommand => _writeMeasDurationCommand ?? (_writeMeasDurationCommand = new RelayCommand(execPar => 
        {
            var str = $"*SETT,meas_proc={SelectedProcess.Num},duration={SelectedProcess.MeasDuration.WriteValue*10}#";
            SelectedProcess.MeasDuration.IsWriting = true;
            VM.CommService.Tcp.WriteMeasProcSettings(str, SelectedProcess.Num);
        }, canExecPar => SelectedProcess.MeasDuration.ValidationOk));
        #endregion

        #region Write Meas Deep
        /// <summary>
        /// Write Meas Deep
        /// </summary>
        RelayCommand _writeMeasDeepCommand;
        /// <summary>
        /// Write Meas Deep
        /// </summary>
        public RelayCommand WriteMeasDeepCommand => _writeMeasDeepCommand ?? (_writeMeasDeepCommand = new RelayCommand(execPar => 
        {
            var str = $"*SETT,meas_proc={SelectedProcess.Num},aver_depth={SelectedProcess.MeasDeep.WriteValue}#";
            SelectedProcess.MeasDeep.IsWriting = true;
            VM.CommService.Tcp.WriteMeasProcSettings(str, SelectedProcess.Num);
        }, canExecPar => SelectedProcess.MeasDeep.ValidationOk));
        #endregion

        #region Change type of measure
        /// <summary>
        /// Change type of measure
        /// </summary>
        RelayCommand _changeMeasTypeCommand;
        /// <summary>
        /// Change type of measure
        /// </summary>
        public RelayCommand ChangeMeasTypeCommand => _changeMeasTypeCommand ?? (_changeMeasTypeCommand = new RelayCommand(execPar =>
        {
            var str = $"*SETT,meas_proc={SelectedProcess.Num},type={SelectedProcess.MeasType.WriteValue}#";
            SelectedProcess.MeasType.IsWriting = true;
            VM.CommService.Tcp.WriteMeasProcSettings(str, SelectedProcess.Num);
        }, canExecPar => true));
        #endregion

        #region Write Activity Command
        /// <summary>
        /// Write Activity Command
        /// </summary>
        RelayCommand _writeActivityCommand;
        /// <summary>
        /// Write Activity Command
        /// </summary>
        public RelayCommand WriteActivityCommand => _writeActivityCommand ?? (_writeActivityCommand = new RelayCommand(execPar => 
        {
            VM.CommService.SetMeasProcActivity();
        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion

        #region Write Fast Change Settings Command
        /// <summary>
        /// Write Fast Change Settings Command
        /// </summary>
        RelayCommand _writeFastChangeActivityCommand;
        /// <summary>
        /// Write Fast Change Settings Command
        /// </summary>
        public RelayCommand WriteFastChangeActivityCommand => _writeFastChangeActivityCommand ?? (_writeFastChangeActivityCommand = new RelayCommand(execPar => 
        {
            var fast = SelectedProcess.FastChange;
            var str = $"*SETT,meas_proc={SelectedProcess.Num},fast_chg={(fast.Activity.WriteValue ? 1 : 0)},{fast.Threshold.Value}#";
            fast.Activity.IsWriting = true;
            VM.CommService.Tcp.WriteMeasProcSettings(str, SelectedProcess.Num);            
        }, canExecPar => true));
        #endregion

        #region Write Fast Change Settings Command
        /// <summary>
        /// Write Fast Change Settings Command
        /// </summary>
        RelayCommand _writeFastChangeThresholdCommand;
        /// <summary>
        /// Write Fast Change Settings Command
        /// </summary>
        public RelayCommand WriteFastChangeThresholdCommand => _writeFastChangeThresholdCommand ?? (_writeFastChangeThresholdCommand = new RelayCommand(execPar =>
        {
            var fast = SelectedProcess.FastChange;
            var str = $"*SETT,meas_proc={SelectedProcess.Num},fast_chg={(fast.Activity.Value ? 1 : 0)},{fast.Threshold.WriteValue}#";
            fast.Threshold.IsWriting = true;
            VM.CommService.Tcp.WriteMeasProcSettings(str, SelectedProcess.Num);
        }, canExecPar => SelectedProcess.FastChange.Threshold.ValidationOk));
        #endregion

        #region WritePipeDiameter
        /// <summary>
        /// WritePipeDiameter
        /// </summary>
        RelayCommand _writePipeDiameterCommand;
        /// <summary>
        /// WritePipeDiameter
        /// </summary>
        public RelayCommand WritePipeDiameterCommand => _writePipeDiameterCommand ?? (_writePipeDiameterCommand = new RelayCommand(execPar => 
        {            
            var str = $"*SETT,meas_proc={SelectedProcess.Num},pipe_diam={(ushort)(SelectedProcess.PipeDiameter.WriteValue * 10)}#";
            SelectedProcess.PipeDiameter.IsWriting = true;
            VM.CommService.Tcp.WriteMeasProcSettings(str, SelectedProcess.Num);
        }, canExecPar => SelectedProcess.PipeDiameter.ValidationOk));
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
            foreach (var att in SelectedProcess.AttCoeffs)
            {
                att.IsWriting = true;
            }
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
            SelectedProcess.CalculationType.IsWriting = true;

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
                volume.IsWriting = true;
            }
            cmd = cmd.Remove(cmd.Length - 1);
            cmd += "#";
            VM.CommService.WriteMeasProcSettings(cmd, SelectedProcess.Num);

        }, canExec => VM.mainModel.Connecting.Value));
        #endregion

        #region Скопировтаь измерительный процесс
        private RelayCommand _copyMeasProcessCommand;
        public RelayCommand CopyMeasProcessCommand => _copyMeasProcessCommand ?? (_copyMeasProcessCommand = new RelayCommand(exec =>
          {
              int par = (int)exec;
              CopyMeasProcess(SelectedProcess, (ushort)par);
          }, canExec => VM.mainModel.Connecting.Value));
        #endregion

        

        void Describe()
        {
            foreach (var mp in VM.mainModel.MeasProcSettings)
            {
                mp.NeedWriteEvent += VM.CommService.WriteMeasProcSettings;
                
                mp.NeedMakeStand += VM.CommService.MakeStand;
                mp.StandFinishEvent += (num) => VM.CommService.Tcp.GetMeasSettingsExternal(num);
                mp.NeedMakeSingleMeasEvent += VM.CommService.MakeSingleMeasure;
                mp.SingleMeasEventFinishedEvent += (num) => VM.CommService.Tcp.GetMeasSettingsExternal(num);
            }
        }

        public void CopyMeasProcess(MeasProcSettings settings, ushort number)
        {
            var header = $"*SETT,meas_proc={number}";
            var str = header + $",cntr={settings.MeasProcCounterNum.Value}";
            str += $",{CopyStandartisation(settings)}";
            str += $",{CopyCalibrCurve(settings)}";
            str += $",{CopySingleMeasResults(settings)}";
            VM.CommService.Tcp.WriteMeasProcSettings(str + "#", number);
            str = header + $",dens_liq={DensitySettCopy(settings.DensityLiqD1)},dens_solid={DensitySettCopy(settings.DensitySolD2)}";
            str += $",{CopyTempCompensation(settings)}";
            str += $",{CopySteamCompensation(settings)}";
            str += $",aver_depth={settings.MeasDeep.Value}";
            str += $",duration={settings.MeasDuration.Value*10}";
            str += $",type={settings.MeasType.Value}";
            str += $",{CopyFastChanges(settings)}";
            str += $",pipe_diam={(ushort)(settings.PipeDiameter.Value * 10)}";
            str += $",att_coeffs={settings.AttCoeffs[0].Value.ToStringPoint()},{settings.AttCoeffs[1].Value.ToStringPoint()}";
            str += $",calc_type={settings.CalculationType.Value}";
            str += $",{CopyVolumeCoeffs(settings)}";
            VM.CommService.Tcp.WriteMeasProcSettings(str + "#", number);
        }

        private string CopyStandartisation(MeasProcSettings settings)
        {
            string arg = string.Empty;
            foreach (var std in settings.MeasStandSettings)
            {
                arg+= $"std={std.Id},0,{std.StandDuration.Value}," +
                    $"{std.LastStandDate.Value:dd:MM:yy}," +
                    $"{std.StandResult.Value.ToStringPoint()}," +
                    $"{std.StandPhysValue.Value.ToStringPoint()}," +
                    $"{std.HalfLifeCorr.Value.ToStringPoint()},";
            }
            arg = arg.Remove(arg.Length - 1);
            return arg;
        }

        private string CopyCalibrCurve(MeasProcSettings settings)
        {
            var curve = settings.CalibrCurve;
            var arg = $"calib_curve={curve.Type.WriteValue},0";
            foreach (var coeff in curve.Coeffs)
            {
                arg += "," + coeff.Value.ToStringPoint();
            }
            arg += $",{curve.Result.Value}";
            return arg;
        }

        private string CopySingleMeasResults(MeasProcSettings settings)
        {
            var arg = "calib_src=";
            for(int j = 0; j < MeasProcSettings.SingleMeasResCount; j++)
            {
                arg += $"{settings.SingleMeasResults[j].Date.Value.ToString("dd:MM:yy")},{settings.SingleMeasResults[j].Weak.Value.ToStringPoint()},{settings.SingleMeasResults[j].CounterValue.Value.ToStringPoint()},";
            }
            arg = arg.Remove(arg.Length - 1);
            return arg;
        }

        private string DensitySettCopy(DensitySett sett)
        {
            return $"0,{sett.PhysValue.Value.ToStringPoint()}";
        }

        private string CopyTempCompensation(MeasProcSettings settings)
        {
            var arg = string.Empty;
            foreach (var comp in settings.TempCompensations)
            {
                arg += $"comp_temp={comp.Index},{(comp.Activity.Value ? 1:0)},0";
                foreach (var coeff in comp.Coeffs)
                {
                    arg += "," + coeff.Value.ToStringPoint();
                }
                arg += ",";
            }
            arg = arg.Remove(arg.Length - 1);
            return arg;
        }
        private string CopySteamCompensation(MeasProcSettings settings)
        {
            var arg = string.Empty;
            var comp = settings.SteamCompensation;
            arg += $"comp_temp={(comp.Activity.Value ? 1 : 0)}," +
                $"{comp.MeasUnitNum.Value},{comp.Sourse.Value}," +
                $"{comp.A.Value.ToStringPoint()}," +
                $"{comp.B.Value.ToStringPoint()}";            
            return arg;
        }

        private string CopyFastChanges(MeasProcSettings settings)
        {
            var fast = settings.FastChange;
            return $"fast_chg={(fast.Activity.Value ? 1 : 0)},{fast.Threshold.Value.ToString()}";
        }

        private string CopyVolumeCoeffs(MeasProcSettings settings)
        {
            var arg = ",volume_coeffs=";
            foreach (var volume in settings.VolumeCoeefs)
            {
                arg += $"{volume.Value.ToStringPoint()},";
            }
            arg = arg.Remove(arg.Length - 1);
            return arg;
        }
    }
}
