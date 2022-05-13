using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using IDensity.AddClasses;
using IDensity.Models;
using IDensity.Models.SQL;
using IDensity.Models.XML;
using IDensity.ViewModels.Commands;
using System.Timers;
using IDensity.AddClasses.EventHistory;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Controls;


namespace IDensity.ViewModels
{
    /// <summary>
    /// Общая ViewModel для окна
    /// </summary>
    class VM : PropertyChangedBase
    {
        #region Пользователи

        #region Текущий пользователь
        User _curUser;
        public User CurUser { get => _curUser; set => Set(ref _curUser, value); }
        #endregion


        #region Коллекция пользователей
        public DataBaseCollection<User> Users { get; } = new DataBaseCollection<User>("Users", new User { Login = "admin", Password = "0000" });
        #endregion
        #endregion

        #region Команды

        #region Обновить список доступных Com портов
        RelayCommand _updateComPortListCommand;
        public RelayCommand UpdateComPortListCommand => _updateComPortListCommand ?? (_updateComPortListCommand = new RelayCommand(o => ComPorts = SerialPort.GetPortNames(), o => true));
        #endregion

        #region Команда "Закрыть приложение"
        RelayCommand _closeAppCommand;
        public RelayCommand CloseAppCommand => _closeAppCommand ?? (_closeAppCommand = new RelayCommand(o =>
        {
            Udp?.Stop();
            Application.Current.Shutdown();
        }, o => true));
        #endregion

        #region Команда вкл-выкл напряжение
        RelayCommand _switchHvCommand;
        public RelayCommand SwitchHvCommand { get => _switchHvCommand ?? (_switchHvCommand = new RelayCommand(o => mainModel.SwitchHv(), o => true)); }
        #endregion

        #region Команда вкл-выкл измерения
        RelayCommand _switchMeasCommand;
        public RelayCommand SwitchMeasCommand { get => _switchMeasCommand ?? (_switchMeasCommand = new RelayCommand(o => mainModel.SwitchMeas(), o => true)); }
        #endregion

        #region Команда плказать архивный тренд
        RelayCommand _showArchivalTrendCommand;
        public RelayCommand ShowArchivalTrendCommand { get => _showArchivalTrendCommand ?? (_showArchivalTrendCommand = new RelayCommand(o => ShowArchivalTrend(), o => true)); }
        #endregion

        #region Команда "Логаут"
        RelayCommand _logoutCommand;
        public RelayCommand LogoutCommand { get => _logoutCommand ?? (_logoutCommand = new RelayCommand(o => CurUser = null, o => true)); }
        #endregion

        #region Команда "Записать архивный тренд в файл"
        RelayCommand _writeLogCommand;
        public RelayCommand WriteLogCommand { get => _writeLogCommand ?? (_writeLogCommand = new RelayCommand(o => WriteArchivalTrendToText(), o => true)); }
        #endregion                   

        #region Команды измерения даты-времени
        #region установить RTC пользователя
        RelayCommand _setRtcCommand;
        public RelayCommand SetRtcCommand => _setRtcCommand ?? (_setRtcCommand = new RelayCommand(execPar =>
        {
            mainModel.SetRtc(mainModel.Rtc.WriteValue);
        }, canExecPar => true));
        #endregion
        #region Синхронизировать c временем ПК
        RelayCommand _syncRtcCommand;
        public RelayCommand SyncRtcCommand => _syncRtcCommand ?? (_syncRtcCommand = new RelayCommand(execPar =>
        {
            mainModel.SetRtc(DateTime.Now);
        }, canExecPar => true));
        #endregion
        #endregion

        #region Команды настроек измерительного порта
        #region Сменить режим порта 
        RelayCommand _changeSerialSelectCommand;
        public RelayCommand ChangeSerialSelectCommand => _changeSerialSelectCommand ?? (_changeSerialSelectCommand = new RelayCommand(o => mainModel.ChangeSerialSelect((int)o), o => o != null));
        #endregion

