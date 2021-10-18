using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using HMI_Плотномер.AddClasses;
using HMI_Плотномер.Models;
using HMI_Плотномер.Models.SQL;

namespace HMI_Плотномер.ViewModels
{
    /// <summary>
    /// Общая ViewModel для окна
    /// </summary>
    class VM : PropertyChangedBase
    {
        #region Коллекция пользователей
        public DataBaseCollection<User> Users { get; } = new DataBaseCollection<User>("Users", new User { Login = "admin",  Password = "0000" });
        #endregion
        MainModel mainModel = new MainModel();

        ObservableCollection<TimePoint> timePoints;
        public ObservableCollection<TimePoint> TimePoints
        {
            get => timePoints ?? (timePoints = mainModel.PlotCollection);
        }

        #region Конструктор
        public VM()
        { 
            mainModel.ModelProcess();
        }
        #endregion
        #region Алгоритм интерполяции
        int _interpolIndex=3;
        public int InterpolIndex { get => _interpolIndex;set { Set(ref _interpolIndex, value); } }

        TimeSpan _plotTime = new TimeSpan( 0, 1, 0);
        public TimeSpan PlotTime { get => _plotTime; }


        #endregion
        
        
    }  
}
