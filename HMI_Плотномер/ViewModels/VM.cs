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
        public string CurUserName { get => CurUser.Somename +" "+ CurUser.Name; }
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

        #endregion
        public MainModel mainModel { get;} = new MainModel();

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

        TimeSpan _plotTime = new TimeSpan(0,1,0);
        public double PlotTime 
        { 
            get => DateTimeAxis.ToDouble(_plotTime)/100000; 
        }



        #endregion
        
        
    }  
}
