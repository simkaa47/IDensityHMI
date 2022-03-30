using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses.Settings
{
    class Compensation :PropertyChangedBase
    {
        public Compensation()
        {
            Activity.CommandEcecutedEvent += (o) => OnWriteExecuted();
            MeasUnitNum.CommandEcecutedEvent += (o) => OnWriteExecuted();
            Sourse.CommandEcecutedEvent += (o) => OnWriteExecuted();
            A.CommandEcecutedEvent += (o) => OnWriteExecuted();
            B.CommandEcecutedEvent += (o) => OnWriteExecuted();
            C.CommandEcecutedEvent += (o) => OnWriteExecuted();
            D.CommandEcecutedEvent += (o) => OnWriteExecuted();
        }
        void OnWriteExecuted()
        {
            NeedWriteEvent?.Invoke($"{(Activity.WriteValue ? 1:0)},{MeasUnitNum.WriteValue},{Sourse.WriteValue},{A.WriteValue.ToStringPoint()},{B.WriteValue.ToStringPoint()},{C.WriteValue.ToStringPoint()},{D.WriteValue.ToStringPoint()}");
        }
        #region Активность компенсации
        /// <summary>
        /// Активность компенсации
        /// </summary>
        public Parameter<bool> Activity { get; } = new Parameter<bool>("CompensationActivity", "Активность", false, true, 0, "");
        #endregion
        #region  Номер ЕИ
        /// <summary>
        /// Номер ЕИ
        /// </summary>
        public Parameter<ushort> MeasUnitNum { get; } = new Parameter<ushort>("CompensationMeasUnitNum", "Тип ЕИ", 0, ushort.MaxValue, 0, "");
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

        #region Коэффициент D
        /// <summary>
        /// Коэффициент D
        /// </summary>
        public Parameter<float> D { get; } = new Parameter<float>("CompensationD", "Коэффициент D", float.MinValue, float.MaxValue, 0, "");
        #endregion

        /// <summary>
        /// Необходимо записать настройки стандартизаций
        /// </summary>
        public event Action<string> NeedWriteEvent;
    }
}
