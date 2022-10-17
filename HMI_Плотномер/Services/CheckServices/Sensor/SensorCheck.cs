using IDensity.AddClasses;
using IDensity.Services.CheckServices;

namespace IDensity.Core.Services.CheckServices.Sensor
{
    public class SensorCheck :Check
    { 
        #region Проверка правильности формы импульсв
        private DeviceCheckResult _pulseCheck;
        public DeviceCheckResult PulseCheck
        {
            get => _pulseCheck;
            set => Set(ref _pulseCheck, value);
        }
        #endregion

        #region Проверка отклонения высокого напряжения на ФЭУ
        private DeviceCheckResult _hvCheck;
        public DeviceCheckResult HvCheck
        {
            get => _hvCheck;
            set => Set(ref _hvCheck, value);
        }
        #endregion
    }
}
