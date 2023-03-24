using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    /// <summary>
    /// Allow a binding where the StringFormat is also bound to a property (and can vary).
    /// </summary>
    public class ToStringFormatConverter : IMultiValueConverter
    {
        float K  = 1;
        float Offset  = 0;
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {            
            float value = 0;
            if (values.Length == 0 || !(values[0] is IConvertible)) return null;
            if (values[0] is string) return values[0].ToString();
            if(!float.TryParse(values[0].ToString(), out value))return values[0].ToString();
            if (values.Length==1 || !(values[1] is string format)) return values[0].ToString();
            if(values.Length>2 && values[2] !=null)
            {
                float.TryParse(values[2].ToString(), out K);
            }
            if (values.Length > 3 && values[3] != null)
            {
                float.TryParse(values[3].ToString(), out Offset);
            }
            return (value * K + Offset).ToString(format);
            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            float temp = 0;
            if (!(value is IConvertible)) return new object[] {value };
            if (value is string) return new object[] { value };
            if (!float.TryParse(value.ToString(), out temp)) return new object[] { value.ToString() };
            return new object[] { (temp - Offset) / K };
        }

        // Some types have Parse methods that are more successful than their type converters at converting strings
        private static object TryParse(object value, Type targetType, CultureInfo culture)
        {
            object result = DependencyProperty.UnsetValue;
            string stringValue = value as string;

            if (stringValue != null)
            {
                try
                {
                    MethodInfo mi;
                    if (culture != null
                        && (mi = targetType.GetMethod("Parse",
                            BindingFlags.Public | BindingFlags.Static, null,
                            new[] { typeof(string), typeof(NumberStyles), typeof(IFormatProvider) }, null))
                        != null)
                    {
                        result = mi.Invoke(null, new object[] { stringValue, NumberStyles.Any, culture });
                    }
                    else if (culture != null
                        && (mi = targetType.GetMethod("Parse",
                            BindingFlags.Public | BindingFlags.Static, null,
                            new[] { typeof(string), typeof(IFormatProvider) }, null))
                        != null)
                    {
                        result = mi.Invoke(null, new object[] { stringValue, culture });
                    }
                    else if ((mi = targetType.GetMethod("Parse",
                            BindingFlags.Public | BindingFlags.Static, null,
                            new[] { typeof(string) }, null))
                        != null)
                    {
                        result = mi.Invoke(null, new object[] { stringValue });
                    }
                }
                catch (TargetInvocationException)
                {
                }
            }

            return result;
        }
    }
}
