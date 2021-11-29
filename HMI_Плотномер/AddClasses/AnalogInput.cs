using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    class AnalogInput:AnalogData
    {
        public AnalogInput(int groupNum) : base(groupNum) 
        {
            ModulNum = 1;
        }
    }
}
