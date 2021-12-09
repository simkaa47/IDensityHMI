using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace IDensity.Views.Converters
{
    /// <summary>
    /// Определяет видимость элемента в зависимости от уровня доступа
    /// </summary>
    class UserLevelConverter : IValueConverter
    {        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            var user = value as User;
            string param = parameter as string;
            if(user==null || param==null)return Visibility.Collapsed;
            int GetIndexOfName(string name)
            {
                switch (name)
                {
                    case "Оператор": return 1;
                    case "Сервис": return 2;
                    case "Администратор": return 3;
                    default:return 0;
                        
                }
            }
            if(GetIndexOfName(user.Level)>=GetIndexOfName(param))return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
