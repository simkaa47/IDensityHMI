using System;
using System.Globalization;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    public class GetBitFromNum : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
        {
            if (p is null) return null;
            var index = System.Convert.ToUInt32(p, c);
            var num = System.Convert.ToUInt32(v, c);
            var bit = (num & (uint)Math.Pow(2, index)) > 0 ? true : false;
            return bit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
