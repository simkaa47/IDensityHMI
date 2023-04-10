using IDensity.AddClasses.EventHistory;
using IDensity.Core.Models.Events;
using IDensity.Core.Models.Users;
using IDensity.Core.ViewModels;
using IDensity.Core.ViewModels.MeasUnits;
using IDensity.DataAccess;
using IDensity.Models;
using IDensity.Models.SQL;
using IDensity.Services.ComminicationServices;
using IDensity.Services.SQL;
using IDensity.ViewModels.Commands;
using IDensity.ViewModels.MasrerSettings;
using IDensity.ViewModels.SdCard;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
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
        public MeasResultsViewModel MeasResultsVM { get; private set; }
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
            MeasResultsVM = new MeasResultsViewModel(this);
            CommService.Tcp.TcpEvent += WriteTcpLog;
            


        }

        #region Инициализация сервисов
        void ServicesInit()
        {
            CommService = new CommunicationService(mainModel);
            CommService.MainProcessExecute();

        }
        #endregion

        async void WriteTcpLog(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("tcplog.txt", true))
                {
                    await writer.WriteLineAsync($"{DateTime.Now.ToString("dd:MM:YYYY HH:mm:ss")}:{message}");
                }
            }
            catch (Exception)
            {

            }
        }

        #region Версия ПО
        public string SoftVersion { get; private set; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        #endregion


        #region Пользователи

        #region Текущий пользователь
        User _curUser;
        public User CurUser
        {
            get => _curUser;
            set
            {
                if (Set(ref _curUser, value))
                {
                    IsAuthorized = _curUser != null;
                }
            }
        }
        #endregion

        #region Авторизован пользователь или нет
        /// <summary>
        /// Авторизован пользователь или нет
        /// </summary>
        private bool _isAuthorized;
        /// <summary>
        /// Авторизован пользователь или нет
        /// </summary>
        public bool IsAuthorized
        {
            get => _isAuthorized;
            set => Set(ref _isAuthorized, value);
        }
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
            const string message = "Закрыть приложение?";
            const string caption = "Закрытие приложения";

            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }

        }, o => true));
        #endregion        

        #region Команда вкл-выкл измерения
        RelayCommand _switchMeasCommand;
        public RelayCommand SwitchMeasCommand { get => _switchMeasCommand ?? (_switchMeasCommand = new RelayCommand(o => CommService.SwitchMeas(), o => true)); }
        #endregion        

        #region Команда "Логаут"
        RelayCommand _logoutCommand;
        public RelayCommand LogoutCommand { get => _logoutCommand ?? (_logoutCommand = new RelayCommand(o => CurUser = null, o => true)); }
        #endregion                         

        #region Команды измерения даты-времени
        #region установить RTC пользователя
        RelayCommand _setRtcCommand;
        public RelayCommand SetRtcCommand => _setRtcCommand ?? (_setRtcCommand = new RelayCommand(execPar =>
        {
            mainModel.Rtc.IsWriting = true;
            CommService.SetRtc(mainModel.Rtc.WriteValue);
        }, canExecPar => true));
        #endregion
        #region Синхронизировать c временем ПК
        RelayCommand _syncRtcCommand;
        public RelayCommand SyncRtcCommand => _syncRtcCommand ?? (_syncRtcCommand = new RelayCommand(execPar =>
        {
            mainModel.Rtc.IsWriting = true;
            CommService.SetRtc(DateTime.Now);
        }, canExecPar => true));
        #endregion
        #endregion                  

        #region Команды вывода и фильтрации событий
        private RelayCommand _showEventsCommand;

        public RelayCommand ShowEventsCommand => _showEventsCommand ?? (_showEventsCommand = new RelayCommand(exec => AddHistoryItemsFromDb(), canExex => true));

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
                ServicesInit();
                InitVm();
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

        #endregion
    }
}
