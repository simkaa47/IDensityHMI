using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Models
{
    public class SdFileInfo : PropertyChangedBase
    {

        #region Номер
        /// <summary>
        /// Номер
        /// </summary>
        private int _id;
        /// <summary>
        /// Номер
        /// </summary>
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        #endregion


        #region Имя файла
        /// <summary>
        /// Имя файла
        /// </summary>
        private string _name;
        /// <summary>
        /// Имя файла
        /// </summary>
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        #endregion


        #region Кол-во записей в файле
        /// <summary>
        /// Кол-во записей в файле
        /// </summary>
        private int _writeNumber;
        /// <summary>
        /// Кол-во записей в файле
        /// </summary>
        public int WriteNumber
        {
            get => _writeNumber;
            set => Set(ref _writeNumber, value);
        }
        #endregion


        #region Стартовый номер для записи 
        /// <summary>
        /// Стартовый номер для записи 
        /// </summary>
        private int _start;
        /// <summary>
        /// Стартовый номер для записи 
        /// </summary>
        public int Start
        {
            get => _start;
            set 
            {
                if (value >= 0 && value <= WriteNumber)
                {
                    if (value > finish) finish = value;
                    Set(ref _start, value);
                } 
            } 
        }
        #endregion


        #region Конечный номер для чтения
        /// <summary>
        /// Конечный номер для чтения
        /// </summary>
        private int _finish;
        /// <summary>
        /// Конечный номер для чтения
        /// </summary>
        public int finish
        {
            get => _finish;
            set 
            {
                if (value >= 0 && value <= WriteNumber)
                {
                    if (value < Start) Start = value;
                    Set(ref _finish, value);
                }
            } 
        }
        #endregion


    }
        
}
