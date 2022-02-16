using IDensity.Models;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDensity.AddClasses
{
    class CalibrData:PropertyChangedBase
    {
        #region Тип калибровки
        /// <summary>
        /// Тип калибровки
        /// </summary>
        public Parameter<ushort> Type { get; } = new Parameter<ushort>("CalibrType", "Тип калибровки", 0, 10, 0, "hold"); 
        #endregion
        #region Номер единицы измерения
        /// <summary>
        /// Номер единицы измерения
        /// </summary>
        public Parameter<ushort> MeasUnitNum { get; set; } = new Parameter<ushort>("CalibrDataMeasUnitNum", "Номер еденицы измерения", 0, ushort.MaxValue, 0, "hold");
        #endregion
        #region КОэффициенты калибровки
        /// <summary>
        /// КОэффициенты калибровки
        /// </summary>
        public List<Parameter<float>> Coeffs { get; set; } = Enumerable.Range(0, 6).Select(i => new Parameter<float>("CalibrCoeff" + i, "Коэффициент калибровочной кривой " + i, float.NegativeInfinity, float.PositiveInfinity, 97 + i * 2, "hold")).ToList();

        #endregion
    }
}
