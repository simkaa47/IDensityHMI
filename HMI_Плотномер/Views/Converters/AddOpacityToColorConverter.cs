using System;
using System.Globalization;
using System.Windows.Media;

namespace IDensity.Core.Views.Converters
{
    internal class AddOpacityToColorConverter: Converter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SolidColorBrush brush)) return null;
            var copy = brush.Color;
            copy.A = 100;
            return new SolidColorBrush(copy);
        }
    }
}
