using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    class AddStrings : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 1) return null;
            var sum = values.Aggregate((s1, s2) => s1.ToString() + s2.ToString());
            return sum;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
