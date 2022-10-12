using IDensity.AddClasses;
using IDensity.Models;
using IDensity.Services.AdcServices;
using IDensity.Services.ComminicationServices;
using IDensity.Services.InitServices;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace IDensity.ViewModels
{
    public  class AdcViewModel:PropertyChangedBase
    {
        public AdcViewModel(VM vM)
        {
            VM = vM;
            AdcAvgSettings = XmlInit.ClassInit<AdcAvgSettings>();
            _spectrLogSettings = XmlInit.ClassInit<SpectrLogSettings>();
            UdpService = new UdpService(AdcAvgSettings, vM);            
            UdpService.Start();
            Describe();
        }

        public VM VM { get; }

        #region Сервисы
        #region Сервис получения даных по UDP
        public UdpService UdpService { get; private set; }
        #endregion
        public CheckPulseService CheckPulseService { get; private set; }
        #endregion

        #region Данные для поиска вершины спектра
        public AdcAvgSettings AdcAvgSettings { get; private set; }
        #endregion

        #region Команды

        #region Записать режим измерения АЦП
        RelayCommand _setAdcModeChange;
        public RelayCommand SetAdcModeChangeCommand => _setAdcModeChange ?? (_setAdcModeChange = new RelayCommand(p => {
            
            VM.CommService.SetAdcMode(VM.mainModel.AdcBoardSettings.AdcMode.WriteValue);
        }, p => true));
        #endregion

        #region Записать Режим синхронизации
        RelayCommand _setAdcSyncModeChangeCommand;
        public RelayCommand SetAdcSyncModeChangeCommand => _setAdcSyncModeChangeCommand ?? (_setAdcSyncModeChangeCommand = new RelayCommand(p => {
            
            VM.CommService.SetAdcSyncMode(VM.mainModel.AdcBoardSettings.AdcSyncMode.WriteValue);
        }, p => true));
        #endregion

        #region Записать Уровень синхронизации
        RelayCommand _setAdcSyncLevelChangeCommand;
        public RelayCommand SetAdcSyncLevelChangeCommand => _setAdcSyncLevelChangeCommand ?? (_setAdcSyncLevelChangeCommand = new RelayCommand(p => {
            
            VM.CommService.SetAdcSyncLevel(VM.mainModel.AdcBoardSettings.AdcSyncLevel.WriteValue);
        }, p => true));
        #endregion

        #region Записать Режим обработки при регистрировании максимальных амплитуд
        RelayCommand _setAdcProcModeChangeCommand;
        public RelayCommand SetAdcProcModeChangeCommand => _setAdcProcModeChangeCommand ?? (_setAdcProcModeChangeCommand = new RelayCommand(p => {
            
            VM.CommService.SetAdcProcMode(VM.mainModel.AdcBoardSettings.AdcProcMode.WriteValue);
        }, p => true));
        #endregion

        #region Записать Таймер выдачи данных
        RelayCommand _setTimerMaxChangeCommand;
        public RelayCommand SetTimerMaxChangeCommand => _setTimerMaxChangeCommand ?? (_setTimerMaxChangeCommand = new RelayCommand(p => {
            
            VM.CommService.SetAdcTimerMax(VM.mainModel.AdcBoardSettings.TimerMax.WriteValue);
        }, p => true));
        #endregion

        #region Записать К-т предусиления
        RelayCommand _setPreampGainChangeCommand;
        public RelayCommand SetPreampGainChangeCommand => _setPreampGainChangeCommand ?? (_setPreampGainChangeCommand = new RelayCommand(p => {
            
            VM.CommService.SetPreampGain(VM.mainModel.AdcBoardSettings.PreampGain.WriteValue);
        }, p => true));
        #endregion

        #region Включить-выключить Hv
        /// <summary>
        /// Включить-выключить Hv
        /// </summary>
        RelayCommand _switchHvCommandCommand;
        /// <summary>
        /// Включить-выключить Hv
        /// </summary>
        public RelayCommand switchHvCommandCommand => 
            _switchHvCommandCommand ?? (_switchHvCommandCommand = new RelayCommand(execPar => 
            {
                VM.CommService.SwitchHv();
            }, canExecPar => true));
        #endregion

        #region Установить напряжение HV
        /// <summary>
        /// Установить напряжение HV
        /// </summary>
        RelayCommand _setHvCommand;
        /// <summary>
        /// Установить напряжение HV
        /// </summary>
        public RelayCommand SetHvCommand => _setHvCommand ?? (_setHvCommand = new RelayCommand(execPar => 
        {
            VM.CommService.SetHv(VM.mainModel.TelemetryHV.VoltageSV.WriteValue);
        }, canExecPar => true));
        #endregion

        #region Запуск-останов платы АЦП
        RelayCommand _startAdcCommand;
        public RelayCommand StartAdcCommand => _startAdcCommand ?? (_startAdcCommand = new RelayCommand(execPar =>
        {
            ushort num = 0;
            if (execPar != null && ushort.TryParse(execPar.ToString(), out num))
            {
                VM.CommService.SwitchAdcBoard(num);
                UdpService.Start();
            }
        }, canEcecPar => true));
        #endregion

        #region Запуск/останов выдачи данных АЦП 
        RelayCommand _startAdcDataCommand;
        public RelayCommand StartAdcDataCommand => _startAdcDataCommand ?? (_startAdcDataCommand = new RelayCommand(execPar =>
        {
            ushort num = 0;
            if (execPar != null && ushort.TryParse(execPar.ToString(), out num)) VM.CommService.StartStopAdcData(num);
            UdpService.Start();
        }, canEcecPar => true));
        #endregion

        #region Показать осциллограмму
        RelayCommand _showOscillCommand;
        public RelayCommand ShowOscillCommand => _showOscillCommand ?? (_showOscillCommand = new RelayCommand(par =>
        {
            VM.CommService.StartStopAdcData(0);
            VM.CommService.SwitchAdcBoard(0);
            VM.CommService.SetAdcMode(0);
            VM.CommService.StartStopAdcData(1);
            VM.CommService.SwitchAdcBoard(1);
            UdpService.Start();

        }, o => VM.mainModel.Connecting.Value));
        #endregion

        #region Показать спектр
        RelayCommand _showSpectrCommand;
        public RelayCommand ShowSpectrCommand => _showSpectrCommand ?? (_showSpectrCommand = new RelayCommand(par =>
        {
            VM.CommService.StartStopAdcData(0);
            UdpService.Start();
            VM.CommService.SwitchAdcBoard(0);
            VM.CommService.SetAdcMode(1);
            VM.CommService.SetAdcProcMode(1);
            VM.CommService.StartStopAdcData(1);
            VM.CommService.SwitchAdcBoard(1);

        }, o => VM.mainModel.Connecting.Value));
        #endregion

        #region Очистить спектр
        RelayCommand _clearSpectrCommand;
        public RelayCommand ClearSpectrCommand => _clearSpectrCommand ?? (_clearSpectrCommand = new RelayCommand(par =>
        {
            VM.CommService.ClearSpectr();
            VM.CommService.SwitchAdcBoard(1);

        }, o => VM.mainModel.Connecting.Value));
        #endregion

        #region Запуск-останов логирования спектра
        RelayCommand _startStopSpectrLogCommand;
        public RelayCommand StartStopSpectrLogCommand => _startStopSpectrLogCommand ?? (_startStopSpectrLogCommand = new RelayCommand(par =>
        {
            if (!IsSpectrLogging)
            {
                if (!File.Exists(SpetrLogPath)) MessageBox.Show($"Путь {SpetrLogPath} некорректный!");
                else IsSpectrLogging = true;
            }
            else IsSpectrLogging = false;
        }, o => true));
        #endregion

        #region Найти максимум спектра и перезаписать границы для выбранного диапазона счетчиков

        #region ПОиск максимума спектра и перезаписать границы для выбранного диапазона счетчиков
        /// <summary>
        /// ПОиск максимума спектра и перезаписать границы для выбранного диапазона счетчиков
        /// </summary>
        RelayCommand _tuneCounterDiapasoneCommand;
        /// <summary>
        /// ПОиск максимума спектра и перезаписать границы для выбранного диапазона счетчиков
        /// </summary>
        public RelayCommand TuneCounterDiapasoneCommand => _tuneCounterDiapasoneCommand ?? (_tuneCounterDiapasoneCommand = new RelayCommand(execPar =>
        {
            if (UdpService.Mode == 2 && AdcDataTrend != null && AdcDataTrend.Count > AdcAvgSettings.SpectrMaxLimit)
            {
                var max = AdcDataTrend[AdcAvgSettings.SpectrMinLimit];
                for (int i = AdcAvgSettings.SpectrMinLimit + 1; i < AdcAvgSettings.SpectrMaxLimit; i++)
                {
                    if (AdcDataTrend[i].Y > max.Y) max = AdcDataTrend[i];
                }
                var index = max.X;
                index = Math.Max(0, index - AdcAvgSettings.SpectrFilterDeep / 2);
                var minLimit = Math.Clamp(index - index * AdcAvgSettings.LeftCounterCoeff, 0, 4095);
                var maxLimit = Math.Clamp(index + index * AdcAvgSettings.RightCounterCoeff, 0, 4095);
                if(SelectedCountDiapasone != null)
                {
                    var diap = SelectedCountDiapasone.Clone() as CountDiapasone;
                    diap.Start.Value = (ushort)minLimit;
                    diap.Width.Value = (ushort)(maxLimit - minLimit);
                    VM.CommService.WriteCounterSettings(diap);
                }
            }
        }, canExecPar => true));
        #endregion

        #endregion

        #region Записать данные дипазона счетчика
        /// <summary>
        /// Записать данные дипазона счетчика
        /// </summary>
        RelayCommand _writeCountDiapasoneCommand;
        /// <summary>
        /// Записать данные дипазона счетчика
        /// </summary>
        public RelayCommand WriteCountDiapasoneCommand => 
            _writeCountDiapasoneCommand ?? 
            (_writeCountDiapasoneCommand = new RelayCommand(execPar => 
            {
                if (SelectedCountDiapasone == null) return;
                if (SelectedCountDiapasone.Clone() is CountDiapasone diapasone)
                {
                    diapasone.CounterMode.Value = SelectedCountDiapasone.CounterMode.WriteValue;
                    diapasone.Start.Value = SelectedCountDiapasone.Start.WriteValue;
                    diapasone.Width.Value = SelectedCountDiapasone.Width.WriteValue;
                    diapasone.CountPeakFind.Value = SelectedCountDiapasone.CountPeakFind.WriteValue;
                    diapasone.CountPeakSmooth.Value = SelectedCountDiapasone.CountPeakSmooth.WriteValue;
                    diapasone.CountTopPerc.Value = SelectedCountDiapasone.CountTopPerc.WriteValue;
                    diapasone.CountBotPerc.Value = SelectedCountDiapasone.CountBotPerc.WriteValue;
                    VM.CommService.WriteCounterSettings(diapasone);
                }
               
            }, canExecPar => true));
        #endregion

        #region Очистить статистику импульсов в режиме осцилографа
        /// <summary>
        /// Очистить статистику импульсов в режиме осцилографа
        /// </summary>
        RelayCommand _clearPulseStaticticCommand;
        /// <summary>
        /// Очистить статистику импульсов в режиме осцилографа
        /// </summary>
        public RelayCommand ClearPulseStaticticCommand => _clearPulseStaticticCommand ?? (_clearPulseStaticticCommand = new RelayCommand(execPar => CheckPulseService.Clear(), canExecPar => true));
        #endregion

        #endregion

        #region Данные UDP
        DateTime lastUpdateTime = DateTime.Now;

        #region Данные тренда
        List<Point> _adcDataTrend;
        public List<Point> AdcDataTrend
        {
            get => _adcDataTrend;
            set
            {
                Set(ref _adcDataTrend, value);
            }
        }
        void UpdateAdcTrend(List<Point> list)
        {
            if (lastUpdateTime.AddMilliseconds(500) < DateTime.Now)
            {
                AdcDataTrend = list;
                lastUpdateTime = DateTime.Now;
            }

        }
        #endregion

        #region Данные гистограммы в режиме максимальных амплитуд
        List<Point> _maxAmplitudesData;
        /// <summary>
        /// Данные гистограммы в режиме максимальных амплитуд
        /// </summary>
        public List<Point> MaxAmplitudesData
        {
            get => _maxAmplitudesData;
            set => Set(ref _maxAmplitudesData, value);
        }
        void UpdateMaxAmpsData(List<Point> list)
        {
            if (lastUpdateTime.AddMilliseconds(500) < DateTime.Now)
            {
                MaxAmplitudesData = list;
                lastUpdateTime = DateTime.Now;
            }
        }
        #endregion
        #endregion

        #region Логирование спектра
        private string _spetrLogPath;

        public string SpetrLogPath
        {
            get { return _spetrLogPath; }
            set { Set(ref _spetrLogPath, value); }
        }


        #region Флаг выполнения
        private bool _isSpectrLogging;
        /// <summary>
        /// Выполняется логирование спектра
        /// </summary>
        public bool IsSpectrLogging
        {
            get { return _isSpectrLogging; }
            set { Set(ref _isSpectrLogging, value); }
        }

        #endregion
        /// <summary>
        /// Сервис логгирования спектра
        /// </summary>
        private SpectrLogService _logService;
        /// <summary>
        /// Сервис логирования спектра
        /// </summary>
        public SpectrLogService SpectrLogService
        {
            get
            {
                if (_logService == null)
                {
                    _logService = new SpectrLogService();
                    _logService.SpectrErrorEvent += (msg) =>
                    {
                        MessageBox.Show(msg);
                        IsSpectrLogging = false;
                    };
                }
                return _logService;
            }

        }
        #endregion

        #region Настройки спектра из xml
        private SpectrLogSettings _spectrLogSettings;

        public SpectrLogSettings SpectrLogSettings
        {
            get { return _spectrLogSettings; }

        }
        #endregion

        #region Дата следующего обнуления  спектра
        /// <summary>
        /// Дата следующего обнуления  спектра
        /// </summary>
        private DateTime _nextSpectrClearTime;
        /// <summary>
        /// Дата следующего обнуления  спектра
        /// </summary>
        public DateTime NextSpectrClearTime
        {
            get => _nextSpectrClearTime;
            set => Set(ref _nextSpectrClearTime, value);
        }
        #endregion

        private DateTime _lastSpectrLogTime = DateTime.MinValue;
        private DateTime _lastSpectrClearTime = DateTime.MinValue;

        void Describe()
        {
            CheckPulseService = new CheckPulseService();
            UdpService.UpdateOscillEvent += (collection) =>
            {
                UpdateAdcTrend(collection);
                if (UdpService.Mode == 2 && IsSpectrLogging)
                {
                    if (!SpectrLogSettings.CyclicLog)
                    {
                        IsSpectrLogging = false;
                        WriteToFile(collection);
                    }
                    else
                    {
                        if (DateTime.Now >= _lastSpectrLogTime.AddSeconds(SpectrLogSettings.LogFreq))
                        {
                            WriteToFile(collection);
                            _lastSpectrLogTime = DateTime.Now;
                        }
                        if (DateTime.Now >= _lastSpectrClearTime.AddSeconds(SpectrLogSettings.FreqClearSpectr))
                        {
                            _lastSpectrClearTime = DateTime.Now;
                            VM.CommService.ClearSpectr();
                            VM.CommService.SwitchAdcBoard(1);
                        }
                        NextSpectrClearTime = _lastSpectrClearTime.AddSeconds(SpectrLogSettings.FreqClearSpectr);
                    }
                }
                else if (UdpService.Mode == 0)
                {
                    CheckPulseService.Calculate(collection);
                }
            };
            UdpService.UpdateAmplitudesEvent += UpdateMaxAmpsData;
        }

        void WriteToFile(List<Point> collection)
        {
            var parameters = $"hv={VM.mainModel.TelemetryHV.VoltageCurOut.Value},syn_level={VM.mainModel.AdcBoardSettings.AdcSyncLevel.Value},preamp_gain={VM.mainModel.AdcBoardSettings.PreampGain.Value}";
            SpectrLogService.WriteToFile(SpetrLogPath, parameters, collection);
        }

        #region выбранный диапазон счетчика
        private CountDiapasone _selectedCountDiapasone;

        public CountDiapasone SelectedCountDiapasone
        {
            get { return _selectedCountDiapasone; }
            set { Set(ref _selectedCountDiapasone, value); }
        }
        #endregion
    }
}
