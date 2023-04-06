using IDensity.AddClasses;
using IDensity.DataAccess;
using IDensity.Models;

namespace IDensity.Core.Models.Tcp
{
    public class TcpConnectData : PropertyChangedBase
    {
        #region IP адрес платы
        string _ip = "192.168.10.151";
        /// <summary>
        /// IP адрес платы
        /// </summary>
        public string IP
        {
            get => _ip;
            set
            {
                if (MainModel.CheckIp(value)) Set(ref _ip, value);
            }
        }
        #endregion

        #region Номер порта
        int _portNum;
        public int PortNum { get => _portNum; set => Set(ref _portNum, value); }
        #endregion

        #region Пауза между запросами
        private int _pause;

        public int Pause
        {
            get { return _pause; }
            set
            {
                if (value >= 10) Set(ref _pause, value);
            }
        }

        #endregion

        #region Циклический опрос
        private bool _cycicRequest;

        public bool CycicRequest
        {
            get { return _cycicRequest; }
            set { Set(ref _cycicRequest, value); }
        }

        #endregion
    }
}
