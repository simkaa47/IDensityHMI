using System;
using System.Globalization;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    public class InterpolationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return OxyPlot.InterpolationAlgorithms.CanonicalSpline;
            else
            {
                int index = 0;
                if (int.TryParse(value.ToString(), out index))
                {
                    switch (index)
                    {
                        case 0: return OxyPlot.InterpolationAlgorithms.CanonicalSpline;
                        case 1: return OxyPlot.InterpolationAlgorithms.CatmullRomSpline;
                        case 2: return OxyPlot.InterpolationAlgorithms.ChordalCatmullRomSpline;
                        default: return OxyPlot.InterpolationAlgorithms.UniformCatmullRomSpline;
                    }
                }
                return OxyPlot.InterpolationAlgorithms.UniformCatmullRomSpline;
            }


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
