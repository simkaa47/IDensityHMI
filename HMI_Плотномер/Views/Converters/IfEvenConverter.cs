using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IDensity.Core.Views.Converters
{
    class IfEvenConverter:Converter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int number = 0;
            if (value is null) return false;
            if (!int.TryParse(value.ToString(), out number)) return false;
            return number%2 == 0;
        }
    }
}
