using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Models
{
    public  class NetworkAdapterData:PropertyChangedBase
    {
        #region Имя
        /// <summary>
        /// Имя
        /// </summary>
        private string _name;
        /// <summary>
        /// Имя
        /// </summary>
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        #endregion

        #region Тип интерфейса
        /// <summary>
        /// Тип интерфейса
        /// </summary>
        private string _interfaceType;
        /// <summary>
        /// Тип интерфейса
        /// </summary>
        public string InterfaceType
        {
            get => _interfaceType;
            set => Set(ref _interfaceType, value);
        }
        #endregion

        #region Описание
        /// <summary>
        /// Описание
        /// </summary>
        private string _description;
        /// <summary>
        /// Описание
        /// </summary>
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }
        #endregion

        #region IP
        /// <summary>
        /// IP
        /// </summary>
        private string  _ip;
        /// <summary>
        /// IP
        /// </summary>
        public string  Ip
        {
            get => _ip;
            set => Set(ref _ip, value);
        }
        #endregion

        #region Selected
        /// <summary>
        /// Selected
        /// </summary>
        private bool _selected;
        /// <summary>
        /// Selected
        /// </summary>
        public bool Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }
        #endregion

    }
}
