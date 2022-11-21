using IDensity.AddClasses;
using IDensity.Core.Services;
using IDensity.Models;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace IDensity.ViewModels
{
    public class CommonSettingsVm:PropertyChangedBase
    {
        public CommonSettingsVm(VM vM)
        {
            VM = vM;
            Init();
        }

        /// <summary>
        /// Главная VM
        /// </summary>
        public VM VM { get; }

        void Init()
        {
            VM.mainModel.HalfLife.CommandEcecutedEvent += (o) => 
                VM.CommService.WriteCommonSettings($"half_life={VM.mainModel.HalfLife.WriteValue.ToStringPoint()}");
            VM.mainModel.DeviceName.CommandEcecutedEvent += (o) =>
                VM.CommService.WriteCommonSettings($"name={VM.mainModel.DeviceName.WriteValue.Substring(0,Math.Min(VM.mainModel.DeviceName.WriteValue.Length,10))}");
            VM.mainModel.IsotopName.CommandEcecutedEvent += (o) => 
                VM.CommService.WriteCommonSettings($"isotope={VM.mainModel.IsotopName.WriteValue.Substring(0, Math.Min(VM.mainModel.IsotopName.WriteValue.Length, 10))}");
            VM.mainModel.SourceInstallDate.CommandEcecutedEvent += (o) => 
                VM.CommService.WriteCommonSettings($"src_inst_date={VM.mainModel.SourceInstallDate.WriteValue.ToString("dd:MM:yy")}");
            VM.mainModel.SourceExpirationDate.CommandEcecutedEvent += (o) => VM.CommService.WriteCommonSettings($"src_exp_date={VM.mainModel.SourceExpirationDate.WriteValue.ToString("dd:MM:yy")}");
        }

        #region Запрос контрольной суммы ПО основного микроконтроллера
        /// <summary>
        /// Запрос контрольной суммы ПО основного микроконтроллера
        /// </summary>
        RelayCommand _getCheckSumCommand;
        /// <summary>
        /// Запрос контрольной суммы ПО основного микроконтроллера
        /// </summary>
        public RelayCommand GetCheckSumCommand => _getCheckSumCommand ?? (_getCheckSumCommand = new RelayCommand(execPar => 
        {
            VM.CommService.GetCheckSum();

        }, canExecPar => true));
        #endregion

        #region Команды
        #region Записать настройки калмана
        private RelayCommand _writeKalmanSettingsCommand;
        public RelayCommand WriteKalmanSettingsCommand => _writeKalmanSettingsCommand ?? (_writeKalmanSettingsCommand = new RelayCommand(exec => 
        {
            if (!(exec is int index)) return;
            if (index > 1) return;
            var kalmans = VM.mainModel.KalmanSettings;
            string arg = $"*SETT,kalman_sett={index},{kalmans[index].Speed.WriteValue.ToStringPoint()},{kalmans[index].Smooth.WriteValue.ToStringPoint()}#";
            VM.CommService.Tcp.SetFsrd8(arg);
        }, canExec => true));


        #endregion

        #region Записать настройки пересчета температуры
        private RelayCommand _writeTempRecalculateSettingsCommand;
        public RelayCommand WriteTempRecalculateSettingsCommand => _writeTempRecalculateSettingsCommand ?? (_writeTempRecalculateSettingsCommand = new RelayCommand(exec =>
        {
            if (!(exec is int index)) return;
            if (index > 1) return;
            var temp = VM.mainModel.GetTemperature;
            string arg = $"*SETT,am_temp_coeffs={index},{temp.Coeffs[index].A.WriteValue.ToStringPoint()},{temp.Coeffs[index].B.WriteValue.ToStringPoint()}#";
            VM.CommService.Tcp.SetFsrd8(arg);
        }, canExec => true));
        #endregion

        #region Записать настройки источника темпрературы
        private RelayCommand _writeTempSourceSettingsCommand;
        public RelayCommand WriteTempSourceSettingsCommand => _writeTempSourceSettingsCommand ?? (_writeTempSourceSettingsCommand = new RelayCommand(exec =>
        {            
            var temp = VM.mainModel.GetTemperature;
            string arg = $"*SETT,temperature_src={temp.Source}#";
            VM.CommService.Tcp.SetFsrd8(arg);
        }, canExec => true));
        #endregion

        #region Записать тип устройства
        private RelayCommand _writeDeviceTypeCommand;
        public RelayCommand WriteDeviceTypeCommand => _writeDeviceTypeCommand ?? (_writeDeviceTypeCommand = new RelayCommand(exec =>
        {           
            string arg = $"*SETT,device_type={VM.mainModel.DeviceType.WriteValue}#";
            VM.CommService.Tcp.SetFsrd8(arg);
        }, canExec => true));
        #endregion

        #region Записать длину уровнемера
        private RelayCommand _writeLevelLengthCommand;
        public RelayCommand WiteLevelLengthCommand => _writeLevelLengthCommand ?? (_writeLevelLengthCommand = new RelayCommand(exec =>
        {
            string arg = $"*SETT,levelmeter_ln={VM.mainModel.LevelLength.WriteValue.ToStringPoint()}#";
            VM.CommService.Tcp.SetFsrd8(arg);
        }, canExec => true));
        #endregion

        #region Записать mainModel в файл
        private RelayCommand _writeSettingsToFile;
        public RelayCommand WriteSettingsToFile => _writeSettingsToFile ?? (_writeSettingsToFile = new RelayCommand(p => 
        {
            SafetyAction(Save);
        }, canExec => true));
        #endregion
        #endregion

        public void Save()
        {
            JsonSaveLoadService.Save(VM.mainModel, typeof(MainModel));
        }

        void SafetyAction(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }

}
