using IDensity.AddClasses;
using IDensity.DataAccess;

namespace IDensity.Core.Models.Adc
{
    public class AdcAvgSettings : PropertyChangedBase
    {
        #region Глубина скользящего среднего для спектра
        /// <summary>
        /// Глубина скользящего среднего для спектра
        /// </summary>
        private int _spectrFilterDeep = 1;
        /// <summary>
        /// Глубина скользящего среднего для спектра
        /// </summary>
        public int SpectrFilterDeep
        {
            get => _spectrFilterDeep;
            set
            {
                if (value > 0) Set(ref _spectrFilterDeep, value);
            }
        }
        #endregion       

        #region Минимальный предел для поиска максимума спектра
        /// <summary>
        /// Минимальный предел для поиска максимума спектра
        /// </summary>
        private int _spectrMinLimit;
        /// <summary>
        /// Минимальный предел для поиска максимума спектра
        /// </summary>
        public int SpectrMinLimit
        {
            get => _spectrMinLimit;
            set
            {
                if (value > 0 && value < 4096)
                {
                    if (value > SpectrMaxLimit) SpectrMaxLimit = value;
                    Set(ref _spectrMinLimit, value);
                }
            }
        }
        #endregion

        #region Максимальный предел для поиска масксимума спектра
        /// <summary>
        /// Максимальный предел для поиска масксимума спектра
        /// </summary>
        private int _spectrMaxLimit;
        /// <summary>
        /// Максимальный предел для поиска масксимума спектра
        /// </summary>
        public int SpectrMaxLimit
        {
            get => _spectrMaxLimit;
            set
            {
                if (value > 0 && value < 4096)
                {
                    if (value < SpectrMinLimit) SpectrMinLimit = value;
                    Set(ref _spectrMaxLimit, value);
                }
            }
        }
        #endregion

        #region Смещение левого предела диапазона счетчика отностительно индекса максмума спетра
        /// <summary>
        /// Смещение левого предела диапазона счетчика отностительно индекса максмума спетра
        /// </summary>
        private double _leftCounterCoeff;
        /// <summary>
        /// Смещение левого предела диапазона счетчика отностительно индекса максмума спетра
        /// </summary>
        public double LeftCounterCoeff
        {
            get => _leftCounterCoeff;
            set { if (value >= 0 && value <= 1) Set(ref _leftCounterCoeff, value); }
        }
        #endregion

        #region Смещение правого предела диапазона счетчикаотносительно иедекса максимума спекра
        /// <summary>
        /// Смещение правого предела диапазона счетчикаотносительно иедекса максимума спекра
        /// </summary>
        private double _rightCounterCoeff;
        /// <summary>
        /// Смещение правого предела диапазона счетчикаотносительно иедекса максимума спекра
        /// </summary>
        public double RightCounterCoeff
        {
            get => _rightCounterCoeff;
            set { if (value >= 0 && value <= 1) Set(ref _rightCounterCoeff, value); }
        }
        #endregion
    }
}
