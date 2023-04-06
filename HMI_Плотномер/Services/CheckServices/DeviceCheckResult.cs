using IDensity.DataAccess;

namespace IDensity.Services.CheckServices
{
    public class DeviceCheckResult : PropertyChangedBase
    {
        #region Название блока проверки прибора
        /// <summary>
        /// Название блока проверки прибора
        /// </summary>
        private string _processName;
        /// <summary>
        /// Название блока проверки прибора
        /// </summary>
        public string ProcessName
        {
            get => _processName;
            set => Set(ref _processName, value);
        }
        #endregion

        #region Статус
        /// <summary>
        /// Статус
        /// </summary>
        private string _status;
        /// <summary>
        /// Статус
        /// </summary>
        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }
        #endregion

        #region Ошибка
        /// <summary>
        /// Ошибка
        /// </summary>
        private bool _isError;
        /// <summary>
        /// Ошибка
        /// </summary>
        public bool IsError
        {
            get => _isError;
            set => Set(ref _isError, value);
        }
        #endregion



    }
}
