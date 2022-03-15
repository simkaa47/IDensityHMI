using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    public static class FloatToStringExtention
    {
        public static string ToStringPoint(this float num)
        {
            return num.ToString().Replace(",", ".");
        }
    }
}
