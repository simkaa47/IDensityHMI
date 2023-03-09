using IDensity.AddClasses.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    class GetFromMeasTypeMultyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return null;
            int selector = 0;
            if (!int.TryParse(values[0].ToString(), out selector)) return null;
            if (!(values[1] is IEnumerable<MeasProcSettings> collection)) return null;
            if (selector >= collection.Count()) return null;
            return  collection.ElementAt(selector).MeasType.Value;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
