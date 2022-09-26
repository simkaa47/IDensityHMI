using IDensity.AddClasses;
using System.Collections.Generic;
using System.Linq;

namespace IDensity.Core.AddClasses.Settings
{
    public class GetTemperature : PropertyChangedBase
    {
        #region Источник температуры
        private int _source; 
        public int Source
        {
            get => _source;
            set => Set(ref _source, value);
        }
        #endregion

        #region Настройки пересчета температуры
        public List<TempRecalculateCoeffs> Coeffs { get; } = Enumerable.Range(0, 2).Select(i => new TempRecalculateCoeffs(i)).ToList();
        #endregion

    }
}
