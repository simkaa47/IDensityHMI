using IDensity.AddClasses;
using IDensity.DataAccess;
using IDensity.Services.XML;
using System;
using System.Linq;

namespace IDensity.Services.InitServices
{
    public static class XmlInit
    {
        public static T ClassInit<T>() where T : PropertyChangedBase
        {
            T cell = XmlMethods.GetParam<T>().FirstOrDefault();
            if (cell == null)
            {
                cell = (T)Activator.CreateInstance(typeof(T));
                XmlMethods.AddToXml<T>(cell);
            }
            cell.PropertyChanged += (sender, e) => XmlMethods.EditParam<T>(cell, e.PropertyName);
            return cell;
        }
    }
}
