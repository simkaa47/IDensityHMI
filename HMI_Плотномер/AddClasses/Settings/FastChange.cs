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
        #region Активность
        /// <summary>
        /// Активность
        /// </summary>
        public Parameter<bool> Activity => new Parameter<bool>("FastChangeActivity", "Активность отслеживания", false, true, 0, "");
        #endregion
        #region Порог реакции
        /// <summary>
        /// Порог реакции
        /// </summary>
        public Parameter<ushort> Threshold => new Parameter<ushort>("FastChangeThreshold", "Порог реакции", 0, ushort.MaxValue, 0, ""); 
        #endregion
    }
}
