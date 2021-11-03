using System;
using System.Collections.Generic;
using System.Text;

namespace HMI_Плотномер.AddClasses
{
    internal struct TimePoint
    {
        public DateTime time { get; set; }
        public float y1 { set; get; }
        public float y2 { set; get; }
    }
}
