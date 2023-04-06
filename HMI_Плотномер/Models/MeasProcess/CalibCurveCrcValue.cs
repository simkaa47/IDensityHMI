using IDensity.AddClasses;
using IDensity.Core.Models.Parameters;
using IDensity.DataAccess;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IDensity.Core.Models.MeasProcess
{
    /// <summary>
    /// Данные, из которых получена калибровочная кривая
    /// </summary>
    [DataContract]
    public class CalibCurveCrcValue : PropertyChangedBase
    {
        public CalibCurveCrcValue(int id)
        {
            Id = id;
            MeasUnitMemoryId = $"SingleSrcMeasUnitMemory{id}";
        }
        #region Id
        private int _id;
        [DataMember]
        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }
        #endregion
        #region Дата 
        /// <summary>
        /// Дата измерения
        /// </summary>
        [DataMember]
        public Parameter<DateTime> Date { get; set; } = new Parameter<DateTime>("CalibCurveCrcDate", "Дата измерения", DateTime.MinValue, DateTime.MaxValue, 0, "");
        #endregion
        #region Ослабление
        /// <summary>
        /// Ослабление
        /// </summary>
        [DataMember]
        public Parameter<float> Weak { get; set; } = new Parameter<float>("CalibCurveCrcWeak", "Ослабление", float.MinValue, float.MaxValue, 0, "");
        #endregion
        #region Значение счетчика
        /// <summary>
        /// Значение счетчика
        /// </summary>
        [DataMember]
        public Parameter<float> CounterValue { get; set; } = new Parameter<float>("CalibCurveCrcValue", "Значение счетчика", 0, float.MaxValue, 0, "");
        #endregion        
        #region Флаг участия в расчете к-тов
        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
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
