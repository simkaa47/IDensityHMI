using System;
using System.Collections;
using System.Globalization;

namespace IDensity.Core.Views.Converters
{
    class GetByIndexConverter : Converter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable) return null;
            return null;
        }
    }
}
