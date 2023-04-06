using IDensity.Core.Models.Parameters;
using System.Runtime.Serialization;

namespace IDensity.Core.Models.Analogs
{
    [DataContract]
    public class AnalogInput : AnalogData
    {
        public AnalogInput(int groupNum) : base(groupNum)
        {
            ModulNum = 1;
        }

        #region активность АЦП
        [DataMember]
        public Parameter<ushort> Activity { get; set; } = new Parameter<ushort>("AdcActivity", "Активность аналогового входа, вкл./выкл.", 0, 1, 0, "");
        #endregion

    }
}
