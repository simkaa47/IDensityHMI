using IDensity.AddClasses;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace IDensity.Core.AddClasses.Settings
{
    [DataContract]
    public class GetTemperature : PropertyChangedBase
    {
        #region Источник температуры
        private int _source;
        [DataMember]
        public int Source
        {
            get => _source;
            set => Set(ref _source, value);
        }
        #endregion

        #region Настройки пересчета температуры
        [DataMember]
        public List<TempRecalculateCoeffs> Coeffs { get; set; } = Enumerable.Range(0, 2).Select(i => new TempRecalculateCoeffs(i)).ToList();
        #endregion

    }
}
