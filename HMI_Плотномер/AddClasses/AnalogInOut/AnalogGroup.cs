using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IDensity.AddClasses
{
    [DataContract]
    public class AnalogGroup: PropertyChangedBase
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
