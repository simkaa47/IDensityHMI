using IDensity.DataAccess;

namespace IDensity.Core.Services.CheckServices
{
    public class DeviceInformation:PropertyChangedBase
    {
        #region Серийник
        private string _serialNumber;
        public string SerialNumber
        {
            get => _serialNumber;
            set => Set(ref _serialNumber, value);
        }
        #endregion
       
        #region DeviceName
        private string _deviceName;
        public string DeviceName
        {
            get => _deviceName;
            set => Set(ref _deviceName, value);
        }
        #endregion
    }
}
