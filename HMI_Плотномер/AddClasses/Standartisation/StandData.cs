using IDensity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDensity.AddClasses.Standartisation
{
    class StandData : ICloneable
    {
        #region Номер регистра, отвечающего за команду стандартизации
        public static readonly int StandCommandRegNum = 15;
        #endregion
        #region Номер набора стандартизации
        public static Parameter<ushort> NumSelection { get; } = new Parameter<ushort>("NumStdSelection", "Номер набора стандартизации", 0, (ushort)MainModel.CountStand, 14, "hold");
        #endregion
        #region Длительность стандартизации
        /// <summary>
        /// Длительность стандартизации
        /// </summary>
        public Parameter<ushort> Duration { get; set; } = new Parameter<ushort>("StandDuration", "Длительность стандартизации, x0.1 c", 0, ushort.MaxValue, 68, "hold");
        #endregion
        #region Тип
        /// <summary>
        /// Тип
        /// </summary>
        public Parameter<ushort> Type { get; set; } = new Parameter<ushort>("StandType", "Тип", 0, ushort.MaxValue, 69, "hold");
        #endregion
        #region Физическая величина
        public Parameter<float> Value { get; set; } = new Parameter<float>("StandValue", "Физическая величина", float.NegativeInfinity, float.PositiveInfinity, 70, "hold");
        #endregion
        #region Результаты
        public Parameter<float>[] Results { get; set; } = Enumerable.Range(0, 8).Select(i => new Parameter<float>("StandResult" + i.ToString(), "Значение " + i.ToString(), float.NegativeInfinity, float.PositiveInfinity, 72, "hold")).ToArray();
        #endregion
        #region Дата последней стандартизации
        public Parameter<DateTime> Date { get; } = new Parameter<DateTime>("LastStandDate", "Время последней стандартизации", DateTime.MinValue, DateTime.MaxValue, 88, "hold")
        {
            OnlyRead = true
        };

        public object Clone()
        {
            return new StandData
            {
                Duration = this.Duration.Clone() as Parameter<ushort>,
                Type = this.Type.Clone() as Parameter<ushort>,
                Value = this.Value.Clone() as Parameter<float>,
                Results = this.Results.Select(r=>r.Clone() as Parameter<float>).ToArray()
            };
        }
        #endregion
    }
}
