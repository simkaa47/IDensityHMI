﻿using IDensity.AddClasses;
using IDensity.AddClasses.EventHistory;
using IDensity.Models;
using IDensity.Models.SQL;
using IDensity.Services.ComminicationServices;
using IDensity.Services.InitServices;
using IDensity.Services.SQL;
using IDensity.Services.XML;
using IDensity.ViewModels.Commands;
using IDensity.ViewModels.MasrerSettings;
using IDensity.ViewModels.SdCard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace IDensity.ViewModels
{
    /// <summary>
    /// Общая ViewModel для окна
    /// </summary>
    public class VM : PropertyChangedBase
    {
        #region Дочерние VM
        public CommonSettingsVm CommonSettingsVm { get; private set; }
        public MasterSettingsViewModel MasterSettingsViewModel { get; private set; }
        public AdcViewModel AdcViewModel { get; private set; }
        public ConnectSettingsVm ConnectSettingsVm { get; private set; }
        public SdCardVm SdCardVm { get; private set; }
        public MeasProcessVm MeasProcessVm { get; private set; }
        public AnalogVm AnalogVm { get; private set; }
        public MeasUnitVm MeasUnitSettingsVm { get; private set; }
        #endregion

        #region Cервисы
        public CommunicationService CommService { get; private set; }
        
        #endregion

        /// <summary>
        /// Инициализация дочерних Vm
        /// </summary>
        void InitVm()
        {
            CommonSettingsVm = new CommonSettingsVm(this);
            MasterSettingsViewModel = new MasterSettingsViewModel(this);
            AdcViewModel = new AdcViewModel(this);
            ConnectSettingsVm = new ConnectSettingsVm(this);
            MeasProcessVm = new MeasProcessVm(this);
            SdCardVm = new SdCardVm(this);
            AnalogVm = new AnalogVm(this);
            MeasUnitSettingsVm = new MeasUnitVm(this);
        }

        #region Инициализация сервисов
        void ServicesInit()
        {
            CommService = new CommunicationService(mainModel);
            CommService.UpdateDataEvent += AddDataToCollection;
            CommService.MainProcessExecute();

        }
        #endregion


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

        #region Команда "Закрыть приложение"
        RelayCommand _closeAppCommand;
        public RelayCommand CloseAppCommand => _closeAppCommand ?? (_closeAppCommand = new RelayCommand(o =>
        {
            AdcViewModel.UdpService?.Stop();
            Application.Current.Shutdown();
        }, o => true));
        #endregion        

        #region Команда вкл-выкл измерения
        RelayCommand _switchMeasCommand;
        public RelayCommand SwitchMeasCommand { get => _switchMeasCommand ?? (_switchMeasCommand = new RelayCommand(o => CommService.SwitchMeas(), o => true)); }
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
            CommService.SetRtc(mainModel.Rtc.WriteValue);
        }, canExecPar => true));
        #endregion
        #region Синхронизировать c временем ПК
        RelayCommand _syncRtcCommand;
        public RelayCommand SyncRtcCommand => _syncRtcCommand ?? (_syncRtcCommand = new RelayCommand(execPar =>
        {
            CommService.SetRtc(DateTime.Now);
        }, canExecPar => true));
        #endregion
        #endregion                  

        #region Команды вывода и фильтрации событий
        private RelayCommand _showEventsCommand;

        public RelayCommand ShowEventsCommand => _showEventsCommand ?? (_showEventsCommand = new RelayCommand(exec => AddHistoryItemsFromDb(), canExex => true));

        #endregion

        #region Переключить реле
        RelayCommand _switchRelayCommand;
        public RelayCommand SwitchRelayCommand => _switchRelayCommand ?? (_switchRelayCommand = new RelayCommand(par =>
        {
            ushort temp = 0;
            if (par != null && ushort.TryParse(par.ToString(), out temp))
            {
                mainModel.SwitchRelay(temp);
            }
        }, canExec => true));
        #endregion       

        #region Обновить доступные ЕИ
        RelayCommand _updateAvialablemeasUnitCommand;
        public RelayCommand UpdateAvialablemeasUnitCommand => _updateAvialablemeasUnitCommand ?? (_updateAvialablemeasUnitCommand = new RelayCommand(par =>
        {
            OnPropertyChanged(nameof(AvialableMeasUnitSettings));
        }, o => true));
        #endregion

        #endregion
        public MainModel mainModel { get; }

        #region Конструктор
        public VM()
        {
            if ((Application.Current is App))
            {
                mainModel = new MainModel();
                timer.Elapsed += (s, e) => CurPcDateTime = DateTime.Now;
                timer.Start();
                Events = new Events(mainModel);
                GetEventHistory();
                Events.EventExecute += AddHistoryItem;
                _selectedEventItems.Filter += OnEventsFiltered;
                _selectedEventItems.SortDescriptions.Add(new SortDescription("EventTime", ListSortDirection.Descending));
                GetMeasDates();
                ServicesInit();
                InitVm();
                TrendInit();
            }
        }

        #endregion        

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

        void TrendInit()
        {
            _trendSettings = XmlInit.ClassInit<GraphSettings>();
            _trendsVisible = XmlInit.ClassInit<TrendVisible>();
        }
        #region Настройки тренда
        GraphSettings _trendSettings;
        public GraphSettings TrendSettings => _trendSettings;
        #endregion

        ObservableCollection<TimePoint> _plotCollection;
        public ObservableCollection<TimePoint> PlotCollection
        {
            get => _plotCollection ?? (_plotCollection = new ObservableCollection<TimePoint>());
        }
        #endregion

        #region Настройки видимости графиков
        private TrendVisible _trendsVisible;
        public TrendVisible TrendsVisible => _trendsVisible;
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
            if (!(mainModel.CycleMeasStatus.Value || TrendSettings.WriteIfNoMeasState)) return;
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
                    while (PlotCollection.Count > 0 && PlotCollection[0].time < DateTime.Now.AddMinutes(TrendSettings.PlotTime * (-1)) && i < 5)
                    {
                        PlotCollection.RemoveAt(0);
                        i++;
                    }
                    SqlMethods.WritetoDb<TimePoint>(tp);
                });

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
            App.Current?.Dispatcher.Invoke(() =>
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
            DateTime startDate = new DateTime(calendar.DisplayDate.Year, calendar.DisplayDate.Month, 1);
            var enabledDates = MeasDates.Where(ms => ms.Time >= startDate && ms.Time <= startDate.AddMonths(1)).Select(ms => ms.Time);
            calendar.DisplayDateEnd = startDate.AddMonths(1);
            var tempDate = startDate;
            calendar.BlackoutDates.Clear();
            while (tempDate <= calendar.DisplayDateEnd)
            {
                if (tempDate != calendar.SelectedDate && !enabledDates.Any(dt => dt == tempDate))
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