        #region Сменить баудрейт 
        RelayCommand _changeBaudrateCommand;
        public RelayCommand ChangeBaudrateCommand => _changeBaudrateCommand ?? (_changeBaudrateCommand = new RelayCommand(o => mainModel.ChangeBaudrate((uint)(o)), o => o != null));
        #endregion
        #endregion        

        #region Команда "Установить напряжение HV"
        RelayCommand _setHvCommand;
        public RelayCommand SetHvCommand => _setHvCommand ?? (_setHvCommand = new RelayCommand(obj => mainModel.SetHv(mainModel.TelemetryHV.VoltageSV.WriteValue), obj => true));
        #endregion

        #region Команды вывода и фильтрации событий
        private RelayCommand _showEventsCommand;

        public RelayCommand ShowEventsCommand => _showEventsCommand ?? (_showEventsCommand = new RelayCommand(exec => AddHistoryItemsFromDb(), canExex => true));

        #endregion

        #region Команда "Установить настроку IP приемника UDP даных
        RelayCommand _setUpsAddrCommand;
        public RelayCommand SetUpsAddrCommand => _setUpsAddrCommand ?? (_setUpsAddrCommand = new RelayCommand(execPar =>
        {
            byte num = 0;
            var nums = (mainModel.UdpAddrString.Split(".", StringSplitOptions.RemoveEmptyEntries)).Where(s => byte.TryParse(s, out num)).Select(s => num).ToArray();
            if (nums.Length == 4) mainModel.SetUdpAddr(nums, mainModel.PortUdp);
        },
            canExecPar => mainModel.Connecting.Value));

        #endregion        

        #region Запуск-останов платы АЦП
        RelayCommand _startAdcCommand;
        public RelayCommand StartAdcCommand => _startAdcCommand ?? (_startAdcCommand = new RelayCommand(execPar =>
        {
            ushort num = 0;
            if (execPar != null && ushort.TryParse(execPar.ToString(), out num))
            {
                mainModel.SwitchAdcBoard(num);
                Udp.Start();
            }
        }, canEcecPar => true));
        #endregion

        #region Запуск/останов выдачи данных АЦП 
        RelayCommand _startAdcDataCommand;
        public RelayCommand StartAdcDataCommand => _startAdcDataCommand ?? (_startAdcDataCommand = new RelayCommand(execPar =>
        {
            ushort num = 0;
            if (execPar != null && ushort.TryParse(execPar.ToString(), out num)) mainModel.StartStopAdcData(num);
            Udp.Start();
        }, canEcecPar => true));
        #endregion

        #region Разорвать tcp соединение 
        RelayCommand _tcpDisconnectCommand;
        public RelayCommand TcpDisconnectCommand => _tcpDisconnectCommand ?? (_tcpDisconnectCommand = new RelayCommand(par => mainModel.Tcp.Disconnect(), o => true));
        #endregion

        #region Переключить реле
        RelayCommand _switchRelayCommand;
        public RelayCommand SwitchRelayCommand => _switchRelayCommand ?? (_switchRelayCommand = new RelayCommand(par => {
            ushort temp = 0;
            if (par != null && ushort.TryParse(par.ToString(), out temp))
            {
                mainModel.SwitchRelay(temp);
            }
        }, canExec => true));
        #endregion

        #region Перезагрузить плату
        RelayCommand _rstBoardCommand;
        public RelayCommand RstBoardCommand => _rstBoardCommand ?? (_rstBoardCommand = new RelayCommand(par => mainModel.RstBoard(), o => mainModel.Connecting.Value));
        #endregion

        #region Обновить доступные ЕИ
        RelayCommand _updateAvialablemeasUnitCommand;
        public RelayCommand UpdateAvialablemeasUnitCommand => _updateAvialablemeasUnitCommand ?? (_updateAvialablemeasUnitCommand = new RelayCommand(par =>
        {
            OnPropertyChanged(nameof(AvialableMeasUnitSettings));
        }, o => true));
        #endregion

