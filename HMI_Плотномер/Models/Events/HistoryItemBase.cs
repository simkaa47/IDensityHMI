using System;

namespace IDensity.Core.Models.Events
{
    /// <summary>
    /// Базовый класс для элемента истории событий
    /// </summary>
    public abstract class HistoryItemBase
    {
        #region Имя пользователя
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; }
        #endregion

        #region Время, когда произошло событие
        /// <summary>
        /// Время, когда произошло событие
        /// </summary>
        public DateTime EventTime { get; set; }
        #endregion

        #region Активность события
        private bool _activity;

        public bool Activity
        {
            get { return _activity; }
            set { _activity = value; }
        }
        #endregion

    }
}
