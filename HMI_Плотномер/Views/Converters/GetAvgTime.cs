using IDensity.AddClasses;
using IDensity.AddClasses.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace IDensity.Views.Converters
{
    /// <summary>
    /// Позволяет получить текущее время усреднения, умножая длительность на количество точек измерения
    /// </summary>
    class GetAvgTime : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var process = value as MeasProcSettings;
            if (process != null) return process.MeasDeep.Value * 0.1 * process.MeasDuration.Value;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