        #region Показать осциллограмму
        RelayCommand _showOscillCommand;
        public RelayCommand ShowOscillCommand => _showOscillCommand ?? (_showOscillCommand = new RelayCommand(par => 
        {
            mainModel.StartStopAdcData(0);
            mainModel.SwitchAdcBoard(0);
            mainModel.SetAdcMode(0);
            mainModel.StartStopAdcData(1);
            mainModel.SwitchAdcBoard(1);
            Udp.Start();

        }, o => mainModel.Connecting.Value));
        #endregion

        #region Показать спектр
        RelayCommand _showSpectrCommand;
        public RelayCommand ShowSpectrCommand => _showSpectrCommand ?? (_showSpectrCommand = new RelayCommand(par =>
        {
            mainModel.StartStopAdcData(0);
            Udp.Start();
            mainModel.SwitchAdcBoard(0);
            mainModel.SetAdcMode(1);
            mainModel.SetAdcProcMode(1);
            mainModel.StartStopAdcData(1);
            mainModel.SwitchAdcBoard(1);

        }, o => mainModel.Connecting.Value));
        #endregion

        #region Очистить спектр
        RelayCommand _clearSpectrCommand;
        public RelayCommand ClearSpectrCommand => _clearSpectrCommand ?? (_clearSpectrCommand = new RelayCommand(par =>
        {
            mainModel.ClearSpectr();
            mainModel.SwitchAdcBoard(1);

        }, o => mainModel.Connecting.Value));
        #endregion

        #region выбранный диапазон счетчика
        private CountDiapasone _selectedCountDiapasone;

        public CountDiapasone SelectedCountDiapasone
        {
            get { return _selectedCountDiapasone; }
            set { Set(ref _selectedCountDiapasone, value); }
        }
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

        #region запись параметров Ethernet параметров платы
        RelayCommand _writeEthParamsCommand;
        public RelayCommand WriteEthParamsCommand => _writeEthParamsCommand ?? (_writeEthParamsCommand = new RelayCommand(par => 
        {
            if (mainModel.CommMode.EthEnable) mainModel.Tcp.SetIPAddr(mainModel.IP, mainModel.Mask, mainModel.GateWay);
        }, 
            o => mainModel.Connecting.Value));
        #endregion
  
        #endregion
        public MainModel mainModel { get; } = new MainModel();

        #region Конструктор
        public VM()
        {
            mainModel.ModelProcess();
            mainModel.UpdateDataEvent += AddDataToCollection;
            timer.Elapsed += (s, e) => CurPcDateTime = DateTime.Now;            
            timer.Start();
            Events = new Events(mainModel);
            GetEventHistory();
            Events.EventExecute += AddHistoryItem;
            _selectedEventItems.Filter += OnEventsFiltered;
            _selectedEventItems.SortDescriptions.Add(new SortDescription("EventTime", ListSortDirection.Descending));
            GetMeasDates();
            UdpInit();            
        }


        #endregion

        private UDP _udp;

        public UDP Udp
        {
            get { return _udp; }
            set { Set(ref _udp, value); }
        }

        

        void UdpInit()
        {
            Udp = new UDP();
            Udp.UpdateOscillEvent += (collection) =>
            {
                UpdateAdcTrend(collection);
                if (Udp.Mode == 2 && IsSpectrLogging)
                {
                    var parameters = $"hv={mainModel.TelemetryHV.VoltageCurOut.Value},syn_level={mainModel.AdcBoardSettings.AdcSyncLevel.Value},preamp_gain={mainModel.AdcBoardSettings.PreampGain.Value}";
                    SpectrLogService.WriteToFile(SpetrLogPath, parameters, collection);
                    IsSpectrLogging = false;
                }
            };
            Udp.UpdateAmplitudesEvent += UpdateMaxAmpsData;
        }

        #region Текущая дата-время компьютера
        private DateTime _curPcDateTime;

        public DateTime CurPcDateTime
        {
            get { return _curPcDateTime; }
            set { Set(ref _curPcDateTime, value); }
        }


        #endregion

        #region таймер
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        #endregion

