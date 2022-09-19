using IDensity.AddClasses;
using IDensity.Models;
using System.Collections.Generic;

namespace IDensity.ViewModels
{
    public class MeasUnitVm:PropertyChangedBase
    {
        public MeasUnitVm(VM vM)
        {
            VM = vM;
            
        }

        public VM VM { get; }

        #region Коллекция ЕИ
        /// <summary>
        /// Коллекция ЕИ
        /// </summary>
        private List<MeasNum> _measUnits;
        /// <summary>
        /// Коллекция ЕИ
        /// </summary>
        public List<MeasNum> MeasUnits
        {
            get => _measUnits;
            set => Set(ref _measUnits, value);
        }
        #endregion





    }
}
