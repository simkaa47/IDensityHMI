using IDensity.AddClasses;
using IDensity.Core.AddClasses.Settings;
using IDensity.Core.Services;
using IDensity.Models;
using IDensity.ViewModels.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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


        #region Флаг загрузки
        /// <summary>
        /// Флаг загрузки
        /// </summary>
        private bool _loadFlag;
        /// <summary>
        /// Флаг загрузки
        /// </summary>
        public bool LoadFlag
        {
            get => _loadFlag;
            set => Set(ref _loadFlag, value);
        }
        #endregion

        #region Индикатр загрузки
        /// <summary>
        /// Индикатр загрузки
        /// </summary>
        private int _loadProgress;
        /// <summary>
        /// Индикатр загрузки
        /// </summary>
        public int LoadProgress
        {
            get => _loadProgress;
            set => Set(ref _loadProgress, value);
        }
        #endregion




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

        #region Записать настройки калмана (Speed)
        private RelayCommand _writeKalmanSpeedCommand;
        public RelayCommand WriteKalmanSpeedCommand => _writeKalmanSpeedCommand ?? (_writeKalmanSpeedCommand = new RelayCommand(exec =>
        {
            if (!(exec is int index)) return;
            if (index > 1) return;
            var kalman = VM.mainModel.KalmanSettings[index];           
            WriteCalmanSettingsById(kalman, "speed", index, kalman.Speed);            
        }, canExec => VM.mainModel.Connecting.Value));
        #endregion

        #region Записать настройки калмана (smooth)
        private RelayCommand _writeKalmanSmoothCommand;
        public RelayCommand WriteKalmanSmoothCommand => _writeKalmanSmoothCommand ?? (_writeKalmanSmoothCommand = new RelayCommand(exec =>
        {
            if (!(exec is int index)) return;
            if (index > 1) return;
            var kalman = VM.mainModel.KalmanSettings[index];
            WriteCalmanSettingsById(kalman, "smooth", index, kalman.Smooth);
        }, canExec => VM.mainModel.Connecting.Value));
        #endregion

        #region Записать настройки пересчета температуры
        private RelayCommand _writeTempRecalculateSettingsCommand;
        public RelayCommand WriteTempRecalculateSettingsCommand => _writeTempRecalculateSettingsCommand ?? (_writeTempRecalculateSettingsCommand = new RelayCommand(exec =>
        {
            if (!(exec is int index)) return;
            if (index > 1) return;
            VM.mainModel.GetTemperature.Coeffs[index].A.IsWriting = true;
            VM.mainModel.GetTemperature.Coeffs[index].B.IsWriting = true;
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
            VM.mainModel.DeviceType.IsWriting = true;
            WriteDeviceType(VM.mainModel.DeviceType.WriteValue);
        }, canExec => true));
        #endregion

        #region Записать длину уровнемера
        private RelayCommand _writeLevelLengthCommand;
        public RelayCommand WiteLevelLengthCommand => _writeLevelLengthCommand ?? (_writeLevelLengthCommand = new RelayCommand(exec =>
        {
            VM.mainModel.LevelLength.IsWriting = true;
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
            foreach (var group in fromFile.AnalogGroups)
            {
                VM.CommService.Tcp.SendAnalogInSwttings(group.AI.GroupNum, group.AI.ModulNum, group.AI);
                VM.CommService.Tcp.SendAnalogOutSwttings(group.AI.GroupNum, group.AI.ModulNum, group.AO);
            }
            //Настроки связи            
            VM.CommService.ChangeSerialSelect(fromFile.PortSelectMode.WriteValue);//; Режим порта
            VM.CommService.ChangeBaudrate(fromFile.PortBaudrate.WriteValue);// байдрейт
            byte num = 0;// udp
            var nums = (fromFile.UdpAddrString.Split(".", StringSplitOptions.RemoveEmptyEntries)).Where(s => byte.TryParse(s, out num)).Select(s => num).ToArray();
            if (nums.Length == 4)
            {
                fromFile.UdpWriting = true;
                VM.CommService.SetUdpAddr(nums, fromFile.PortUdp);
            }
            VM.CommService.SetTcpSettings(fromFile.IP, fromFile.Mask, fromFile.GateWay);// tcp

            // настройки АЦП
            VM.CommService.SetAdcMode(fromFile.AdcBoardSettings.AdcMode.WriteValue);
            VM.CommService.SetAdcSyncMode(fromFile.AdcBoardSettings.AdcSyncMode.WriteValue);
            VM.CommService.SetAdcSyncLevel(fromFile.AdcBoardSettings.AdcSyncLevel.WriteValue);
            VM.CommService.SetAdcProcMode(fromFile.AdcBoardSettings.AdcProcMode.WriteValue);
            VM.CommService.SetAdcTimerMax(fromFile.AdcBoardSettings.TimerMax.WriteValue);
            VM.CommService.SetPreampGain(fromFile.AdcBoardSettings.PreampGain.WriteValue);
            VM.CommService.SetHv(fromFile.TelemetryHV.VoltageSV.WriteValue);
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

        void WriteSourceInstallDate(MainModel model)
        {
            model.SourceInstallDate.IsWriting = true;
            VM.CommService.WriteCommonSettings($"src_inst_date={model.SourceInstallDate.WriteValue.ToString("dd:MM:yy")}");
        }

        void WriteSourceExpirationDate(MainModel model)
        {
            model.SourceExpirationDate.IsWriting = true;
            VM.CommService.WriteCommonSettings($"src_exp_date={model.SourceExpirationDate.WriteValue.ToString("dd:MM:yy")}");
        }

        void WriteCalmanSettingsById<T>(Kalman kalman, string id, int index, Parameter<T> par) where T: IComparable
        {
            string arg = $"*SETT,kalman_sett={index}," +
                $"{(id == "speed" ? kalman.Speed.WriteValue : kalman.Speed.Value).ToStringPoint()}," +
                $"{(id == "smooth" ? kalman.Smooth.WriteValue : kalman.Smooth.Value).ToStringPoint()}#";
            par.IsWriting = true;
            VM.CommService.Tcp.SetFsrd8(arg);
        }

        void WriteCalmanSettings(Kalman kalman,  int index)
        {
            string arg = $"*SETT,kalman_sett={index}," +
                $"{kalman.Speed.WriteValue.ToStringPoint()}," +
                $"{kalman.Smooth.WriteValue.ToStringPoint()}#";
            kalman.Smooth.IsWriting = true;
            kalman.Speed.IsWriting = true;
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

        void WriteIsotope(MainModel model)
        {
            model.IsotopeIndex.IsWriting = true;            
            VM.CommService.WriteCommonSettings($"isotope={model.IsotopeIndex.WriteValue}");
        }
    }



   
}
