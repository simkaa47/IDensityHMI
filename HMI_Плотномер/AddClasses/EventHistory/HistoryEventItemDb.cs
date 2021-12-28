using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses.EventHistory
{
    /// <summary>
    /// Класс, для загрузки данных из БД
    /// </summary>
    class HistoryEventItemDb : HistoryItemBase
    {       

        #region Id собятия
        /// <summary>
        /// Id события
        /// </summary>
        public string Id { get; set; } 
        #endregion
    }
}
