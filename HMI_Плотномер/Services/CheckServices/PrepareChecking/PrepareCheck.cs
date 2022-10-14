using IDensity.DataAccess;
using IDensity.Services.CheckServices;

namespace IDensity.Core.Services.CheckServices
{
    public class PrepareCheck:PropertyChangedBase
    {
        private bool _checkResult;
        public bool CheckResult
        {
            get => _checkResult;
            set => Set(ref _checkResult, value);
        }

        public DeviceCheckResult CheckSysytemErrors { get; } = new DeviceCheckResult();
    }
}
