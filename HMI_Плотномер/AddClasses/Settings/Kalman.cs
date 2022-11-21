using IDensity.AddClasses;
using System.Runtime.Serialization;

namespace IDensity.Core.AddClasses.Settings
{
    [DataContract]
    public class Kalman:PropertyChangedBase
    {
        public Kalman(int index)
        {
            Index = index;
        }

        private int _index;
        [DataMember]
        public int Index
        {
            get => _index;
            set => Set(ref _index, value);
        }

        #region Скорость измрениния, к-т
        [DataMember]
        public Parameter<float> Speed { get; set; } = new Parameter<float>("KalmanSpeed", "К-т скорости изменения сигнала", float.MinValue, float.MaxValue, 0, "");
        #endregion

        #region К-т сглаживания
        [DataMember]
        public Parameter<float> Smooth { get; set; } = new Parameter<float>("KalmanSmooth", "К-т сглаживания", float.MinValue, float.MaxValue, 0, "");
        #endregion
    }
}
