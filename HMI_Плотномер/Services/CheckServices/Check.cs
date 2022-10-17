using IDensity.DataAccess;

namespace IDensity.Core.Services.CheckServices
{
    public class Check:PropertyChangedBase
    {
        #region Результат проверки всей секции
        private bool _checkResult;
        public bool CheckResult
        {
            get => _checkResult;
            set => Set(ref _checkResult, value);
        }
        #endregion
    }
}
