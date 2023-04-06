using IDensity.DataAccess;
using System.Runtime.Serialization;

namespace IDensity.Core.Models.Analogs
{
    [DataContract]
    public class AnalogGroup : PropertyChangedBase
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public AnalogInput AI { get; set; }
        [DataMember]
        public AnalogOutput AO { get; set; }
        public AnalogGroup(int groupNum)
        {
            Id = groupNum;
            AI = new AnalogInput(groupNum);
            AO = new AnalogOutput(groupNum);
        }
    }
}
