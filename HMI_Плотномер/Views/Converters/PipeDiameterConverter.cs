using IDensity.Core.Models.Users;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    class PipeDiameterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return Visibility.Collapsed;
            if (!(values[0] is float diam))return Visibility.Collapsed;
            if (!(values[1] is User user)) return Visibility.Collapsed;
            var visibility = (user.Level != "Администратор" && diam == 0) ? Visibility.Collapsed : Visibility.Visible;
            return visibility;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
