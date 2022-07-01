using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using IDensity.AddClasses;
using Microsoft.Data.Sqlite;

namespace IDensity.Models.SQL
{
    /// <summary>
    /// Коллекция, синхронизирующаяся с базой данных
    /// </summary>
    public class DataBaseCollection<T> where T: PropertyChangedBase, IDataBased
    {
        #region Событие изменения коллекции
        public event Action CollectionChangedEvent;
        #endregion
        #region Событие ошибки работы с ДБ
        /// <summary>
        /// Событие ошибки работы с ДБ
        /// </summary>
        public event Action<string> SqlErrorEvent;
        #endregion
        #region Имя ДБ
        private string DBName { get; } = "application"; 
        #endregion
        #region Имя таблицы
        /// <summary>
        /// Имя таблицы
        /// </summary>
        public readonly string TableName;
        #endregion
        #region Коллекция, которую надо синхронизировать
        /// <summary>
        /// Коллекция, которую надо синхронизировать с ДБ
        /// </summary>
        public ObservableCollection<T> Data { get; }
        #endregion
        #region Строка подлючения
        string ConnectionString { get => $"Data Source={DBName}.db;Mode=ReadWriteCreate";}

        #endregion
        #region Конструктор
        public DataBaseCollection(string tableName, T defaultCell)
        {
            TableName = tableName;
            Data = new ObservableCollection<T>();// создаем коллекцию
            DoSqlCommand(ReadFromSql);// Читаем данные
            // Подписка на изменение коллекции
            Data.CollectionChanged += UpdateCollection;
            Data.CollectionChanged += (o, e) =>
             {
                 CollectionChangedEvent?.Invoke();
             };
            if (Data.Count == 0 && defaultCell != null)// Если ничего не прочитали, то добавляем значение по умолчанию
                Data.Add(defaultCell);

            // Подписка на изменение свойтсва каждого элемента коллекции
            foreach (var item in Data) 
            {
                item.PropertyChanged += EditCellSql;
                item.PropertyChanged += (o, e) => CollectionChangedEvent?.Invoke();
            } 
        }
        #endregion
        #region Методы работы с ДБ
        #region Взять данные из базы данных
        /// <summary>
        /// Взять данные из базы данных
        /// </summary>
        void  ReadFromSql()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                Data.Clear();
                CreateTable();// Если базы данных не существует, создаем ее
                SqliteCommand command = new SqliteCommand($"SELECT * FROM {TableName};", connection);
                SqliteDataReader reader = command.ExecuteReader();
                Type type = typeof(T);
                var props = type.GetProperties();
                while (reader.Read())
                {
                    T cell = (T)Activator.CreateInstance(type);
                    for (int i = 0; i < props.Length; i++)
                    {
                        type.GetProperty(props[i].Name).SetValue(cell, reader.GetValue(i));
                    }
                    Data.Add(cell);
                }
            }
        }
        #endregion
        
        #region Создание таблицы в базе данных
        /// <summary>
        /// Создание таблицы в базе данных
        /// </summary>
        void CreateTable()
        {
            var props = typeof(T).GetProperties();
            string cmd = "CREATE TABLE IF NOT EXISTS  " + TableName + "(";
            foreach (var item in props)
            {
                cmd += item.Name;
                cmd += " " + GetSqlTypeName(item.PropertyType);
                cmd += ", ";
            }
            cmd = cmd.Substring(0, cmd.Length - 2);
            cmd += ");";
            SqliteCommand command = new SqliteCommand(cmd);
            SqlExecuteCmd(command);
        }
        #endregion
        #region Сопоставление имен system и sql
        /// <summary>
        /// Сопоставление имен system и sql
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public static string GetSqlTypeName(Type propertyType)
        {
            switch (propertyType.Name.ToLower())
            {
                case "int":
                case "int32":
                case "int64":
                    return "INTEGER NOT NULL default 0";
                case "string": return "nvarchar(40) NOT NULL";
                case "double": return "float NOT NULL";
                case "float": return "real NOT NULL";
                default:
                    throw new Exception($"Для типа {propertyType.Name} не определен соответствующий SQL  тип!");
            }
        }
        #endregion
        #region Выполнить sql команду
        /// <summary>
        /// Выполнить sql команду
        /// </summary>
        /// <param name="command"></param>
        void SqlExecuteCmd(SqliteCommand command)
        {                         
                using (SqliteConnection connection = new SqliteConnection(ConnectionString))
                {
                DoSqlCommand(() =>
                {
                    connection.Open();
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
                );                    
                }            
        }
        #endregion
        #region Проверка наличия свойства в классе
        bool ContainStringArr(PropertyInfo[] props, string name)
        {
            foreach (var item in props)
            {
                if (item.Name == name) return true;
            }
            return false;
        }
        #endregion
        #region Изменение данных в коллекции
        void EditCellSql(object sender, PropertyChangedEventArgs e)
        {
            Type type = typeof(T);
            var props = type.GetProperties();
            if (ContainStringArr(props, "Id") && ContainStringArr(props, e.PropertyName) && type == sender.GetType())
            {
                var index = (long)type.GetProperty("Id").GetValue((T)sender);
                var propValue = type.GetProperty(e.PropertyName).GetValue((T)sender);
                UpdateSql("Id", index, e.PropertyName, propValue);
            }
        }
        #endregion
        #region Редактирование данных
        void UpdateSql(string idName, object idValue, string setName, object setValue)
        {
            SqliteCommand command = new SqliteCommand();
            command.Parameters.Add(new SqliteParameter("@idValue", idValue));
            command.Parameters.Add(new SqliteParameter("@setValue", setValue));
            command.CommandText = $"UPDATE {TableName} SET {setName} = @setValue WHERE {idName} = @idValue;";
            SqlExecuteCmd(command);
        }
        #endregion
        #region Добавление и удаление данных из коллекции
        /// <summary>
        /// Добавление и удаление данных из коллекции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="tableName"></param>
        void UpdateCollection(object sender, NotifyCollectionChangedEventArgs e)
        {
            Type type = typeof(T);
            var props = type.GetProperties();
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ObservableCollection<T> collection = sender as ObservableCollection<T>;
                if (collection != null && collection.Count >= 1 && ContainStringArr(props, "Id"))
                {
                    long index = 0;
                    if(collection.Count>1)index = (long)type.GetProperty("Id").GetValue(collection[collection.Count - 2]) + 1;
                    type.GetProperty("Id").SetValue((T)e.NewItems[0], index);
                    InsertToTable((T)e.NewItems[0]);
                    (e.NewItems[0] as T).PropertyChanged += EditCellSql;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (ContainStringArr(props, "Id"))
                {
                    foreach (var deleteItem in e.OldItems)
                    {
                        var index = (long)type.GetProperty("Id").GetValue((T)deleteItem);
                        DeleteFromSql("Id", index);
                    }
                }
            }
        }
        #endregion
        #region Добвление в таблицу
        void InsertToTable(T cell)
        {
            SqliteCommand command = new SqliteCommand();
            Type myType = cell.GetType();
            string cmd = $"INSERT INTO {TableName}(";
            string values = "VALUES(";
            var props = myType.GetProperties();
            string paramName = null;
            foreach (var item in props)
            {
                cmd = cmd + item.Name + ", ";
                paramName = $"@{item.Name}";
                command.Parameters.Add(new SqliteParameter(paramName, myType.GetProperty(item.Name).GetValue(cell)));
                values += paramName;
                values += ", ";
            }
            cmd = cmd.Substring(0, cmd.Length - 2);
            cmd += ")";
            values = values.Substring(0, values.Length - 2);
            values += ")";
            cmd = cmd + values;
            cmd += ";";
            command.CommandText = cmd;
            SqlExecuteCmd(command);
        }
        #endregion
        #region Удаление данных из таблицы
        void DeleteFromSql(string propertyName, object propertyValue)
        {
            string propertyValueString = (propertyValue.GetType() != typeof(string)) ? propertyValue.ToString() : "'" + propertyValue.ToString() + "'";
            string cmd = $"DELETE FROM {TableName} WHERE {propertyName} = {propertyValueString};";
            SqliteCommand command = new SqliteCommand(cmd);
            SqlExecuteCmd(command);
        }
        #endregion
        #endregion
        #region Общий метод для отлавливания исключений
        void DoSqlCommand(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                SqlErrorEvent?.Invoke(ex.Message);
            }
        }
        #endregion
    }
}
