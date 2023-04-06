using IDensity.DataAccess;

namespace IDensity.Services.AdcServices
{
    public class CheckPulseResult : PropertyChangedBase
    {

        #region Общее количество испульсов
        /// <summary>
        /// Общее количество испульсов
        /// </summary>
        private int _commontCount;
        /// <summary>
        /// Общее количество испульсов
        /// </summary>
        public int CommontCount
        {
            get => _commontCount;
            set => Set(ref _commontCount, value);
        }
        #endregion

        #region Количество импульсов с пересветом
        /// <summary>
        /// Количество импульсов с пересветом
        /// </summary>
        private int _overValuePulses;
        /// <summary>
        /// Количество импульсов с пересветом
        /// </summary>
        public int OverValuePulses
        {
            get => _overValuePulses;
            set => Set(ref _overValuePulses, value);
        }
        #endregion

        #region Количесвто шумов
        /// <summary>
        /// Количесвто шумов
        /// </summary>
        private int _noisePulses;
        /// <summary>
        /// Количесвто шумов
        /// </summary>
        public int NoisePulses
        {
            get => _noisePulses;
            set => Set(ref _noisePulses, value);
        }
        #endregion

        #region Процент удачных импульсов
        /// <summary>
        /// Процент удачных импульсов
        /// </summary>
        private int _successDeviation;
        /// <summary>
        /// Процент удачных импульсов
        /// </summary>
        public int SuccessDeviation
        {
            get => _successDeviation;
            set => Set(ref _successDeviation, value);
        }
        #endregion

    }
}
