using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace IDensity.Core.Views.Converters
{
    class GetByIndexFromParameterConverter:Converter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = 0;
            if (!int.TryParse(value.ToString(), out index)) return null;

            if (!(parameter is IEnumerable<object> list)) return null;
            if (list.Count() < index + 1) return null;
            return list.ElementAt(index);
        }
    }
}
