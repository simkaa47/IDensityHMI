using HMI_Плотномер.AddClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HMI_Плотномер.Models
{
    class MainModel: PropertyChangedBase
    {
        #region Спсоб соединения с платой
        enum ConnectionMode
        {
            RS485,
            TCP,
            HART
        }
        #endregion
        #region Данные для графика
        ObservableCollection<TimePoint> _plotCollection;
        public ObservableCollection<TimePoint> PlotCollection
        {
            get => _plotCollection ?? (_plotCollection = new ObservableCollection<TimePoint>());
        }
        #endregion

        #region RTC контроллера платы
        DateTime _rtc;
        /// <summary>
        /// Часы реального времени в плате контроллера
        /// </summary>
        public DateTime Rtc { get => _rtc; set => Set(ref _rtc, value); }

        #endregion

        #region Статус соединения с платой
        bool _connecting;
        /// <summary>
        /// Статус соединения с платой
        /// </summary>
        public bool Connecting { get => _connecting; private set => Set(ref _connecting, value); }
        #endregion

        public async void ModelProcess()
        {
            await Task.Run(() => AddCollection());
        }

        void AddCollection()
        {
            int sdfsd = 14;
            sdfsd++;
            while (true)
            {
                
                App.Current?.Dispatcher?.Invoke(Add) ;
                Thread.Sleep(1000);
            }
        }
        void Add()
        {
            PlotCollection.Add(new TimePoint { time = DateTime.Now, y = new Random().Next(18, 32) });
        }
    }
}
