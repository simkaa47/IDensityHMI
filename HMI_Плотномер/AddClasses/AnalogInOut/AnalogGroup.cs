using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    public class AnalogGroup: PropertyChangedBase
    {        
        public int Id { get; }
        public AnalogInput AI { get; }
        public AnalogOutput AO { get; }
        public AnalogGroup(int groupNum)
        {
            Id = groupNum;
            AI = new AnalogInput(groupNum);
            AO = new AnalogOutput(groupNum);
        }
    }
}
