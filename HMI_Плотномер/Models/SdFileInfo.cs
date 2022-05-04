using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Models
{
    class SdFileInfo : PropertyChangedBase
    {

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


    }
        
}
