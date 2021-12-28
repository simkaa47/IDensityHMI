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
        public RelayCommand UpdateComPortListCommand => _updateComPortListCommand ?? (_updateComPortListCommand = new RelayCommand(o => ComPorts=SerialPort.GetPortNames(), o => true));
        #endregion

        #region Команда "Закрыть приложение"
        RelayCommand _closeAppCommand;
        public RelayCommand CloseAppCommand => _closeAppCommand ?? (_closeAppCommand = new RelayCommand(o => Application.Current.Shutdown(), o => true));
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

        #region Команды настроек измерительных процессов
        #region Команда "Записать номер калибровочной кривой в диапазоне"
        RelayCommand _setCalibCurveDiapCommand;
        public RelayCommand SetCalibCurveDiapCommand
        {
            get => _setCalibCurveDiapCommand ?? (_setCalibCurveDiapCommand = new RelayCommand(execPar => 
            {
                MeasProcess process = SelectedMeasProcess.Clone() as MeasProcess;
                process.Ranges[SelectedDiapNum].CalibCurveNum.Value = process.Ranges[SelectedDiapNum].CalibCurveNum.WriteValue;
                mainModel.SetMeasProcessSettings(process, SelectedMeasProcessNum);
            }, 
                canExecPar => true));
            
        }
        #endregion
        #region Команда "Записать номер стандартизации в диапазоне"
        RelayCommand _setStandNumDiapCommand;
        public RelayCommand SetStandNumDiapCommand
        {
            get => _setStandNumDiapCommand ?? (_setStandNumDiapCommand = new RelayCommand(execPar =>
            {
                MeasProcess process = SelectedMeasProcess.Clone() as MeasProcess;
                process.Ranges[SelectedDiapNum].StandNum.Value = process.Ranges[SelectedDiapNum].StandNum.WriteValue;
                mainModel.SetMeasProcessSettings(process, SelectedMeasProcessNum);
            },
                canExecPar => true));

        }
        #endregion
        #region Команда "Записать счетчик в диапазоне"
        RelayCommand _setCounterDiapCommand;
        public RelayCommand SetCounterDiapCommand
        {
            get => _setCounterDiapCommand ?? (_setCounterDiapCommand = new RelayCommand(execPar =>
            {
                MeasProcess process = SelectedMeasProcess.Clone() as MeasProcess;
                process.Ranges[SelectedDiapNum].CounterNum.Value = process.Ranges[SelectedDiapNum].CounterNum.WriteValue;
                mainModel.SetMeasProcessSettings(process, SelectedMeasProcessNum);
            },
                canExecPar => true));

        }
        #endregion
        #region Команда "Записать номер стандартизации"
        RelayCommand _setStandNumCommand;
        public RelayCommand SetStandNumCommand
        {
            get => _setStandNumCommand ?? (_setStandNumCommand = new RelayCommand(execPar =>
            {
                MeasProcess process = SelectedMeasProcess.Clone() as MeasProcess;
                process.BackStandNum.Value = process.BackStandNum.WriteValue;
                mainModel.SetMeasProcessSettings(process, SelectedMeasProcessNum);
            },
                canExecPar => true));
        }
        #endregion
        #region Команда "Записать время измерения одной точки"
        RelayCommand _setMeasDurationCommand;
        public RelayCommand SetMeasDurationCommand
        {
            get => _setMeasDurationCommand ?? (_setMeasDurationCommand = new RelayCommand(execPar =>
            {
                MeasProcess process = SelectedMeasProcess.Clone() as MeasProcess;
                process.MeasDuration.Value = process.MeasDuration.WriteValue;
                mainModel.SetMeasProcessSettings(process, SelectedMeasProcessNum);
            },
                canExecPar => true));
        }
        #endregion
        #region Команда "Записать глубину усреднения"
        RelayCommand _setMeasDeepCommand;
        public RelayCommand SetMeasDeepCommand
        {
            get => _setMeasDeepCommand ?? (_setMeasDeepCommand = new RelayCommand(execPar =>
            {
                MeasProcess process = SelectedMeasProcess.Clone() as MeasProcess;
                process.MeasDeep.Value = process.MeasDeep.WriteValue;
                mainModel.SetMeasProcessSettings(process, SelectedMeasProcessNum);
            },
                canExecPar => true));
        }
        #endregion
        #region Команда "Отправить данные периода-полураспада"
        RelayCommand _setHalfLifeCommand;
        public RelayCommand SetHalfLifeCommand { 
            get => _setHalfLifeCommand ?? (_setHalfLifeCommand = new RelayCommand(execPar => 
            {
                MeasProcess process = SelectedMeasProcess.Clone() as MeasProcess;
                process.HalfLife.Value = process.HalfLife.WriteValue;
                mainModel.SetMeasProcessSettings(process, SelectedMeasProcessNum);
            }, 
                canExecPar => true)); }
        #endregion
        #region Команда "Отправить данные плотности жидкости"
        RelayCommand _setDensityLiqCommand;
        public RelayCommand SetDensityLiqCommand
        {
            get => _setDensityLiqCommand ?? (_setDensityLiqCommand = new RelayCommand(execPar =>
            {
                MeasProcess process = SelectedMeasProcess.Clone() as MeasProcess;
                process.DensityLiquid.Value = process.DensityLiquid.WriteValue;
                mainModel.SetMeasProcessSettings(process, SelectedMeasProcessNum);
            },
                canExecPar => true));
        }
        #endregion
        #region Команда "Отправить данные плотности твердости"
        RelayCommand _setDensitySolidCommand;
        public RelayCommand SetDensitySolidCommand
        {
            get => _setDensitySolidCommand ?? (_setDensitySolidCommand = new RelayCommand(execPar =>
            {
                MeasProcess process = SelectedMeasProcess.Clone() as MeasProcess;
                process.DensitySolid.Value = process.DensitySolid.WriteValue;
                mainModel.SetMeasProcessSettings(process, SelectedMeasProcessNum);
            },
                canExecPar => true));
        }
        #endregion
        #region Команда "Сменить номер измерительного процесса"
        RelayCommand _changeMeasProcessCommand;
        public RelayCommand ChangeMeasProcessCommand => _changeMeasProcessCommand ?? (_changeMeasProcessCommand = new RelayCommand(execPar => mainModel.ChangeMeasProcess((int)execPar), canExecPar => canExecPar != null));
        #endregion

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
        public RelayCommand ChangeBaudrateCommand => _changeBaudrateCommand ?? (_changeBaudrateCommand = new RelayCommand(o => mainModel.ChangeBaudrate((int)o), o =>o != null));
        #endregion
        #endregion

        #region Команда изменения времени усреднения для пользователя
        RelayCommand _changeUserAvgTime;
        public RelayCommand ChangeUserAvgTime => _changeUserAvgTime ?? (_changeUserAvgTime = new RelayCommand(o => 
        {
            if (mainModel.CurMeasProcess.MeasDuration.Value > 0)
            {
                MeasProcess process = mainModel.CurMeasProcess.Clone() as MeasProcess;
                process.MeasDeep.Value = (ushort)(((uint)o)*10/ mainModel.CurMeasProcess.MeasDuration.Value);
                mainModel.SetMeasProcessSettings(process, SelectedMeasProcessNum);
            }
        
        }, o => true));

        #endregion

        #region Команда "Установить напряжение HV"
        RelayCommand _setHvCommand;
        public RelayCommand SetHvCommand => _setHvCommand ?? (_setHvCommand = new RelayCommand(obj =>mainModel.SetHv(mainModel.TelemetryHV.VoltageSV.WriteValue), obj => true));
        #endregion

        #region Команды вывода и фильтрации событий
        private RelayCommand _showEventsCommand;

        public RelayCommand ShowEventsCommand => _showEventsCommand ?? (_showEventsCommand = new RelayCommand(exec => AddHistoryItemsFromDb(), canExex => true));

        #endregion

        #endregion
        public MainModel mainModel { get; } = new MainModel();

        #region Конструктор
        public VM()
        {
            mainModel.ModelProcess();
            mainModel.UpdateDataEvent += AddDataToCollection;
            timer.Elapsed += (s, e) => CurPcDateTime.Value = DateTime.Now;
            timer.Start();
            Events = new Events(mainModel);
            GetEventHistory();
            Events.EventExecute += AddHistoryItem;
        }
        #endregion

        #region Текущая дата-время компьютера
        public Parameter<DateTime> CurPcDateTime { get; private set; } = new Parameter<DateTime>("CurPcDateTime", "Текущие время и дата компьютера", DateTime.MinValue, DateTime.MaxValue, 0, "");
        
        #endregion

        #region таймер
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        #endregion

        #region Cписок доступных Com портов 
        string[] _comPorts;
        public string[] ComPorts { get => _comPorts ?? (_comPorts = SerialPort.GetPortNames()); set => Set(ref _comPorts, value); }
        #endregion

        #region Аналоговые входы
        public List<AnalogInput> AnalogInputs  => mainModel.AnalogGroups.Select(g => g.AI).ToList();
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
        #region Названия измерительных процессов
        public DataBaseCollection<EnumCustom> MeasProcessNames { get; } = new DataBaseCollection<EnumCustom>("MeasProcessNames", new EnumCustom());
        #endregion;        

        #region Названия единиц измерения
        public DataBaseCollection<EnumCustom> UnitNames { get; } = new DataBaseCollection<EnumCustom>("UnitNames", new EnumCustom());
        #endregion

        #region Названия переменных для аналогового выхода
        public DataBaseCollection<EnumCustom> TypeAnalogOutVars { get; } = new DataBaseCollection<EnumCustom>("TypeAnalogOutVars", new EnumCustom());
        #endregion
        #endregion

        #region Выбранный номер измерительного процесса (не текущий!)
        int _selectedMeasProcessNum;
        public int SelectedMeasProcessNum
        {
            get => _selectedMeasProcessNum;
            set 
            {
                if (value < mainModel.MeasProcesses.Length)
                {
                    Set(ref _selectedMeasProcessNum, value);
                    SelectedMeasProcess = mainModel.MeasProcesses[value];
                    SelectedDiap = SelectedMeasProcess.Ranges[SelectedDiapNum];
                }
                    

            } 
        }
        #endregion

        #region Выбранный измерительный процесс
        MeasProcess _selectedMeasProcess;
        public MeasProcess SelectedMeasProcess
        {
            get => _selectedMeasProcess ?? (_selectedMeasProcess = mainModel.MeasProcesses[0]);
            set => Set(ref _selectedMeasProcess, value);            
        }
        #endregion

        #region Выбранный номер диапазона
        int _selectedDiapNum;
        public int SelectedDiapNum
        {
            get => _selectedDiapNum;
            set
            {
                if (value < SelectedMeasProcess.Ranges.Length)
                {
                    Set(ref _selectedDiapNum, value);
                    SelectedDiap = SelectedMeasProcess.Ranges[value];
                }
            }
        }
        #endregion

        #region Выбранный диапазон
        public Diapasone _selectedDiap;
        public Diapasone SelectedDiap
        {
            get => _selectedDiap ?? (_selectedDiap = SelectedMeasProcess.Ranges[SelectedDiapNum]);
            set => Set(ref _selectedDiap, value);
        }
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

        #region Добавление данных в график
        void AddDataToCollection()
        {
            App.Current?.Dispatcher?.Invoke(
                () =>
                {
                    var tp = new TimePoint { time = DateTime.Now, y1 = mainModel.PhysValueAvg.Value, y2 = mainModel.PhysValueCur.Value };
                    PlotCollection.Add(tp);
                    while (PlotCollection.Count>0 && PlotCollection[0].time < DateTime.Now.AddMinutes(TrendSettings.PlotTime * (-1)))
                    {
                        PlotCollection.RemoveAt(0);
                    }
                    SqlMethods.WritetoDb<TimePoint>(tp);
                }); ;

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
                    foreach (var item in ArchivalDataPotnts)
                    {
                        builder.Append(item.time.ToString("dd/MM/yyyy HH:mm:ss:f") + "\t" + item.y1.ToString("0.000") + "\t" + item.y2.ToString("0.000" + "\n"));
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

        #endregion

        #region Класс событий
        public Events Events { get; }
        #endregion

        #region История событий
        /// <summary>
        /// Коллекция истории событий
        /// </summary>
        public ObservableCollection<HistoryEventItem> HistoryEventItems { get; private set; } = new ObservableCollection<HistoryEventItem>();
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
                    HistoryEventItems.Clear();
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
            App.Current.Dispatcher.Invoke(()=> 
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
