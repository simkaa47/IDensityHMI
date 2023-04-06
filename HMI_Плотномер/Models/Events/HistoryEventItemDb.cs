namespace IDensity.Core.Models.Events
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
