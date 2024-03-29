﻿using IDensity.Core.Extentions;
using IDensity.Core.Models.MeasProcess;
using IDensity.DataAccess;
using IDensity.ViewModels.Commands;
using System.Timers;

namespace IDensity.ViewModels
{
    public class StandarisationViewModel : PropertyChangedBase
    {
        private readonly VM _vM;

        public StandarisationViewModel(VM vM)
        {
            _vM = vM;
        }

        #region Standartisation flag
        /// <summary>
        /// Standartisation flag
        /// </summary>
        private bool _isStandartisation;
        /// <summary>
        /// Standartisation flag
        /// </summary>
        public bool IsStandartisation
        {
            get => _isStandartisation;
            set => Set(ref _isStandartisation, value);
        }
        #endregion

        Timer standTimer = new Timer();


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
            var proc = _vM.MeasProcessVm.SelectedProcess;
            var stand = _vM.MeasProcessVm.SelectedStandartisation;
            if (proc is null || stand is null) return;
            var str = GetWriteCommand("duration", proc, stand);
            stand.StandDuration.IsWriting = true;
            _vM.CommService.Tcp.WriteMeasProcSettings(str, proc.Num);
        }, canExecPar => _vM.mainModel.Connecting.Value && _vM.MeasProcessVm.SelectedStandartisation.StandDuration.ValidationOk));
        #endregion

        #region Write StandLastDateCommand
        /// <summary>
        /// Write StandLastDateCommand
        /// </summary>
        RelayCommand _lastStandDateWriteCommand;
        /// <summary>
        /// Write StandLastDateCommand
        /// </summary>
        public RelayCommand LastStandDateWriteCommand => _lastStandDateWriteCommand ?? (_lastStandDateWriteCommand = new RelayCommand(execPar =>
        {
            var proc = _vM.MeasProcessVm.SelectedProcess;
            var stand = _vM.MeasProcessVm.SelectedStandartisation;
            if (proc is null || stand is null) return;
            var str = GetWriteCommand("date", proc, stand);
            stand.LastStandDate.IsWriting = true;
            _vM.CommService.Tcp.WriteMeasProcSettings(str, proc.Num);
        }, canExecPar => _vM.mainModel.Connecting.Value && _vM.MeasProcessVm.SelectedStandartisation.LastStandDate.ValidationOk));
        #endregion

        #region Write Standartisation result
        /// <summary>
        /// Write Standartisation result
        /// </summary>
        RelayCommand _resultWriteCommand;
        /// <summary>
        /// Write Standartisation result
        /// </summary>
        public RelayCommand ResultWriteCommand => _resultWriteCommand ?? (_resultWriteCommand = new RelayCommand(execPar =>
        {
            var proc = _vM.MeasProcessVm.SelectedProcess;
            var stand = _vM.MeasProcessVm.SelectedStandartisation;
            if (proc is null || stand is null) return;
            var str = GetWriteCommand("result", proc, stand);
            stand.StandResult.IsWriting = true;
            _vM.CommService.Tcp.WriteMeasProcSettings(str, proc.Num);

        }, canExecPar => _vM.mainModel.Connecting.Value && _vM.MeasProcessVm.SelectedStandartisation.StandResult.ValidationOk));
        #endregion

        #region Write standartisation phys value
        /// <summary>
        /// Write standartisation phys value
        /// </summary>
        RelayCommand _physValueWriteCommand;
        /// <summary>
        /// Write standartisation phys value
        /// </summary>
        public RelayCommand PhysValueWriteCommand => _physValueWriteCommand ?? (_physValueWriteCommand = new RelayCommand(execPar =>
        {
            var proc = _vM.MeasProcessVm.SelectedProcess;
            var stand = _vM.MeasProcessVm.SelectedStandartisation;
            if (proc is null || stand is null) return;
            var str = GetWriteCommand("value", proc, stand);
            stand.StandPhysValue.IsWriting = true;
            _vM.CommService.Tcp.WriteMeasProcSettings(str, proc.Num);
        }, canExecPar => _vM.mainModel.Connecting.Value && _vM.MeasProcessVm.SelectedStandartisation.StandPhysValue.ValidationOk));
        #endregion

        #region Write half life corrected value
        /// <summary>
        /// Write half life corrected value
        /// </summary>
        RelayCommand _halfLifeCorrWriteCommand;
        /// <summary>
        /// Write half life corrected value
        /// </summary>
        public RelayCommand HalfLifeCorrWriteCommand => _halfLifeCorrWriteCommand ?? (_halfLifeCorrWriteCommand = new RelayCommand(execPar =>
        {
            var proc = _vM.MeasProcessVm.SelectedProcess;
            var stand = _vM.MeasProcessVm.SelectedStandartisation;
            if (proc is null || stand is null) return;
            var str = GetWriteCommand("half_life", proc, stand);
            stand.HalfLifeCorr.IsWriting = true;
            _vM.CommService.Tcp.WriteMeasProcSettings(str, proc.Num);
        }, canExecPar => _vM.mainModel.Connecting.Value && _vM.MeasProcessVm.SelectedStandartisation.HalfLifeCorr.ValidationOk));
        #endregion


        #region Make standartisation
        /// <summary>
        /// Make standartisation
        /// </summary>
        RelayCommand _makeStandCommand;
        /// <summary>
        /// Make standartisation
        /// </summary>
        public RelayCommand MakeStandCommand => _makeStandCommand ?? (_makeStandCommand = new RelayCommand(execPar =>
        {
            if (!IsStandartisation)
            {
                var proc = _vM.MeasProcessVm.SelectedProcess;
                var stand = _vM.MeasProcessVm.SelectedStandartisation;
                if (proc is null || stand is null) return;
                _vM.CommService.MakeStand(proc.Num, (ushort)stand.Id);
                standTimer.Elapsed += (o, e) =>
                {
                    IsStandartisation = false;
                    stand.IsStandartisation = false;
                    _vM.CommService.Tcp.GetMeasSettingsExternal(proc.Num);
                    standTimer.Stop();
                };
                standTimer.Interval = stand.StandDuration.Value * 1000 + 4000;
                standTimer.Start();
                IsStandartisation = true;
                stand.IsStandartisation = true;
            }

        }, canExecPar => _vM.mainModel.Connecting.Value && !IsStandartisation));
        #endregion


        private string GetWriteCommand(string id, MeasProcSettings proc, StandSettings stand)
        {
            var str = $"*SETT,meas_proc={proc.Num}," +
            $"std={stand.Id},0," +
            $"{(id == "duration" ? stand.StandDuration.WriteValue * 10 : stand.StandDuration.Value * 10)}," +
            $"{(id == "date" ? stand.LastStandDate.WriteValue : stand.LastStandDate.Value).ToString("dd:MM:yy")}," +
            $"{(id == "result" ? stand.StandResult.WriteValue : stand.StandResult.Value).ToStringPoint()}," +
            $"{(id == "value" ? stand.StandPhysValue.WriteValue : stand.StandPhysValue.Value).ToStringPoint()}," +
            $"{(id == "half_life" ? stand.HalfLifeCorr.WriteValue : stand.HalfLifeCorr.Value).ToStringPoint()}#";
            return str;
        }

        #endregion







    }
}
