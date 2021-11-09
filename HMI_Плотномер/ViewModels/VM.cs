using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HMI_Плотномер.AddClasses;
using HMI_Плотномер.Models;
using HMI_Плотномер.Models.SQL;
using HMI_Плотномер.Models.XML;
using HMI_Плотномер.ViewModels.Commands;
using OxyPlot.Axes;

namespace HMI_Плотномер.ViewModels
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

        #region Команда "Записать архивный тренд в файл"
        RelayCommand _writeLogCommand;
        public RelayCommand WriteLogCommand { get => _writeLogCommand ?? (_writeLogCommand = new RelayCommand(o => WriteArchivalTrendToText(), o => true)); }
        #endregion


        #endregion
        public MainModel mainModel { get; } = new MainModel();

        #region Конструктор
        public VM()
        {
            mainModel.ModelProcess();
            mainModel.UpdateDataEvent += AddDataToCollection;
        }
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


    }
}
