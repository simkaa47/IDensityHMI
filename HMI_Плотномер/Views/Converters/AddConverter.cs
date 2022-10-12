using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    class AddConverter : IMultiValueConverter
    {
        public object Convert(object[] v, Type t, object p, CultureInfo c)
        {
            var nums = new float[v.Length];            
            for (int i = 0; i < v.Length; i++)
            {
                if (!float.TryParse(v[i].ToString(), out nums[i])) return 0;
            }
            var sum = nums.Sum();
            return sum;
        }

        public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c)
        {
            throw new NotImplementedException();
        }
    }
}
