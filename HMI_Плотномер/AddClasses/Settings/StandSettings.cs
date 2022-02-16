using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses.Settings
{
    class StandSettings:PropertyChangedBase
    {
        public StandSettings(int id)
        {
            this.Id = id;
        }       
        #region Id
        private int _id;
        /// <summary>
        /// Идентификатор стандартизации
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }


        #endregion
        #region Номер единицы измерения
        /// <summary>
        /// Номер единицы измерения
        /// </summary>
        public Parameter<ushort> StandMeasUnitNum { get; } = new Parameter<ushort>("StandMeasUnitNum", "Номер единицы измерения", 0, 5, 0, "hold");
        #endregion
        #region Длительность стандартизации
        /// <summary>
        /// Длительность стандартизации
        /// </summary>
        public Parameter<ushort> StandDuration { get; } = new Parameter<ushort>("StandDuration", "Длительность стандартизации, x 0.1 c.", 0, 5, 1, "hold");
        #endregion
        #region Дата последней стандартизации
        /// <summary>
        /// Дата последней стандартизации
        /// </summary>
        public Parameter<DateTime> LastStandDate { get; } = new Parameter<DateTime>("LastStandDate", "Дата последней стандартизации", DateTime.MinValue, DateTime.MaxValue, 2, "hold");
        #endregion
        #region Результат
        /// <summary>
        /// Результат
        /// </summary>
        public Parameter<float> StandResult { get; } = new Parameter<float>("StandResult", "Результат", float.MinValue, float.MaxValue, 5, "hold");
        #endregion
        #region Физическая величина
        /// <summary>
        /// Физическая величина
        /// </summary>
        public Parameter<float> StandPhysValue { get; } = new Parameter<float>("StandPhysValue", "Физическая величина", float.MinValue, float.MaxValue, 7, "hold");
        #endregion
    }
}
