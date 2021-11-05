using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
        public User CurUser { get; set; }
        public string CurUserName { get => CurUser.Somename + " " + CurUser.Name; }
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
        IEnumerable<TimePoint> _archivalDataPotnts;
        public IEnumerable<TimePoint> ArchivalDataPotnts { get => _archivalDataPotnts; private set { Set(ref _archivalDataPotnts, value); } }
        #endregion

        #region Вывести данные из БД
        void ShowArchivalTrend()
        {
            var list = SqlMethods.ReadFromSql<TimePoint>("SELECT * FROM TimePoints");
            ArchivalDataPotnts = list;
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


    }
}
