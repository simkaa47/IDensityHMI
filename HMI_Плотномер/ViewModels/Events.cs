using IDensity.AddClasses;
using IDensity.AddClasses.EventHistory;
using IDensity.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IDensity.ViewModels
{
    class Events
    {
        #region Произошло событие
        public event Action<EventDevice> EventExecute;
        #endregion
        #region Модель, в которой происходят события
        /// <summary>
        /// Модель, в которой происходят события
        /// </summary>
        MainModel Model { get; set; }
        #endregion
        #region Конструктор
        public Events(MainModel model)
        {
            this.Model = model;
            GetEventCollection();
            Describe();           
        }
        #endregion
        #region События
        #region События связи
        #region Событие потери связи с платой
        public EventDevice EventBoardCommErr { get; } = new EventDevice("EventBoardCommErr");
        #endregion
        #region Событие потери связи с платой HV
        public EventDevice EventHvBoardCommErr { get; } = new EventDevice("EventHvBoardCommErr");
        #endregion
        #region Событие потери связи с платой ADC
        public EventDevice EventAdcBoardCommErr { get; } = new EventDevice("EventAdcBoardCommErr");
        #endregion
        #endregion
        #endregion

        #region Коллеция событий
        /// <summary>
        /// Коллеция событий
        /// </summary>
        public List<EventDevice> EventDevices { get;set; }

        #region Метод инициализации коллекции событий
        void GetEventCollection()
        {
            EventDevices = this.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(EventDevice))
                .Select(p => p.GetValue(this) as EventDevice)
                .ToList();
            foreach (var d in EventDevices)
            {
                d.EventExecuted += OnEventExecute;
            }
        }
        #endregion
        #endregion

        #region Подписка событий на события
        /// <summary>
        /// В этом методе все события подписываютя на изменение соответсвующих их свойств
        /// </summary>
        void Describe()
        {
            Model.Connecting.PropertyChanged += (obj, args) =>
            {
                if ((obj as Parameter<bool>) != null && args.PropertyName == nameof(Parameter<bool>.Value))
                {
                    EventBoardCommErr.IsActive = !(obj as Parameter<bool>).Value;
                }
            };          

        }
        #endregion

        #region Действие по наступлению события
        void OnEventExecute(EventDevice device)
        {
            EventExecute?.Invoke(device);
        } 
        #endregion






    }
}


