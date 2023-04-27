using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    class MultiplyNums : IMultiValueConverter
    {
        public float K { get; set; }
        public float Duration { get; set; }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {            
            if (values.Length < 2) return "0";
            float temp = 0;
            var nums = values.Where(o => float.TryParse(o.ToString(), out temp)).Select(v => temp).ToArray();
            if (nums.Length >= 3)
            {
                var k = nums.Aggregate((a, b) => a * b);
                K = nums[0];
                Duration = nums[2];
                return Math.Round(k, 2).ToString();
            }
            return "0";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is null) return new object[] {K};
            float num = 0;
            if(!float.TryParse(value.ToString(), out num)) return new object[] { K};
            var arr =  new object[] { K, (ushort)(num/Duration/K), Duration};
            return arr;
        }
    }
}