        #region Cписок доступных Com портов 
        string[] _comPorts;
        public string[] ComPorts { get => _comPorts ?? (_comPorts = SerialPort.GetPortNames()); set => Set(ref _comPorts, value); }
        #endregion

        #region Аналоговые входы
        public List<AnalogInput> AnalogInputs => mainModel.AnalogGroups.Select(g => g.AI).ToList();
        #endregion

        #region Аналоговые выходы
        public List<AnalogOutput> AnalogOutputs => mainModel.AnalogGroups.Select(g => g.AO).ToList();
        #endregion

        #region Данные для текущего тренда
        #region Алгоритм интерполяции
        int _interpolIndex = 3;
        public int InterpolIndex { get => _interpolIndex; set { Set(ref _interpolIndex, value); } }
        #endregion

        #region Настройки тренда
        GraphSettings _trendSettings;
        public GraphSettings TrendSettings { get => _trendSettings ?? (_trendSettings = ClassInit<GraphSettings>()); }
        #endregion        
        
        ObservableCollection<TimePoint> _plotCollection;
        public ObservableCollection<TimePoint> PlotCollection
        {
            get => _plotCollection ?? (_plotCollection = new ObservableCollection<TimePoint>());
        }
        #endregion

        #region Настройки видимости графиков
        private TrendVisible _trendsVisible;
        public TrendVisible TrendsVisible => _trendsVisible ?? (_trendsVisible = ClassInit<TrendVisible>());
        #endregion

        #region Данные для архивного тренда
        #region Коллекция
        IEnumerable<TimePoint> _archivalDataPotnts;
        public IEnumerable<TimePoint> ArchivalDataPotnts { get => _archivalDataPotnts; private set { Set(ref _archivalDataPotnts, value); } }
        #endregion

        #region Настройки
        #region Стартовая точка отображаемого тренда
        DateTime _displayDateStart = DateTime.Today.AddDays(-1);
        public DateTime DisplayDateStart
        {
            get => _displayDateStart;
            set
            {
                Set(ref _displayDateStart, value);
                if (value >= DisplayDateEnd) DisplayDateEnd = value.AddMinutes(1);
                if (value.AddDays(2) < DisplayDateEnd) DisplayDateEnd = value.AddDays(2);
            }
        }
        #endregion
        #region Конечная точка отображаемого тренда
        DateTime _displayDateEnd = DateTime.Today;
        public DateTime DisplayDateEnd
        {
            get => _displayDateEnd;
            set
            {
                Set(ref _displayDateEnd, value);
                if (value <= DisplayDateStart) DisplayDateStart = value.AddMinutes(-1);
                if (value.AddDays(-2) > DisplayDateStart) DisplayDateStart = value.AddDays(-2);
            }
        }
        #endregion
        #region Путь к логируемому файлу
        string _logPath;
        public string LogPath { get => _logPath; set => Set(ref _logPath, value); }
        #endregion

        #region Список дат, когда происходили измерения
        List<BlackOutData> MeasDates { get; set; } = new List<BlackOutData>();
        void GetMeasDates()
        {
            MeasDates = SqlMethods.ReadFromSql<BlackOutData>($"SELECT DISTINCT date(time) FROM TimePoints;");
        }
        #endregion

        #endregion

        #region Состояние загрузки из ДБ
        bool _archivalTrendDownloading;
        public bool ArchivalTrendDownloading { get => _archivalTrendDownloading; private set { Set(ref _archivalTrendDownloading, value); } }
        #endregion

        #region Состояние загрузки в текстовый файл
        bool _archivalTrendUploading;
        public bool ArchivalTrendUploading { get => _archivalTrendUploading; private set { Set(ref _archivalTrendUploading, value); } }
        #endregion

        #endregion

        #region Данные перечислений        

        #region Доступные ЕИ
        public IEnumerable<MeasUnitSettings> AvialableMeasUnitSettings => mainModel.MeasUnitSettings.Where(mu => mu.A.Value != 0 || mu.B.Value != 0).Select(mu => mu);
        
        #endregion


