using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses.Settings
{
    class Compensation :PropertyChangedBase
    {
        #region Активность компенсации
        /// <summary>
        /// Активность компенсации
        /// </summary>
        public Parameter<bool> Activity => new Parameter<bool>("CompensationActivity", "Активность", false, true, 0, "");
        #endregion
        #region  Номер ЕИ
        /// <summary>
        /// Номер ЕИ
        /// </summary>
        public Parameter<ushort> MeasUnitNum => new Parameter<ushort>("CompensationMeasUnitNum", "Тип ЕИ", 0, ushort.MaxValue, 0, "");
        #endregion
        #region Источник компенсации
        /// <summary>
        /// Источник компенсации
        /// </summary>
        public Parameter<ushort> Sourse { get; } = new Parameter<ushort>("CompensationSource", "Источник", 0, ushort.MaxValue, 0, ""); 
        #endregion
        #region Коэффициент А
        /// <summary>
        /// Коэффициент А
        /// </summary>
        public Parameter<float> A { get; } = new Parameter<float>("CompensationA", "Коэффициент А", float.MinValue, float.MaxValue, 0, "");
        #endregion
        #region Коэффициент B
        /// <summary>
        /// Коэффициент B
        /// </summary>
        public Parameter<float> B { get; } = new Parameter<float>("CompensationB", "Коэффициент B", float.MinValue, float.MaxValue, 0, "");
        #endregion
        #region Коэффициент C
        /// <summary>
        /// Коэффициент C
        /// </summary>
        public Parameter<float> C { get; } = new Parameter<float>("CompensationC", "Коэффициент C", float.MinValue, float.MaxValue, 0, "");
        #endregion

    }
}
