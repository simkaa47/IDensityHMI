using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace IDensity.AddClasses.EventHistory
{
    public class EventDevice: PropertyChangedBase
    {
        public event Action<EventDevice> EventExecuted;
        public enum EventTypes
        { 
            Error, Event
        }
        #region Конструктор
        public EventDevice(string id)
        {
            this.Id = id;
            GetFromSql();
            PropertyChanged += (o, e) => OnSqlPropertyChanged(e.PropertyName);
        }
        #endregion

        #region Дата, когда возникло последнее событие
        private DateTime _lastExecDate;

        public DateTime LastExecDate
        {
            get { return _lastExecDate; }
            set { Set(ref _lastExecDate, value); }
        } 
        #endregion

        #region Идентификатор события
        private string _id;
        /// <summary>
        /// Индификатор события
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }
        #endregion

        #region Номер события
        private int _num;
        /// <summary>
        /// Номер события
        /// </summary>
        public int Num
        {
            get { return _num; }
            set { Set(ref _num, value); }
        }

        #endregion

        #region Заголовок
        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }
        #endregion

        #region Тип ошибки
        private EventTypes _type;
        /// <summary>
        /// Тип события
        /// </summary>
        public int Type
        {
            get { return (int)_type; }
            set { Set(ref _type, (EventTypes)value); }
        }

        #endregion

        #region Описание
        private string _description;
        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }
        #endregion      

        #region Цвет активного события
        private Color _activeColor = Colors.Red;
        /// <summary>
        /// Цвет активного события
        /// </summary>
        public Color ActiveColor
        {
            get { return _activeColor; }
            set 
            {
                if (Set(ref _activeColor, value)) ActiveBrush = new SolidColorBrush(value); 
            }
        }
        #endregion

        #region Кисть активного события
        private SolidColorBrush _activeBrush;
        public SolidColorBrush ActiveBrush
        {
            get { return _activeBrush ?? (_activeBrush = new SolidColorBrush(ActiveColor)); }
            set { Set(ref _activeBrush, value); }
        }

        #endregion

        #region Цвет неактивного события
        private Color _notActiveColor = Colors.Green;

        public Color NotActiveColor
        {
            get { return _notActiveColor; }
            set 
            {
                if (Set(ref _notActiveColor, value)) NonActiveBrush = new SolidColorBrush(value);
            }            
        }

        #endregion

        #region Кисть неактивного события
        private SolidColorBrush _nonActiveBrush;
        public SolidColorBrush NonActiveBrush
        {
            get { return _nonActiveBrush ?? (_nonActiveBrush = new SolidColorBrush(NotActiveColor)); }
            set { Set(ref _nonActiveBrush, value); }
        }

        #endregion        

        #region Активность события
        private bool _issActive;

        public bool IsActive
        {
            get { return _issActive; }
            set 
            { 
                Set(ref _issActive, value);
                if (value == true || Type == (int)EventTypes.Error)
                {
                    LastExecDate = DateTime.Now;
                    EventExecuted?.Invoke(this);
                } 
                
            }
        }
        #endregion

        #region Синхронизация с SQL
        #region Событие ошибки работы с ДБ
        /// <summary>
        /// Событие ошибки работы с ДБ
        /// </summary>
        public event Action<string> SqlErrorEvent;
        #endregion

        #region Название БД
        /// <summary>
        /// Название БД
        /// </summary>
        public static string DbName { get; set; } = "application";
        #endregion

        #region Имя таблицы
        /// <summary>
        /// Имя таблицы
        /// </summary>
        public static string TableName { get; set; } = "events";
        #endregion

        #region Строка подлючения
        string ConnectionString { get => $"Data Source={DbName}.db;Mode=ReadWriteCreate"; }

        #endregion

        #region Методы работы с SQL
        #region прочитать из ДБ
        void GetFromSql()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    CreateTable();// Если базы данных не существует, создаем ее
                    SqliteCommand command = new SqliteCommand($"SELECT * FROM {TableName} WHERE Id = '{Id}' ;", connection);
                    SqliteDataReader reader = command.ExecuteReader();                    
                    if (reader.Read())
                    {
                        Id = reader.GetString(1);
                        Num = IntParse(reader.GetString(2));
                        Title = reader.GetString(3);
                        Description = reader.GetString(4);
                        ActiveColor = ColorFromInt(IntParse(reader.GetString(5)));
                        NotActiveColor = ColorFromInt(IntParse(reader.GetString(6)));
                        Type = IntParse(reader.GetString(7));
                    }
                    else
                    {
                        InsertToTable();
                    }
                }
                catch (Exception ex)
                {
                    SqlErrorEvent?.Invoke(ex.Message);
                }
            }
        }

        int IntParse(string input)
        {
            int temp = 0;
            int.TryParse(input.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries)[0], NumberStyles.HexNumber, null, out temp);
            return temp;

        }

        Color ColorFromInt(int value)
        {
            var bytes =  BitConverter.GetBytes(value).Reverse().ToArray();
            return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
        }
        #endregion

        #region Создание таблицы в базе данных
        /// <summary>
        /// Создание таблицы в базе данных
        /// </summary>
        void CreateTable()
        {
            string cmd = "CREATE TABLE IF NOT EXISTS  " + TableName + "(_id INTEGER NOT NULL PRIMARY KEY UNIQUE, Id TEXT NOT NULL, Num TEXT, Title TEXT, Description TEXT, ActiveColor TEXT, NotActiveColor TEXT, Type TEXT);";
            SqliteCommand command = new SqliteCommand(cmd);
            SqlExecuteCmd(command);
        }
        #endregion

        #region Добвление в таблицу
        void InsertToTable()
        {
            SqliteCommand command = new SqliteCommand();
            string cmd = $"INSERT INTO {TableName}(Id, Num, Title, Description, ActiveColor, NotActiveColor, Type) VALUES(@id, @num, @title, @desc, @actCol, @notActCol, @type);";
            command.Parameters.Add(new SqliteParameter("@id", Id));
            command.Parameters.Add(new SqliteParameter("@num", Num));
            command.Parameters.Add(new SqliteParameter("@title", Title!=null ? Title : ""));
            command.Parameters.Add(new SqliteParameter("@desc", Description!=null ? Description : ""));
            command.Parameters.Add(new SqliteParameter("@actCol", ActiveColor.ToString()));
            command.Parameters.Add(new SqliteParameter("@notActCol", NotActiveColor.ToString()));
            command.Parameters.Add(new SqliteParameter("@type", Type));
            command.CommandText = cmd;
            SqlExecuteCmd(command);
        }
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

        #region Редактирование строки        
        void UpdateSql(string setName, object setValue)
        {
            SqliteCommand command = new SqliteCommand();
            command.Parameters.Add(new SqliteParameter("@idValue", Id));
            command.Parameters.Add(new SqliteParameter("@setValue", setValue.ToString()));
            command.CommandText = $"UPDATE {TableName} SET {setName} = @setValue WHERE Id = '{Id}';";
            SqlExecuteCmd(command);
        }
        #endregion
        #region  Функция, вызываемая при изменении свойства
        /// <summary>
        /// Функция, вызываемая при изменении свойства
        /// </summary>
        void OnSqlPropertyChanged(string propName)
        {
            switch (propName)
            {
                case nameof(Id):
                    UpdateSql("Id", Id);
                    return;
                case nameof(Num):
                    UpdateSql("Num", Num);
                    return;
                case nameof(Title):
                    UpdateSql("Title", Title);
                    return;
                case nameof(Description):
                    UpdateSql("Description", Description);
                    return;
                case nameof(ActiveColor):
                    UpdateSql("ActiveColor", ActiveColor);
                    return;
                case nameof(NotActiveColor):
                    UpdateSql("NotActiveColor", NotActiveColor);
                    return;
                case nameof(Type):
                    UpdateSql("Type", Type);
                    return;
                default:
                    break;
            }

        }
        #endregion
        #endregion
        #endregion


    }
}
