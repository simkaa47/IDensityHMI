using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace IDensity.Views.Converters
{
    class GetNameFromEnumCustom : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            /*
             values[0] - индекс
             values[1] - сама коллекция
            */
            if (values.Length < 2) return "НЕДОСТАТОЧНО ПАРАМЕТРОВ";
            var enumCustoms = values[1] as IEnumerable<EnumCustom>;
            if (enumCustoms == null) return "ПАРАМЕТР Collection НЕ ЯВЛЯЕТСЯ ОБЬЕКТОМ  IEnumerable<EnumCustom>";
            var names = enumCustoms.Select(e => e.Name).ToArray();
            var index = 0;
            if (values[0] != null && int.TryParse(values[0].ToString(), out index) && index < names.Length && index >= 0)
            {
                return names[index];
            }
            return names[names.Length - 1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
