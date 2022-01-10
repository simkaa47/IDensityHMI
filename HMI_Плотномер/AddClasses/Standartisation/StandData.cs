using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDensity.AddClasses.Standartisation
{
    class StandData
    {        
        #region Длительность стандартизации
        /// <summary>
        /// Длительность стандартизации
        /// </summary>
        public Parameter<ushort> Duration { get; } = new Parameter<ushort>("StandDuration", "Длительность стандартизации, x0.1 c", 0, ushort.MaxValue, 68, "hold");
        #endregion
        #region Тип
        /// <summary>
        /// Тип
        /// </summary>
        public Parameter<ushort> Type { get; } = new Parameter<ushort>("StandType", "Тип", 0, ushort.MaxValue, 69, "hold");
        #endregion
        #region Физическая величина
        public Parameter<float> Value { get; } = new Parameter<float>("StandValue", "Физическая величина", float.NegativeInfinity, float.PositiveInfinity, 70, "hold");
        #endregion
        #region Результаты
        public Parameter<float>[] Results { get; } = Enumerable.Range(0, 8).Select(i => new Parameter<float>("StandResult" + i.ToString(), "Значение " + i.ToString(), float.NegativeInfinity, float.PositiveInfinity, 72, "hold")).ToArray();
        #endregion
        #region Дата последней стандартизации
        public Parameter<DateTime> Date { get; } = new Parameter<DateTime>("LastStandDate", "Время последней стандартизации", DateTime.MinValue, DateTime.MaxValue, 88, "hold")
        {
            OnlyRead = true
        };
        #endregion
    }
}
