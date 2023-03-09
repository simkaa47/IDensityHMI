using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Timers;

namespace IDensity.AddClasses.Settings
{
    [DataContract]
    public class StandSettings:PropertyChangedBase
    {
        Timer standTimer = new Timer();
        
        public StandSettings(int id)
        {
            this.Id = id;            
            MeasUnitMemoryId = $"StandMeasMemory{id}";
        }        
        
        #region Id
        private int _id;
        /// <summary>
        /// Идентификатор стандартизации
        /// </summary>
        [DataMember]
        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }


        #endregion        
        #region Длительность стандартизации
        /// <summary>
        /// Длительность стандартизации
        /// </summary>
        [DataMember]
        public Parameter<ushort> StandDuration { get; set; } = new Parameter<ushort>("StandDuration", "Длительность стандартизации, x 0.1 c.", 0, 5, 1, "hold");
        #endregion
        #region Дата последней стандартизации
        /// <summary>
        /// Дата последней стандартизации
        /// </summary>
        [DataMember]
        public Parameter<DateTime> LastStandDate { get; set; } = new Parameter<DateTime>("LastStandDate", "Дата последней стандартизации", DateTime.MinValue, DateTime.MaxValue, 2, "hold");
        #endregion
        #region Результат
        /// <summary>
        /// Результат
        /// </summary>
        [DataMember]
        public Parameter<float> StandResult { get; set; } = new Parameter<float>("StandResult", "Результат", float.MinValue, float.MaxValue, 5, "hold");
        #endregion
        #region Физическая величина
        /// <summary>
        /// Физическая величина
        /// </summary>
        [DataMember]
        public Parameter<float> StandPhysValue { get; set; } = new Parameter<float>("StandPhysValue", "Физическая величина", float.MinValue, float.MaxValue, 7, "hold");
        #endregion
        #region Значение стандартизации, скорректрованое по времени
        [DataMember]
        public Parameter<float> HalfLifeCorr { get; set; } = new Parameter<float>("StandHalfLifeCorr", "Значение с учетом полураспада", float.MinValue, float.MaxValue, 0, "");

        #endregion
        #region Standartisation flag
        /// <summary>
        /// Standartisation flag
        /// </summary>
        private bool _isStandartisation;
        /// <summary>
        /// Standartisation flag
        /// </summary>
        public bool IsStandartisation
        {
            get => _isStandartisation;
            set => Set(ref _isStandartisation, value);
        }
        #endregion
        #region ID ЕИ
        private string _measUnitMemoryId;
        public string MeasUnitMemoryId
        {
            get => _measUnitMemoryId;
            set => Set(ref _measUnitMemoryId, value);
        } 
        #endregion
       
    }
}
