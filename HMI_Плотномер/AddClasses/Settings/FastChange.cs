using System;
using System.Runtime.Serialization;

namespace IDensity.AddClasses.Settings
{
    /// <summary>
    /// Данные отслеживания быстрых изменений
    /// </summary>
    [DataContract]
    public class FastChange : PropertyChangedBase
    {
        public FastChange()
        {
            
        }
        #region Активность
        /// <summary>
        /// Активность
        /// </summary>
        [DataMember]
        public Parameter<bool> Activity { get; set; } = new Parameter<bool>("FastChangeActivity", "Активность отслеживания", false, true, 0, "");
        #endregion
        #region Порог реакции
        /// <summary>
        /// Порог реакции
        /// </summary>
        [DataMember]
        public Parameter<ushort> Threshold { get; set; } = new Parameter<ushort>("FastChangeThreshold", "Порог реакции", 0, ushort.MaxValue, 0, "");
        #endregion       

    }
}