        //#endregion

        #region Названия типов измерений
        public DataBaseCollection<EnumCustom> MeasTypesNamesNames { get; } = new DataBaseCollection<EnumCustom>("MeasTypesNamesNames", new EnumCustom());
        #endregion

        #region Названия источников компенсаций
        public DataBaseCollection<EnumCustom> CompensationSrcNames { get; } = new DataBaseCollection<EnumCustom>("CompensationSrcNames", new EnumCustom());
        #endregion

        #region Названия типов калибровок
        public DataBaseCollection<EnumCustom> TypeCalibrations { get; } = new DataBaseCollection<EnumCustom>("TypeCalibrations", new EnumCustom());
        #endregion

        #region Названия измерительных процессов
        public DataBaseCollection<EnumCustom> MeasProcNames { get; } = new DataBaseCollection<EnumCustom>("MeasProcNames", new EnumCustom());
        #endregion

        #region Названия стандартизаций
        public DataBaseCollection<EnumCustom> StandNames { get; } = new DataBaseCollection<EnumCustom>("StandNames", new EnumCustom());
        #endregion
        #endregion        

        #region Вывести данные из БД
        async void ShowArchivalTrend()
        {
            try
            {
                ArchivalTrendDownloading = true;
                await Task.Run(() =>
                {
                    var list = SqlMethods.ReadFromSql<TimePoint>($"SELECT * FROM TimePoints WHERE time >= datetime('{DisplayDateStart.ToString("u")}') AND time <= datetime('{DisplayDateEnd.ToString("u")}');");
                    ArchivalDataPotnts = list;
                    ArchivalTrendDownloading = false;
                });
                
            }
            catch (Exception)
            {

            }
        }
        #endregion

        public IEnumerable<string> UnitNames => mainModel.MeasUnitSettings.Select(mu => mu.MeasUnitName.Value);

        #region Добавление данных в график
        void AddDataToCollection()
        {
            App.Current?.Dispatcher?.Invoke(() =>
                {
                    var tp = new TimePoint
                    {
                        time = DateTime.Now,
                        y1 = mainModel.MeasResults[0].CounterValue.Value,
                        y2 = mainModel.MeasResults[0].PhysValueCur.Value,
                        y3 = mainModel.MeasResults[0].PhysValueAvg.Value,
                        y4 = mainModel.MeasResults[1].CounterValue.Value,
                        y5 = mainModel.MeasResults[1].PhysValueCur.Value,
                        y6 = mainModel.MeasResults[1].PhysValueAvg.Value,
                        y7 = mainModel.AnalogGroups[0].AI.AdcValue.Value,
                        y8 = mainModel.AnalogGroups[1].AI.AdcValue.Value,
                        y9 = mainModel.TelemetryHV.VoltageCurOut.Value,
                        y10 = mainModel.TempTelemetry.TempInternal.Value
                    };
                    PlotCollection.Add(tp);
                    int i = 0;
                    while (PlotCollection.Count > 0 && PlotCollection[0].time < DateTime.Now.AddMinutes(TrendSettings.PlotTime * (-1)) && i<5)
                    {
                        PlotCollection.RemoveAt(0);
                        i++;
                    }
                    SqlMethods.WritetoDb<TimePoint>(tp);                    
                }); 

        }
        #endregion

        #region Инициализация унивесального параметра
        T ClassInit<T>() where T : PropertyChangedBase
        {
            T cell = XmlMethods.GetParam<T>().FirstOrDefault();
            if (cell == null)
            {
                cell = (T)Activator.CreateInstance(typeof(T));
                XmlMethods.AddToXml<T>(cell);
            }
            cell.PropertyChanged += (sender, e) => XmlMethods.EditParam<T>(cell, e.PropertyName);
            return cell;
        }
        #endregion

        #region Время усреднения для пользователя(запись)
        uint _avgTimeWrite = 1;
        public uint AvgTimeWrite
        {
            get => _avgTimeWrite;
            set 
            {
                if (value > 0) Set(ref _avgTimeWrite, value);
            }
        }
        #endregion

