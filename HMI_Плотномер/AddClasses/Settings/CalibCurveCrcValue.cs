using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses.Settings
{
    /// <summary>
    /// Данные, из которых получена калибровочная кривая
    /// </summary>
    class CalibCurveCrcValue:PropertyChangedBase
    { 
        #region Дата 
        /// <summary>
        /// Дата измерения
        /// </summary>
        public Parameter<DateTime> Date { get; } = new Parameter<DateTime>("CalibCurveCrcDate", "Дата измерения", DateTime.MinValue, DateTime.MaxValue, 0, "");
        #endregion
        #region Ослабление
        /// <summary>
        /// Ослабление
        /// </summary>
        public Parameter<float> Weak { get; } = new Parameter<float>("CalibCurveCrcWeak", "Ослабление", float.MinValue, float.MaxValue, 0, "");
        #endregion
        #region Значение счетчика
        /// <summary>
        /// Значение счетчика
        /// </summary>
        public Parameter<float> CounterValue { get; } = new Parameter<float>("CalibCurveCrcValue", "Значение счетчика", 0, float.MaxValue, 0, ""); 
        #endregion
    }
}
