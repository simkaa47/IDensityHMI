using IDensity.AddClasses;
using IDensity.Core.Extentions;
using IDensity.Core.Models.Parameters;
using IDensity.DataAccess;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IDensity.Core.Models.MeasProcess
{
    [DataContract]
    public class Compensation : PropertyChangedBase
    {

        #region Активность компенсации
        /// <summary>
        /// Активность компенсации
        /// </summary>
        [DataMember]
        public Parameter<bool> Activity { get; set; } = new Parameter<bool>("CompensationActivity", "Активность", false, true, 0, "");
        #endregion
        #region  Номер ЕИ
        /// <summary>
        /// Номер ЕИ
        /// </summary>
        [DataMember]
        public Parameter<ushort> MeasUnitNum { get; set; } = new Parameter<ushort>("CompensationMeasUnitNum", "Тип ЕИ", 0, ushort.MaxValue, 0, "");
        #endregion
        #region Источник компенсации
        /// <summary>
        /// Источник компенсации
        /// </summary>
        [DataMember]
        public Parameter<ushort> Sourse { get; set; } = new Parameter<ushort>("CompensationSource", "Источник", 0, ushort.MaxValue, 0, "");
        #endregion
        #region Коэффициент А
        /// <summary>
        /// Коэффициент А
        /// </summary>
        [DataMember]
        public Parameter<float> A { get; set; } = new Parameter<float>("CompensationA", "Коэффициент А", float.MinValue, float.MaxValue, 0, "");
        #endregion
        #region Коэффициент B
        /// <summary>
        /// Коэффициент B
        /// </summary>
        [DataMember]
        public Parameter<float> B { get; set; } = new Parameter<float>("CompensationB", "Коэффициент B", float.MinValue, float.MaxValue, 0, "");
        #endregion
        #region Коэффициент C
        /// <summary>
        /// Коэффициент C
        /// </summary>
        [DataMember]
        public Parameter<float> C { get; set; } = new Parameter<float>("CompensationC", "Коэффициент C", float.MinValue, float.MaxValue, 0, "");
        #endregion
        #region Коэффициент D
        /// <summary>
        /// Коэффициент D
        /// </summary>
        [DataMember]
        public Parameter<float> D { get; set; } = new Parameter<float>("CompensationD", "Коэффициент D", float.MinValue, float.MaxValue, 0, "");
        #endregion

        public string Copy()
        {
            return $"{(Activity.Value ? 1 : 0)},{MeasUnitNum.Value},{Sourse.Value},{A.Value.ToStringPoint()},{B.Value.ToStringPoint()}";
        }
    }
}
