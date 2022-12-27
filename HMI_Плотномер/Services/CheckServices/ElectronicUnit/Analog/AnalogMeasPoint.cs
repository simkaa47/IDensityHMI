using IDensity.AddClasses;

namespace IDensity.Core.Services.CheckServices.ElectronicUnit.Analog
{
    public enum AnalogCheckState
    {
        Ok,
        BreakError,
        DeviationError
    }
    public class AnalogMeasPoint:PropertyChangedBase
    {
        public AnalogMeasPoint(float setValue, float measValue)
        {
            SetValue = setValue;
            MeasValue = measValue;
            MaxDeviation = 0.016f;
        }

        #region Уставка
        /// <summary>
        /// Уставка
        /// </summary>
        private float _setValue;
        /// <summary>
        /// Уставка
        /// </summary>
        public float SetValue
        {
            get => _setValue;
            set => Set(ref _setValue, value);
        }
        #endregion

        #region Измеренное значение
        /// <summary>
        /// Измеренное значение
        /// </summary>
        private float _measValue;
        /// <summary>
        /// Измеренное значение
        /// </summary>
        public float MeasValue
        {
            get => _measValue;
            set => Set(ref _measValue, value);
        }
        #endregion

        #region Максимальное отклонение
        /// <summary>
        /// Максимальное отклонение
        /// </summary>
        private float _maxDeviation;
        /// <summary>
        /// Максимальное отклонение
        /// </summary>
        public float MaxDeviation
        {
            get => _maxDeviation;
            set => Set(ref _maxDeviation, value);
        }
        #endregion

        #region Статус замера 
        /// <summary>
        /// Статус проверки 
        /// </summary>
        private AnalogCheckState _status;
        /// <summary>
        /// Статус проверки 
        /// </summary>
        public AnalogCheckState Status
        {
            get => _status;
            set => Set(ref _status, value);
        }
        #endregion
    }
}
