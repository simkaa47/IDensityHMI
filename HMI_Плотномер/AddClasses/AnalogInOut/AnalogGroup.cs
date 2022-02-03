using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    class AnalogGroup: PropertyChangedBase
    {
        public AnalogInput AI { get; }
        public AnalogOutput AO { get; }
        public AnalogGroup(int groupNum)
        {
            AI = new AnalogInput(groupNum);
            AO = new AnalogOutput(groupNum);
        }
    }
}
