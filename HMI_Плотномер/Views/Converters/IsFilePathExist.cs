using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    class IsFilePathExist : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var path = value as string;
                if (path == null) return false;
                return File.Exists(path);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