        #region Данные UDP
        DateTime lastUpdateTime = DateTime.Now;
        #region Данные тренда
        IEnumerable<Point> _adcDataTrend;
        public IEnumerable<Point> AdcDataTrend
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
        IEnumerable<Point> _maxAmplitudesData;
        /// <summary>
        /// Данные гистограммы в режиме максимальных амплитуд
        /// </summary>
        public IEnumerable<Point> MaxAmplitudesData
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

        #region Запись в файл
        async void WriteArchivalTrendToText()
        {
            if (ArchivalTrendUploading) return;
            try
            {
                ArchivalTrendUploading = true;
                await Task.Run(() =>
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("Дата Время" + "\t" + "Процесс 0: счетчик" + "\t" + "Процесс 0: мгновенная ФВ" + "\t"
                             + "Процесс 0: усредненная ФВ" + "\t" + "Процесс 1: счетчик" + "\t"
                             + "Процесс 1: мгновенная ФВ" + "\t" + "Процесс 1: усредненная ФВ" + "\t"
                             + "Ток AI0, мкA" + "\t" + "Ток AI1, мкA" + "\t" + "Значение HV out, V" + "\t" + "Температура" + "\n");
                    foreach (var item in ArchivalDataPotnts)
                    {
                        builder.Append(item.time.ToString("dd/MM/yyyy HH:mm:ss:f") + "\t" + item.y1.ToString("0.000") + "\t" + item.y2.ToString("0.000") + "\t"
                             + item.y3.ToString("0.000") + "\t" + item.y4.ToString("0.000") + "\t"
                             + item.y5.ToString("0.000") + "\t" + item.y6.ToString("0.000") + "\t"
                             + item.y7.ToString("0.000") + "\t" + item.y8.ToString("0.000") + "\t" + item.y9.ToString("0.000") + "\t" + item.y10.ToString("0.000") + "\n");
                    }
                    using (StreamWriter sw = new StreamWriter(LogPath, false, System.Text.Encoding.Default))
                    {
                        sw.WriteLine(builder.ToString());
                    }
                });
                ArchivalTrendUploading = false;
            }
            catch (Exception ex)
            {
                
            }
        }
        #endregion        

        #region События
        #region Фильтрация событий
        #region Начальная точка
        private DateTime _startPointHistoryEvent = DateTime.Today;

        public DateTime StartPointHistoryEvent
        {
            get { return _startPointHistoryEvent; }
            set { Set(ref _startPointHistoryEvent, value); }
        }
        #endregion

        #region Конечная точка
        private DateTime _endPointHistoryEvent = DateTime.Now;

        public DateTime EndPointHistoryEvent
        {
            get { return _endPointHistoryEvent; }
            set { Set(ref _endPointHistoryEvent, value); }
        }
        #endregion

        private readonly CollectionViewSource _selectedEventItems = new CollectionViewSource();
        public ICollectionView SelectedEventItems => _selectedEventItems.View;

        private string _eventFilterText;
        /// <summary>
        /// Текст фильтра событий
        /// </summary>
        public string EventFilterText
        {
            get { return _eventFilterText; }
            set 
            {
                if (!Set(ref _eventFilterText, value)) return;
                _selectedEventItems.View.Refresh();
            }
        }

        private void OnEventsFiltered(object sender, FilterEventArgs e)
        {
            if (!(e.Item is HistoryEventItem eventItem))
            {
                e.Accepted = false;
                return;
            }
            var text = EventFilterText;
            if (string.IsNullOrWhiteSpace(text)) return;
            if (eventItem.Event.Id.Contains(text)) return;
            if (eventItem.Event.Num.ToString().Contains(text)) return;
            if (eventItem.Event.Description.Contains(text)) return;
            if (eventItem.Event.Title.Contains(text)) return;
            if (eventItem.UserName.Contains(text)) return;
            if (eventItem.EventTime.ToString().Contains(text)) return;
            e.Accepted = false;
        }
        #endregion

