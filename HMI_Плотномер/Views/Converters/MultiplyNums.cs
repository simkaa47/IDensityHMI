using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace IDensity.Views.Converters
{
    class MultiplyNums : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
           if (values.Length < 2) return "0";
            float temp = 0;
            var nums = values.Where(o => float.TryParse(o.ToString(), out temp)).Select(v => temp).ToArray();
            if (nums.Length == 2) return (nums[0] * nums[1]*0.1).ToString();
            return "0";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
