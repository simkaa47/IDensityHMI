using IDensity.AddClasses;
using IDensity.AddClasses.Settings;
using IDensity.ViewModels.Commands;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace IDensity.ViewModels
{
    public class MeasProcessVm:PropertyChangedBase
    {
        public MeasProcessVm(VM vM)
        {
            VM = vM;
            Describe();
            StandVm = new StandarisationViewModel(VM);
            CalibrationVm = new CalibrationVm(VM);
        }
        public VM VM { get; }

        public StandarisationViewModel StandVm { get; }
        public CalibrationVm CalibrationVm { get; }


        #region Selected standartisation data
        /// <summary>
        /// Selected standartisation data
        /// </summary>
        private StandSettings _selectedStandartisation;
        /// <summary>
        /// Selected standartisation data
        /// </summary>
        public StandSettings SelectedStandartisation
        {
            get => _selectedStandartisation;
            set => Set(ref _selectedStandartisation, value);
        }
        #endregion


        #region Selected Temp Compensation index
        /// <summary>
        /// Selected Temp Compensation index
        /// </summary>
        private int _tempCompensationIndex;
        /// <summary>
        /// Selected Temp Compensation index
        /// </summary>
        public int TempCompensationIndex
        {
            get => _tempCompensationIndex;
            set => Set(ref _tempCompensationIndex, value);
        }
        #endregion


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
            set 
            { 
                if(value!=null) Set(ref _selectedProcess, value);
            } 
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
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.MeasProcCounterNum)));
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
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.MeasDuration)));
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
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.MeasDeep)));
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
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.MeasType)));
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
        }, canExecPar => SelectedProcess != null && VM.mainModel.Connecting.Value));
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
        }, canExecPar => SelectedProcess!= null && GetCommandCondition(SelectedProcess.FastChange.Activity)));
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
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.FastChange.Threshold)));
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
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.PipeDiameter)));
        #endregion


        #region Write temperature compensation
        /// <summary>
        /// Write temperature compensation
        /// </summary>
        RelayCommand _tempCompensationActivityWriteCommand;
        /// <summary>
        /// Write temperature compensation
        /// </summary>
        public RelayCommand TempCompensationActivityWriteCommand => _tempCompensationActivityWriteCommand ?? (_tempCompensationActivityWriteCommand = new RelayCommand(execPar => 
        {
            if (SelectedProcess is null || TempCompensationIndex < 0) return;
            var par = SelectedProcess.TempCompensations[TempCompensationIndex].Activity;
            if (par.ValidationOk) WriteTempCompensation("activity", par);

        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion

        #region Write temp comp coeff
        /// <summary>
        /// Write temp comp coeff
        /// </summary>
        RelayCommand _tempCompensationCoeffCommand;
        /// <summary>
        /// Write temp comp coeff
        /// </summary>
        public RelayCommand TempCompensationCoeffCommand => _tempCompensationCoeffCommand ?? (_tempCompensationCoeffCommand = new RelayCommand(execPar => 
        {
            if (SelectedProcess is null || TempCompensationIndex < 0) return;
            if (execPar is null) return;
            int i = 0;
            if (!int.TryParse(execPar.ToString(), out i)) return;
            var par = SelectedProcess.TempCompensations[TempCompensationIndex].Coeffs[i];
            if (par.ValidationOk) WriteTempCompensation(i.ToString(), par);
        }, canExecPar => true));
        #endregion




        void WriteTempCompensation<T>(string id, Parameter<T> par) where T : IComparable
        {
            var proc = SelectedProcess;
            string cmd = ($"*SETT,meas_proc={proc.Num}," +
                $"comp_temp={TempCompensationIndex}," +
                $"{((id=="activity" ? proc.TempCompensations[TempCompensationIndex].Activity.WriteValue : proc.TempCompensations[TempCompensationIndex].Activity.Value) ? 1 : 0)},0");
            var coeffs = SelectedProcess.TempCompensations[TempCompensationIndex].Coeffs;
            for (int i = 0; i < coeffs.Count; i++)
            {
                cmd += $",{(id == i.ToString() ? coeffs[i].WriteValue : coeffs[i].Value).ToStringPoint()}";
            }
            par.IsWriting = true;
            cmd += "#";
            VM.CommService.WriteMeasProcSettings(cmd, SelectedProcess.Num);
        }        

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

        }, canExec => SelectedProcess != null && GetCommandCondition(SelectedProcess.CalculationType)));
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

        }, canExec => SelectedProcess != null && VM.mainModel.Connecting.Value));
        #endregion

        #region Write calibration result
        /// <summary>
        /// Write calibration result
        /// </summary>
        RelayCommand _calibrResultWriteCommand;
        /// <summary>
        /// Write calibration result
        /// </summary>
        public RelayCommand CalibrResultWriteCommand => _calibrResultWriteCommand ?? (_calibrResultWriteCommand = new RelayCommand(execPar => 
        {
            WriteCalibrCurveData("result", SelectedProcess.CalibrCurve.Result);
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.CalibrCurve.Result)));
        #endregion

        #region Write calibration type
        /// <summary>
        /// Write calibration type
        /// </summary>
        RelayCommand _calibrTypeWriteCommand;
        /// <summary>
        /// Write calibration type
        /// </summary>
        public RelayCommand CalibrTypeWriteCommand => _calibrTypeWriteCommand ?? (_calibrTypeWriteCommand = new RelayCommand(execPar => 
        {
            WriteCalibrCurveData("type", SelectedProcess.CalibrCurve.Type);
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.CalibrCurve.Type)));
        #endregion

        #region Write calibration coeff
        /// <summary>
        /// Write calibration coeff
        /// </summary>
        RelayCommand _calibrCoeffWriteCommand;
        /// <summary>
        /// Write calibration coeff
        /// </summary>
        public RelayCommand CalibrCoeffWriteCommand => _calibrCoeffWriteCommand ?? (_calibrCoeffWriteCommand = new RelayCommand(execPar => 
        {
            int i = 0;
            if (!int.TryParse(execPar.ToString(), out i)) return;
            WriteCalibrCurveData(i.ToString(), SelectedProcess.CalibrCurve.Coeffs[i]);
        }, canExecPar => VM.mainModel.Connecting.Value && SelectedProcess != null));
        #endregion

        #region Write density liq settings
        /// <summary>
        /// Write density liq settings
        /// </summary>
        RelayCommand _densityLiqSettingsCommand;
        /// <summary>
        /// Write density liq settings
        /// </summary>
        public RelayCommand DensityLiqSettingsCommand => _densityLiqSettingsCommand ?? (_densityLiqSettingsCommand = new RelayCommand(execPar => 
        {
            var proc = SelectedProcess;
            var str = $"*SETT,meas_proc={proc.Num},dens_liq=0,{proc.DensityLiqD1.PhysValue.WriteValue}#";
            proc.DensityLiqD1.PhysValue.IsWriting = true; ;
            VM.CommService.Tcp.WriteMeasProcSettings(str, SelectedProcess.Num);
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.DensityLiqD1.PhysValue)));
        #endregion

        #region Write density solid settings
        /// <summary>
        /// Write density solid settings
        /// </summary>
        RelayCommand _densitySolidSettingsCommand;
        /// <summary>
        /// Write density solid settings
        /// </summary>
        public RelayCommand DensitySolidSettingsCommand => _densitySolidSettingsCommand ?? (_densitySolidSettingsCommand = new RelayCommand(execPar => 
        {
            var proc = SelectedProcess;
            var str = $"*SETT,meas_proc={proc.Num},dens_solid=0,{proc.DensitySolD2.PhysValue.WriteValue}#";
            proc.DensitySolD2.PhysValue.IsWriting = true; ;
            VM.CommService.Tcp.WriteMeasProcSettings(str, SelectedProcess.Num);
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.DensitySolD2.PhysValue)));
        #endregion

        #region Write steam compensation activity
        /// <summary>
        /// Write steam compensation activity
        /// </summary>
        RelayCommand _steamCompActivityWriteCommand;
        /// <summary>
        /// Write steam compensation activity
        /// </summary>
        public RelayCommand SteamCompActivityWriteCommand => _steamCompActivityWriteCommand ?? (_steamCompActivityWriteCommand = new RelayCommand(execPar => 
        {
            WriteSteamCompensation("activity", SelectedProcess.SteamCompensation.Activity);
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.SteamCompensation.Activity)));
        #endregion

        #region Write steam compensation source
        /// <summary>
        /// Write steam compensation source
        /// </summary>
        RelayCommand _steamCompSourceWriteCommand;
        /// <summary>
        /// Write steam compensation source
        /// </summary>
        public RelayCommand SteamCompSourceWriteCommand => _steamCompSourceWriteCommand ?? (_steamCompSourceWriteCommand = new RelayCommand(execPar => 
        {
            WriteSteamCompensation("source", SelectedProcess.SteamCompensation.Sourse);
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.SteamCompensation.Sourse)));
        #endregion

        #region Write steam compensation A
        /// <summary>
        /// Write steam compensation A
        /// </summary>
        RelayCommand _steamCompensationWriteACommand;
        /// <summary>
        /// Write steam compensation A
        /// </summary>
        public RelayCommand SteamCompensationWriteACommand => _steamCompensationWriteACommand ?? (_steamCompensationWriteACommand = new RelayCommand(execPar => 
        {
            WriteSteamCompensation("A", SelectedProcess.SteamCompensation.A);
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.SteamCompensation.A)));
        #endregion

        #region Write steam compensation B
        /// <summary>
        /// Write steam compensation B
        /// </summary>
        RelayCommand _steamCompensationWriteBCommand;
        /// <summary>
        /// Write steam compensation B
        /// </summary>
        public RelayCommand SteamCompensationWriteBCommand => _steamCompensationWriteBCommand ?? (_steamCompensationWriteBCommand = new RelayCommand(execPar =>
        {
            WriteSteamCompensation("B", SelectedProcess.SteamCompensation.B);
        }, canExecPar => SelectedProcess != null && GetCommandCondition(SelectedProcess.SteamCompensation.B)));
        #endregion



        private void WriteSteamCompensation<T>(string id, Parameter<T> par) where T : IComparable
        {
            var proc = SelectedProcess;
            var steam = SelectedProcess.SteamCompensation;
            var str = $"*SETT,meas_proc={proc.Num}," +
                $"comp_steam={((id == "activity" ? steam.Activity.WriteValue : steam.Activity.Value) ? 1 : 0)}," +
                $"{steam.MeasUnitNum.WriteValue}," +
                $"{(id == "source" ? steam.Sourse.WriteValue : steam.Sourse.Value)}," +
                $"{(id == "A" ? steam.A.WriteValue : steam.A.Value).ToStringPoint()}," +
                $"{(id == "B" ? steam.B.WriteValue : steam.B.Value).ToStringPoint()}#";
            par.IsWriting = true;
            VM.CommService.Tcp.WriteMeasProcSettings(str, SelectedProcess.Num);
        }
       

        private void WriteCalibrCurveData<T>(string key, Parameter<T> par) where T : IComparable
        {
            var curve = SelectedProcess.CalibrCurve;
            var str = $"*SETT,meas_proc={SelectedProcess.Num},calib_curve={(key=="type" ? curve.Type.WriteValue : curve.Type.Value)},0,";
            for(int i = 0;i<SelectedProcess.CalibrCurve.Coeffs.Count;i++)
            {
                str += (key == i.ToString() ? curve.Coeffs[i].WriteValue : curve.Coeffs[i].Value).ToStringPoint();
                str += ",";
            }
            str += $"{(key == "result" ? curve.Result.WriteValue : curve.Result.Value)}#";
            par.IsWriting = true;
            VM.CommService.Tcp.WriteMeasProcSettings(str, SelectedProcess.Num);

        }

        private bool GetCommandCondition<T>(Parameter<T> par) where T:IComparable
        {
            return VM.mainModel.Connecting.Value && SelectedProcess != null && par.ValidationOk;
        }


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
