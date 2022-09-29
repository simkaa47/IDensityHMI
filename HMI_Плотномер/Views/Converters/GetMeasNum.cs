using IDensity.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    internal class GetMeasNum : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3) return null;
            if (!(values[0] is IEnumerable<MeasUnit> measUnits)) return null;
            if (!(values[1] is int mode)) return null;

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
