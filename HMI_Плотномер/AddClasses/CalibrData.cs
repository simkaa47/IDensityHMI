using IDensity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDensity.AddClasses
{
    class CalibrData:ICloneable
    {
        #region Номер набора данных калибровки
        /// <summary>
        /// Номер набора данных калибровки
        /// </summary>
        public Parameter<ushort> Num { get; set; } = new Parameter<ushort>("CalibrDataNum", "Номер набора данных калибровки", 0, (ushort)MainModel.CalibCurveNum, 94, "hold");
        #endregion
        #region Номер единицы измерения
        /// <summary>
        /// Номер единицы измерения
        /// </summary>
        public Parameter<ushort> MeasUnitNum { get; set; } = new Parameter<ushort>("CalibrDataMeasUnitNum", "Номер еденицы измерения", 0, ushort.MaxValue, 95, "hold");
        #endregion
        #region КОэффициенты калибровки
        /// <summary>
        /// КОэффициенты калибровки
        /// </summary>
        public List<Parameter<float>> Coeffs { get; set; } = Enumerable.Range(0, 6).Select(i => new Parameter<float>("CalibrCoeff" + i, "Коэффициент калибровочной кривой " + i, float.NegativeInfinity, float.PositiveInfinity, 96 + i * 2, "hold")).ToList();

        #endregion
        public object Clone()
        {
            return new CalibrData()
            {
                Num = this.Num.Clone() as Parameter<ushort>,
                MeasUnitNum = this.MeasUnitNum.Clone() as Parameter<ushort>,
                Coeffs = this.Coeffs.Select(coeff=>coeff.Clone() as Parameter<float>).ToList()
            };
        }


    }
}
