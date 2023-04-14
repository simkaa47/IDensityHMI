using IDensity.AddClasses;
using IDensity.Core.Models.Parameters;
using IDensity.DataAccess;
using System;
using System.Runtime.Serialization;

namespace IDensity.Core.Models.MeasProcess
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
        public Parameter<ushort> Threshold { get; set; } = new Parameter<ushort>("FastChangeThreshold", "Порог реакции быстрых изменений, %", 0, 100, 0, "");
        #endregion       

    }
}
