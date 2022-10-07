using IDensity.AddClasses;
using System;
using System.Globalization;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    /// <summary>
    /// Получает из текущего пользователя имя, фамилию, уровень
    /// </summary>
    class UserTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Пользователь не авторизован";
            var user = value as User;
            if (user == null) return "Пользователь не авторизован";
            return user.Somename + " " + user.Name + $"({user.Level})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
