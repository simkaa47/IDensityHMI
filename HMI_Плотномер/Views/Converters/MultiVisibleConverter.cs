using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    public class MultiVisibleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return null;
            var visibilities = values
                .Where(v => v is Visibility vis)
                .Select(v => (Visibility)v).ToList();
            if (visibilities.Count != values.Length) return Visibility.Collapsed;
            if (visibilities.All(v => v == Visibility.Visible)) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
