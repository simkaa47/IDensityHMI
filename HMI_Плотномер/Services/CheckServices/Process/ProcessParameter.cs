using IDensity.AddClasses;

namespace IDensity.Core.Services.CheckServices
{
    public class ProcessParameter : PropertyChangedBase
    {
        #region Описание параметра
        private string _description;
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }
        #endregion

        #region ЕИ
        private string _measUnit;
        public string MeasUnit
        {
            get => _measUnit;
            set => Set(ref _measUnit, value);
        }
        #endregion

        #region Актуальная величина
        private float _value;
        public float Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
        #endregion

        #region Минимальная величина
        private float _minValue;
        public float MinValue
        {
            get => _minValue;
            set => Set(ref _minValue, value);
        }
        #endregion

        #region Максимальная величина
        private float _maxValue;
        public float MaxValue
        {
            get => _maxValue;
            set => Set(ref _maxValue, value);
        }
        #endregion

    }
}
