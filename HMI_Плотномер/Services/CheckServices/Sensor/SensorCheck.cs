using IDensity.AddClasses;
using IDensity.Services.CheckServices;

namespace IDensity.Core.Services.CheckServices.Sensor
{
    class SensorCheck :PropertyChangedBase
    {

        #region Результат проверки всей секции
        private bool _checkResult;
        public bool CheckResult
        {
            get => _checkResult;
            set => Set(ref _checkResult, value);
        }
        #endregion

        #region Проверка правильности формы импульсв
        private DeviceCheckResult _pulseCheck;
        public DeviceCheckResult PulseCheck
        {
            get => _pulseCheck;
            set => Set(ref _pulseCheck, value);
        }
        #endregion




    }
}
