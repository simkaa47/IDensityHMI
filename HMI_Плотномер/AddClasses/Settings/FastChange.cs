using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses.Settings
{
    /// <summary>
    /// Данные отслеживания быстрых изменений
    /// </summary>
    class FastChange:PropertyChangedBase
    {
        public FastChange()
        {
            Activity.CommandEcecutedEvent += (o) => OnWriteExecuted();
            Threshold.CommandEcecutedEvent += (o) => OnWriteExecuted();
        }
        #region Активность
        /// <summary>
        /// Активность
        /// </summary>
        public Parameter<bool> Activity { get; } = new Parameter<bool>("FastChangeActivity", "Активность отслеживания", false, true, 0, "");
        #endregion
        #region Порог реакции
        /// <summary>
        /// Порог реакции
        /// </summary>
        public Parameter<ushort> Threshold { get; } = new Parameter<ushort>("FastChangeThreshold", "Порог реакции", 0, ushort.MaxValue, 0, "");
        #endregion
        void OnWriteExecuted()
        {
            NeedWriteEvent?.Invoke($"fast_chg={(Activity.WriteValue ? 1 : 0)},{Threshold.WriteValue.ToString()}");
        }
        /// <summary>
        /// Необходимо записать настройки стандартизаций
        /// </summary>
        public event Action<string> NeedWriteEvent;

        public string Copy()
        {
            return $"fast_chg={(Activity.Value ? 1 : 0)},{Threshold.Value.ToString()}";
        }
    }
}
