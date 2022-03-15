using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace IDensity.Views.Converters
{
    class StringByIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return "НЕДОСТАТОЧНО ПАРАМЕТРОВ";
            var strings = values[1] as IEnumerable<string>;
            if (strings == null) return "ПАРАМЕТР Collection НЕ ЯВЛЯЕТСЯ ОБЬЕКТОМ  IEnumerable<string>";
            var strArr = strings.ToArray();
            var index = 0;
            if (values[0] != null && int.TryParse(values[0].ToString(), out index) && index< strArr.Length && index>=0)
            {
                return strArr[index];
            }
            return strArr[strArr.Length - 1];            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
