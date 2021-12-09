using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace IDensity.Views.Converters
{
    class GetStatusConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
               
                var arrStr = parameter as ArrayList;
                if (arrStr == null) return "КОЛЛЕКЦИЯ ПУСТА";
                var condition = (bool)value;
             return condition ? arrStr[1] : arrStr[0];
            }
            catch (Exception ex)
            {

                return "ОШИБКА ПРЕОБРАЗОВАНИЯ В СТРОКУ";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
