using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;

namespace IDensity.Services.XML
{
    /// <summary>
    /// Набор статических методов для чтеня, записи, удаления, редактирования данных в XML документе
    /// </summary>
    class XmlMethods
    {
        #region Событие
        static public event Action<string> XmlErrorEvent = (message) => MessageBox.Show(message);
        #endregion

        #region Путь
        public static string _path = "settings.xml";
        #endregion

        #region Метод чтения данных по дескриптору
        /// <summary>
        /// Чтение данных по дескриптору
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static List<T> GetParam<T>()
        {
            var paramList = new List<T>();
            try
            {
                var xDoc = new XmlDocument();// создаем документ
                xDoc.Load(_path);// загружаем данные
                Type type = typeof(T);
                var props = typeof(T).GetProperties();

                XmlNodeList nodeList = xDoc.DocumentElement.GetElementsByTagName(type.Name);
                for (int i = 0; i < nodeList.Count; i++)
                {
                    T cell = (T)Activator.CreateInstance(type);
                    for (int j = 0; j < props.Length; j++)
                    {
                        type.GetProperty(props[j].Name).SetValue(cell, Parse(props[j].PropertyType, nodeList.Item(i).Attributes[props[j].Name].Value));
                    }
                    paramList.Add(cell);
                }
            }
            catch (Exception ex)
            {
                XmlErrorEvent?.Invoke(ex.Message);
            }
            return paramList;

        }
        #endregion

        #region Преобразование из строки в зависимости от типа
        static object Parse(Type type, string par)
        {
            switch (type.Name.ToLower())
            {
                case "boolean": return par.ToLower() == "true";
                case "uint32": return uint.Parse(par);
                case "int32": return int.Parse(par);
                case "byte": return byte.Parse(par);
                case "single":
                case "float": return float.Parse(par.Replace(",", "."), CultureInfo.InvariantCulture);
                case "double": return float.Parse(par.Replace(",", "."), CultureInfo.InvariantCulture);
                default: return par;
            }
        }
        #endregion

        #region Добавление записи
        public static void AddToXml<T>(T param)
        {
            try
            {
                var xDoc = new XmlDocument();// создаем документ
                xDoc.Load(_path);// загружаем данные
                XmlNode xNode = xDoc.DocumentElement.SelectSingleNode("params");
                Type type = typeof(T);
                XmlElement deviceElem = xDoc.CreateElement(type.Name);
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    XmlAttribute attr = xDoc.CreateAttribute(prop.Name);
                    XmlText text = xDoc.CreateTextNode(type.GetProperty(prop.Name).GetValue(param).ToString());
                    attr.AppendChild(text);
                    deviceElem.Attributes.Append(attr);

                }
                xNode.AppendChild(deviceElem);
                xDoc.Save(_path);
            }
            catch (Exception ex)
            {
                XmlErrorEvent?.Invoke(ex.Message);
            }
        }
        #endregion

        #region Редактирование записи
        public static void EditParam<T>(T param, string changedProperty)
        {

            try
            {
                var xDoc = new XmlDocument();
                xDoc.Load(_path);
                Type type = typeof(T);
                var props = typeof(T).GetProperties();
                XmlNodeList nodeList = xDoc.DocumentElement.GetElementsByTagName(type.Name);
                for (int i = 0; i < nodeList.Count; i++)
                {
                    foreach (var prop in props)
                    {
                        if (prop.Name == changedProperty) nodeList.Item(i).Attributes[changedProperty].Value = type.GetProperty(changedProperty).GetValue(param).ToString();
                    }
                }
                xDoc.Save(_path);
            }
            catch (Exception ex)
            {
                XmlErrorEvent?.Invoke(ex.Message);
            }
        }
        #endregion
    }
}