        #region Класс событий
        public Events Events { get; }
        #endregion

        #region История событий
        /// <summary>
        /// Коллекция истории событий
        /// </summary>
        private ObservableCollection<HistoryEventItem> _historyEventItems;
        public ObservableCollection<HistoryEventItem> HistoryEventItems
        {
            get => _historyEventItems;
            private set
            {
                if (!Set(ref _historyEventItems, value)) return;
                _selectedEventItems.Source = value; 
                OnPropertyChanged(nameof(SelectedEventItems));
            }
        } 
        #endregion

        #region Флаг загрузки данных событий из БД
        private bool _historyItemDownloading;
        /// <summary>
        /// Флаг загрузки данных событий из БД
        /// </summary>
        public bool HistoryItemDownloading
        {
            get { return _historyItemDownloading; }
            set { Set(ref _historyItemDownloading, value); }
        }

        #endregion

        #region Метод загрузки событий из базы данных
        async void AddHistoryItemsFromDb()
        {
            HistoryItemDownloading = true;
            await Task.Run(() => GetEventHistory());
            HistoryItemDownloading = false;
        }
        void GetEventHistory()
        {
            try
            {
                var list = SqlMethods.ReadFromSql<HistoryEventItemDb>($"SELECT * FROM HistoryEventItemDbs WHERE EventTime >= datetime('{StartPointHistoryEvent.ToString("u")}') AND EventTime <= datetime('{EndPointHistoryEvent.ToString("u")}');");
                App.Current.Dispatcher.Invoke(() =>
                {
                    HistoryEventItems = new ObservableCollection<HistoryEventItem>();
                    for (int i = 0; i < list.Count; i++)
                        HistoryEventItems.Add(new HistoryEventItem()
                        {
                            UserName = list[i].UserName,
                            EventTime = list[i].EventTime,
                            Activity = list[i].Activity,
                            Event = Events.EventDevices.Where(dev => dev.Id == list[i].Id).FirstOrDefault()
                        });
                });                        
            }
            catch (Exception)
            {

            }
        }
        #endregion
               

        #region Метод загрузки события в базу данных и коллекцию
        void AddHistoryItem(EventDevice device)
        {            
            App.Current?.Dispatcher.Invoke(()=> 
            {
                var time = DateTime.Now;
                var name = CurUser == null ? "Пользователь не авторизован" : $"{CurUser.Somename} {CurUser.Name}";
                HistoryEventItems.Add(new HistoryEventItem()
                {
                    Event = device,
                    EventTime = time,
                    UserName = name,
                    Activity = device.IsActive
                }); ;
                try
                {
                    SqlMethods.WritetoDb<HistoryEventItemDb>(new HistoryEventItemDb
                    {
                        EventTime = time,
                        Id = device.Id,
                        Activity = device.IsActive,
                        UserName = name
                    });
                }
                catch (Exception)
                {
                    
                }
            });
        }
        #endregion


        void UpadateDates(object obj)
        {
            if (!(obj is Calendar calendar)) return;
            DateTime startDate = new DateTime(calendar.DisplayDate.Year,calendar.DisplayDate.Month,1);
            var enabledDates = MeasDates.Where(ms => ms.Time >= startDate && ms.Time <= startDate.AddMonths(1)).Select(ms=>ms.Time);            
            calendar.DisplayDateEnd = startDate.AddMonths(1);            
            var tempDate = startDate;
            calendar.BlackoutDates.Clear();
            while (tempDate<= calendar.DisplayDateEnd)
            {
                if (tempDate != calendar.SelectedDate && !enabledDates.Any(dt=>dt==tempDate))
                    calendar.BlackoutDates.Add(new CalendarDateRange(tempDate));
                tempDate = tempDate.AddDays(1);
            }  
        }
        private RelayCommand _selectDatesCommand;

        public RelayCommand SelectDatesCommand
        {
            get { return _selectDatesCommand ?? (_selectDatesCommand = new RelayCommand(obj => UpadateDates(obj), obj => true)); }
            
        }
        #endregion

    }
}
