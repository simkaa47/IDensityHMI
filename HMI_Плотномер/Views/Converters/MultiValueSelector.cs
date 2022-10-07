using System;
using System.Globalization;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    public class MultiValueSelector : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3) return null;
            int selector = 0;
            if (!(int.TryParse(values[0].ToString(), out selector))) return null;            
            if (selector  > values.Length) return null;
            return values[selector+1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
