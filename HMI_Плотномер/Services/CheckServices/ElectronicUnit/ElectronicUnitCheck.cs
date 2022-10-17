using IDensity.DataAccess;
using IDensity.Services.CheckServices;

namespace IDensity.Core.Services.CheckServices.ElectronicUnit
{
    public class ElectronicUnitCheck : Check
    {
        
        #region Ппроверка напряжения
        private DeviceCheckResult _hvSourceCheck;
        public DeviceCheckResult HvSourceCheck
        {
            get => _hvSourceCheck;
            set => Set(ref _hvSourceCheck, value);
        }
        #endregion

        #region Проверка AO0
        private DeviceCheckResult _analogCheck0;
        public DeviceCheckResult AnalogCheck0
        {
            get => _analogCheck0;
            set => Set(ref _analogCheck0, value);
        }
        #endregion

        #region Проверка AO1
        private DeviceCheckResult _analogCheck1;
        public DeviceCheckResult AnalogCheck1
        {
            get => _analogCheck1;
            set => Set(ref _analogCheck1, value);
        }
        #endregion

        #region Проверка Контрольной суммы
        private DeviceCheckResult _softwareCheck;
        public DeviceCheckResult SoftwareCheck
        {
            get => _softwareCheck;
            set => Set(ref _softwareCheck, value);
        }
        #endregion

        #region Проверка RTC
        private DeviceCheckResult _rtcCheck;
        public DeviceCheckResult RtcCheck
        {
            get => _rtcCheck;
            set => Set(ref _rtcCheck, value);
        }
        #endregion




    }
}
