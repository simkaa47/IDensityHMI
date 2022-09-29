using IDensity.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    public class GetMeasNumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return null;
            if(!(values[0] is IEnumerable<MeasUnit> measUnits)) return null;
            if (!(values[1] is int mode)) return null;
            return measUnits.Where(mu=>mu.Mode==mode).ToList();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
