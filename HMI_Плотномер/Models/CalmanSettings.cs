using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Core.Models
{
    public class CalmanSettings:PropertyChangedBase
    {
        #region Номер
        /// <summary>
        /// Номер
        /// </summary>
        private int _num;
        /// <summary>
        /// Номер
        /// </summary>
        public int Num
        {
            get => _num;
            set => Set(ref _num, value);
        }
        #endregion

        #region Скорость
        /// <summary>
        /// Скорость
        /// </summary>
        private float _speed;
        /// <summary>
        /// Скорость
        /// </summary>
        public float Speed
        {
            get => _speed;
            set => Set(ref _speed, value);
        }
        #endregion

        #region Сглаживание
        /// <summary>
        /// Сглаживание
        /// </summary>
        private float _smooth;
        /// <summary>
        /// Сглаживание
        /// </summary>
        public float Smooth
        {
            get => _smooth;
            set => Set(ref _smooth, value);
        }
        #endregion

    }
}
