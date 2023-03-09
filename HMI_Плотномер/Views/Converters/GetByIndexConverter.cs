using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace IDensity.Core.Views.Converters
{
    class GetByIndexConverter : Converter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = 0;
            if (parameter is null) return null;
            if (!int.TryParse(parameter.ToString(), out index)) return null;

            if (!(value is IEnumerable<object> list)) return null;
            if (list.Count() < index+1) return null;            
            return list.ElementAt(index);
        }
    }
}
