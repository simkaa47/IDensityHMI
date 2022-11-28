using IDensity.AddClasses;
using IDensity.Core.AddClasses.Settings;
using IDensity.Core.Services;
using IDensity.Models;
using IDensity.ViewModels.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace IDensity.ViewModels
{
    public class CommonSettingsVm : PropertyChangedBase
    {
        public CommonSettingsVm(VM vM)
        {
            VM = vM;
        }

        /// <summary>
        /// Главная VM
        /// </summary>
        public VM VM { get; }

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

        #region Строка для импорта-экспорта файла с настроками прибора
        /// <summary>
        /// Строка для импорта-экспорта файла с настроками прибора
        /// </summary>
        private string _fileSettingsPath;
        /// <summary>
        /// Строка для импорта-экспорта файла с настроками прибора
        /// </summary>
        public string FileSettingsPath
        {
            get => _fileSettingsPath;
            set => Set(ref _fileSettingsPath, value);
        }
        #endregion

        #region Команды

        #region Записать имя прибора
        /// <summary>
        /// Записать имя прибора
        /// </summary>
        RelayCommand _writeDeviceNameCommand;
        /// <summary>
        /// Записать имя прибора
        /// </summary>
        public RelayCommand WriteDeviceNameCommand => _writeDeviceNameCommand ?? (_writeDeviceNameCommand = new RelayCommand(execPar =>
        {
            WriteDeviceName(VM.mainModel);
        }, canExecPar => true));
        #endregion

        #region Записать имя изотопа
        /// <summary>
        /// Записать имя изотопа
        /// </summary>
        RelayCommand _writeIsotopNameCommand;
        /// <summary>
        /// Записать имя изотопа
        /// </summary>
        public RelayCommand WriteIsotopNameCommand => _writeIsotopNameCommand ?? (_writeIsotopNameCommand = new RelayCommand(execPar =>
        {
            WriteIsotope(VM.mainModel);
        }, canExecPar => true));
        #endregion

        #region записать дату установки источника
        /// <summary>
        /// записать дату установки источника
        /// </summary>
        RelayCommand _writeSourceInstallDateCommand;
        /// <summary>
        /// записать дату установки источника
        /// </summary>
        public RelayCommand WriteSourceInstallDateCommand => _writeSourceInstallDateCommand ?? (_writeSourceInstallDateCommand = new RelayCommand(execPar =>
        {
            WriteSourceInstallDate(VM.mainModel);
        }, canExecPar => true));
        #endregion

        #region Записать дату истечения срока давности источника
        /// <summary>
        /// Записать дату истечения срока давности источника
        /// </summary>
        RelayCommand _writeSourceExpirationDateCommand;
        /// <summary>
        /// Записать дату истечения срока давности источника
        /// </summary>
        public RelayCommand WriteSourceExpirationDateCommand => _writeSourceExpirationDateCommand ?? (_writeSourceExpirationDateCommand = new RelayCommand(execPar =>
        {
            WriteSourceExpirationDate(VM.mainModel);
        }, canExecPar => true));
        #endregion

        #region Записать настройки периода полураспада
        /// <summary>
        /// Записать настройки периода полураспада
        /// </summary>
        RelayCommand _writeHalfLifeCommand;
        /// <summary>
        /// Записать настройки периода полураспада
        /// </summary>
        public RelayCommand WriteHalfLifeCommand => _writeHalfLifeCommand ?? (_writeHalfLifeCommand = new RelayCommand(execPar =>
        {
            WriteHalfLife(VM.mainModel);
        }, canExecPar => true));
        #endregion

        #region Browse для пути файла импорта-экспорта
        /// <summary>
        /// Browse для пути файла импорта-экспорта
        /// </summary>
        RelayCommand _browseFilePathCommand;
        /// <summary>
        /// Browse для пути файла импорта-экспорта
        /// </summary>
        public RelayCommand BrowseFilePathCommand => _browseFilePathCommand ?? (_browseFilePathCommand = new RelayCommand(execPar =>
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                Multiselect = true,
                Title = "Выберите файл"
            };
            fileDialog.Filter = "  Текстовые файлы (*.json)|*.json";
            Nullable<bool> dialogOK = fileDialog.ShowDialog();


            if (dialogOK == true)
            {
                FileSettingsPath = fileDialog.FileName;

            }
        }, canExecPar => true));
        #endregion

        #region Записать настройки калмана
        private RelayCommand _writeKalmanSettingsCommand;
        public RelayCommand WriteKalmanSettingsCommand => _writeKalmanSettingsCommand ?? (_writeKalmanSettingsCommand = new RelayCommand(exec =>
        {
            if (!(exec is int index)) return;
            if (index > 1) return;
            var kalman = VM.mainModel.KalmanSettings[index];
            WriteCalmanSettings(kalman, index);            
        }, canExec => true));


        #endregion

        #region Записать настройки пересчета температуры
        private RelayCommand _writeTempRecalculateSettingsCommand;
        public RelayCommand WriteTempRecalculateSettingsCommand => _writeTempRecalculateSettingsCommand ?? (_writeTempRecalculateSettingsCommand = new RelayCommand(exec =>
        {
            if (!(exec is int index)) return;
            if (index > 1) return;
            WriteTempRecalculateSettings(VM.mainModel.GetTemperature, index);
        }, canExec => true));
        #endregion

        #region Записать настройки источника темпрературы
        private RelayCommand _writeTempSourceSettingsCommand;
        public RelayCommand WriteTempSourceSettingsCommand => _writeTempSourceSettingsCommand ?? (_writeTempSourceSettingsCommand = new RelayCommand(exec =>
        {
            WriteTempSourceSettings(VM.mainModel.GetTemperature);
        }, canExec => true));
        #endregion

        #region Записать тип устройства
        private RelayCommand _writeDeviceTypeCommand;
        public RelayCommand WriteDeviceTypeCommand => _writeDeviceTypeCommand ?? (_writeDeviceTypeCommand = new RelayCommand(exec =>
        {
            WriteDeviceType(VM.mainModel.DeviceType.WriteValue);
        }, canExec => true));
        #endregion

        #region Записать длину уровнемера
        private RelayCommand _writeLevelLengthCommand;
        public RelayCommand WiteLevelLengthCommand => _writeLevelLengthCommand ?? (_writeLevelLengthCommand = new RelayCommand(exec =>
        {
            WiteLevelLength(VM.mainModel.LevelLength.WriteValue);
        }, canExec => true));
        #endregion

        #region Записать mainModel в файл
        private RelayCommand _writeSettingsToFile;
        public RelayCommand WriteSettingsToFile => _writeSettingsToFile ?? (_writeSettingsToFile = new RelayCommand(p =>
        {
            SafetyAction(Save);
        }, canExec => !string.IsNullOrEmpty(FileSettingsPath)));
        #endregion


        #region Прочитать настроки из файла и записать в память прибора
        /// <summary>
        /// Прочитать настроки из файла и записать в память прибора
        /// </summary>
        RelayCommand _readSettingsToMemoryCommand;
        /// <summary>
        /// Прочитать настроки из файла и записать в память прибора
        /// </summary>
        public RelayCommand ReadSettingsToMemoryCommand => _readSettingsToMemoryCommand ?? (_readSettingsToMemoryCommand = new RelayCommand(execPar =>
        {
            SafetyAction(Load);
        }, canExec => !string.IsNullOrEmpty(FileSettingsPath)));
        #endregion

        #endregion

        public void Save()
        {
            JsonSaveLoadService.Save(VM.mainModel, typeof(MainModel), FileSettingsPath);
        }

        public void Load()
        {
            var fromFile = JsonSaveLoadService.LoadFromJson<MainModel>(FileSettingsPath) as MainModel;
            //Запись измерительных процессов
            for (int i = 0; i < MainModel.MeasProcNum; i++)
            {
                VM.MeasProcessVm.CopyMeasProcess(fromFile.MeasProcSettings[i], (ushort)i);
            }
            //Запись счетчиков
            foreach (var diap in fromFile.CountDiapasones)
            {
                VM.CommService.WriteCounterSettings(diap);
            }
            // Запись общих настроек
            WriteHalfLife(fromFile);
            WriteDeviceName(fromFile);
            WriteIsotope(fromFile);
            WriteSourceInstallDate(fromFile);
            WriteSourceExpirationDate(fromFile);
            for (int i = 0; i < VM.mainModel.KalmanSettings.Count; i++)
                WriteCalmanSettings(VM.mainModel.KalmanSettings[i], i);
            for (int i = 0; i < VM.mainModel.GetTemperature.Coeffs.Count; i++)
                WriteTempRecalculateSettings(VM.mainModel.GetTemperature, i);
            WriteTempSourceSettings(VM.mainModel.GetTemperature);
            WriteDeviceType(fromFile.DeviceType.WriteValue);
            WiteLevelLength(fromFile.LevelLength.WriteValue);
            // Запись настроек аналогов


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

        void WriteHalfLife(MainModel model)
        {
            VM.CommService.WriteCommonSettings($"half_life={model.HalfLife.WriteValue.ToStringPoint()}");
        }

        void WriteDeviceName(MainModel model)
        {
            VM.CommService.WriteCommonSettings($"name={model.DeviceName.WriteValue.Substring(0, Math.Min(model.DeviceName.WriteValue.Length, 10))}");
        }

        void WriteIsotope(MainModel model)
        {
            VM.CommService.WriteCommonSettings($"isotope={model.IsotopName.WriteValue.Substring(0, Math.Min(model.IsotopName.WriteValue.Length, 10))}");
        }

        void WriteSourceInstallDate(MainModel model)
        {
            VM.CommService.WriteCommonSettings($"src_inst_date={model.SourceInstallDate.WriteValue.ToString("dd:MM:yy")}");
        }

        void WriteSourceExpirationDate(MainModel model)
        {
            VM.CommService.WriteCommonSettings($"src_exp_date={model.SourceExpirationDate.WriteValue.ToString("dd:MM:yy")}");
        }

        void WriteCalmanSettings(Kalman kalman, int index)
        {
            string arg = $"*SETT,kalman_sett={index},{kalman.Speed.WriteValue.ToStringPoint()},{kalman.Smooth.WriteValue.ToStringPoint()}#";
            VM.CommService.Tcp.SetFsrd8(arg);
        }

        void WriteTempRecalculateSettings(GetTemperature temperature, int index)
        {
            string arg = $"*SETT,am_temp_coeffs={index},{temperature.Coeffs[index].A.WriteValue.ToStringPoint()},{temperature.Coeffs[index].B.WriteValue.ToStringPoint()}#";
            VM.CommService.Tcp.SetFsrd8(arg);
        }

        void WriteTempSourceSettings(GetTemperature temperature)
        {
            string arg = $"*SETT,temperature_src={temperature.Source}#";
            VM.CommService.Tcp.SetFsrd8(arg);
        }

        void WriteDeviceType(ushort value)
        {
            string arg = $"*SETT,device_type={value}#";
            VM.CommService.Tcp.SetFsrd8(arg);
        }

        void WiteLevelLength(float value)
        {
            string arg = $"*SETT,levelmeter_ln={value.ToStringPoint()}#";
            VM.CommService.Tcp.SetFsrd8(arg);
        }
    }



   
}
