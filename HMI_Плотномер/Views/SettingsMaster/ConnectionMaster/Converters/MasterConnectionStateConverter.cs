using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace IDensity.Views.SettingsMaster.ConnectionMaster.Converters
{
    class GetStatusConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if(parameter==null || value == null) return Visibility.Collapsed;
                var state = (StateConnectionMaster)value;
                var comparedState = (StateConnectionMaster)parameter;
                return state == comparedState ? Visibility.Visible : Visibility.Collapsed; 
            }
            catch (Exception ex)
            {

                return Visibility.Collapsed;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
