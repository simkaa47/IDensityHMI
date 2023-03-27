using System;
using System.Globalization;

namespace IDensity.Core.Views.Converters
{
    class MultiplyOnNumberConverter: Converter
    {
        public float Offset { get; set; }
        public MultiplyOnNumberConverter()
        {
           
        }
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float temp = 0;
            if (value is null) return 0;
            if(!float.TryParse(value.ToString(), out temp)) return 0;
            float x = temp;
            if (parameter is null) return 0;
            if (!float.TryParse(parameter.ToString(), out temp)) return 0;
            float k = temp;
            return k * x + Offset;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float temp = 0;
            if (value is null) return 0;
            if (float.TryParse(value.ToString(), out temp)) return 0;
            float x = temp;
            if (parameter is null) return 0;
            if (float.TryParse(parameter.ToString(), out temp)) return 0;
            float k = temp;
            return (x - Offset) / k;
        }
    }
}
