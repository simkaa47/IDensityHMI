using System;
using System.Collections;
using System.Globalization;

namespace IDensity.Core.Views.Converters
{
    class SelectStringByBoolConverter : Converter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is ArrayList list)) return null;
            if (list.Count < 2) return null;
            if (!(value is bool condition)) return null;
            var i = condition ? 1 : 0;
            return list[i];
        }
    }
}
