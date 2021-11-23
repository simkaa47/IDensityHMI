using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HMI_Плотномер.AddClasses
{
    class Diapasone : ICloneable
    {
        #region Номер калибровочной кривой
        public Parameter<ushort> CalibCurveNum { get; set; } = new Parameter<ushort>("CalibCurveNumDiap", "Номер калибровочной кривой", 0, 8, 0, "");
        #endregion

        #region Номер стандартизации ЕИ
        public Parameter<ushort> StandNum { get; set; } = new Parameter<ushort>("StandNumDiap", "Номер стандартизации", 0, 11, 0, "");
        #endregion

        #region Номер счетчика
        public Parameter<ushort> CounterNum { get; set; } = new Parameter<ushort>("CounterNumDiap", "Номер счетчика", 0, 7, 0, "");
        #endregion

        public object Clone()
        {
            return new Diapasone()
            {
                CalibCurveNum = this.CalibCurveNum.Clone() as Parameter<ushort>,
                StandNum = this.StandNum.Clone() as Parameter<ushort>,
                CounterNum = this.CounterNum.Clone() as Parameter<ushort>
            };
        }
    }
}
