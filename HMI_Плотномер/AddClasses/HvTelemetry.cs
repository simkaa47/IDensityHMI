using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IDensity.AddClasses
{
    [DataContract]
    public class HvTelemetry
    {
        #region Входное напряжение, вольт
        #region Уставка напряжения
        [DataMember]
        public Parameter<ushort> VoltageSV { get; set; } = new Parameter<ushort>("VoltageSvHv", "Уставка напряжения, вольт", 400, 2000, 67, "hold");        
        #endregion 
        #endregion
        #region Входное напряжение, вольт
        public Parameter<float> VoltageCurIn { get; } = new Parameter<float>("VoltageCurIn", "Входное напряжение, вольт", 0, float.PositiveInfinity, 5, "read")
        {           
            OnlyRead = true           
        };
        #endregion
        #region Выходное напряжение, вольт
        public Parameter<ushort> VoltageCurOut { get; } = new Parameter<ushort>("VoltageCurOut", "Выходное напряжение, вольт", 0, ushort.MaxValue, 6, "read")
        {           
            OnlyRead = true
        };
        #endregion
        #region Ток, ампер
        public Parameter<ushort> Current { get; } = new Parameter<ushort>("Current", "Ток, ампер", 0, ushort.MaxValue, 7, "read")
        {            
            OnlyRead = true,           
        };
        #endregion
        #region Статус высокого напряжения
        public Parameter<bool> HvOn { get; } = new Parameter<bool>("HvOn", "Статус высокого напряжения", false, true, 0, "");
        #endregion
        #region Статус связи с платой HV
        public Parameter<bool> HvCommState { get; } = new Parameter<bool>("HvCommState", "Статус связи с платой HV", false, true, 0, "");
        #endregion
    }
}
