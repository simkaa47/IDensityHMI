using IDensity.AddClasses;

namespace IDensity.Core.AddClasses.Settings
{
    public class Kalman:PropertyChangedBase
    {
        public Kalman(int index)
        {
            Index = index;
        }

        private int _index;
        public int Index
        {
            get => _index;
            set => Set(ref _index, value);
        }

        #region Скорость измрениния, к-т
        public Parameter<float> Speed { get; } = new Parameter<float>("KalmanSpeed", "К-т скорости изменения сигнала", float.MinValue, float.MaxValue, 0, "");
        #endregion

        #region К-т сглаживания
        public Parameter<float> Smooth { get; } = new Parameter<float>("KalmanSmooth", "К-т сглаживания", float.MinValue, float.MaxValue, 0, "");
        #endregion
    }
}
