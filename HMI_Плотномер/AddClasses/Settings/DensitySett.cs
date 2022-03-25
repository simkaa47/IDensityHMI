using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses.Settings
{
    /// <summary>
    /// Определяет настройки плотности
    /// </summary>
    class DensitySett:PropertyChangedBase
    {
        #region Номер еденицы измерения
        /// <summary>
        /// Номер еденицы измерения
        /// </summary>
        public Parameter<ushort> MeasValueNum { get; } = new Parameter<ushort>("DensitySettMeasUnitNum", "Номер еденицы измерений", 0, 10, 0, "hold");
        #endregion
        #region Физическая величина
        /// <summary>
        /// Физическая величина
        /// </summary>
        public Parameter<float> PhysValue { get; } = new Parameter<float>("DensitySettValue", "Физическая величина", float.MinValue, float.MaxValue, 0, "");
        #endregion

        public DensitySett()
        {
            //Подписка на изменение свойств
            MeasValueNum.CommandEcecutedEvent +=(o)=> OnWriteExecuted();
            PhysValue.CommandEcecutedEvent += (o) => OnWriteExecuted();
        }
        void OnWriteExecuted()
        {
            NeedWriteEvent?.Invoke($"{MeasValueNum.WriteValue},{PhysValue.WriteValue.ToStringPoint()}");
        }
        /// <summary>
        /// Необходимо записать настройки стандартизаций
        /// </summary>
        public event Action<string> NeedWriteEvent;
    }
}
