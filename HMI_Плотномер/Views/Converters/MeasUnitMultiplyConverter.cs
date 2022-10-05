using IDensity.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    [ValueConversion(typeof(object[]),typeof(int))]
    class MeasUnitMultiplyConverter : IMultiValueConverter
    {
        public MeasUnit CurMeasUnit { get; set; }
        public object Convert(object[] v, Type t, object p, CultureInfo c)
        {
            
            double k = 1;
            if (v.Length < 2) return 0;
            if (!(v[0] is float result)) return 0;
            if (v[1] is MeasUnit unit)
            {
                CurMeasUnit = unit;
                k = unit.K;
            }
            double y = k * result;
            return y.ToString("f3");

        }

        public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c)
        {
            var k = CurMeasUnit is null ? 1 : CurMeasUnit.K;
            float value = 0;
            if (!(float.TryParse(v.ToString(), out value ))) value = 0;
            return new object[] { value / k, CurMeasUnit };
        }
    }
}
