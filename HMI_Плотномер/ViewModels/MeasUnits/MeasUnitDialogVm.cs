using IDensity.AddClasses;
using IDensity.DataAccess.Models;
using System.Collections.Generic;
using PropertyChangedBase = IDensity.DataAccess.PropertyChangedBase;

namespace IDensity.Core.ViewModels.MeasUnits
{
    public class MeasUnitDialogVm : PropertyChangedBase
    {
        #region ЕИ
        /// <summary>
        /// ЕИ
        /// </summary>
        private MeasUnit _measUnit;
        /// <summary>
        /// ЕИ
        /// </summary>
        public MeasUnit MeasUnit
        {
            get => _measUnit;
            set => Set(ref _measUnit, value);
        }
        #endregion

        #region Имена
        /// <summary>
        /// Имена
        /// </summary>
        private IEnumerable<EnumCustom> _measUnitNames;
        /// <summary>
        /// Имена
        /// </summary>
        public IEnumerable<EnumCustom> measUnitNames
        {
            get => _measUnitNames;
            set => Set(ref _measUnitNames, value);
        }
        #endregion

    }
}
