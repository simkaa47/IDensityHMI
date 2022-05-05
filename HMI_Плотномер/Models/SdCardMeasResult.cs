using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Models
{
    class SdCardMeasResult:PropertyChangedBase
    {

        #region Дата и время измерения
        /// <summary>
        /// Дата и время измерения
        /// </summary>
        private DateTime _time;
        /// <summary>
        /// Дата и время измерения
        /// </summary>
        public DateTime Time
        {
            get => _time;
            set => Set(ref _time, value);
        }
        #endregion

        



    }

}
