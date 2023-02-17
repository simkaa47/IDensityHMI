using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    class GetMeasProcessTypeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null) return String.Empty;
            if (values.Length <4) return String.Empty;
            if(!(values[0] is ushort deviceType)) return String.Empty;
            if (!(values[1] is int measType)) return String.Empty;
            if (!(values[2] is IEnumerable<object> densities)) return String.Empty;
            if (!(values[3] is IEnumerable<object> levels)) return String.Empty;
            var source = deviceType == 0 ? densities : levels;
            if(measType>=source.Count()) return String.Empty;
            return source.ElementAt(measType);

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
