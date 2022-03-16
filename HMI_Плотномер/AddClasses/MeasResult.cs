using IDensity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    class MeasResult:PropertyChangedBase
    {
        #region Значение счетчика, импульсов/с
        /// <summary>
        /// Значение счетчика, импульсов/с
        /// </summary>
        public Parameter<float> CounterValue { get; } = new Parameter<float>("MeasResultCounter", "Значение счетчика, импульсов/с.", 0, float.MaxValue, 0, "read");
        #endregion
        #region Мгновенное значение ФВ
        /// <summary>
        /// Мгновенное значение ФВ
        /// </summary>
        public Parameter<float> PhysValueCur { get; } = new Parameter<float>("MeasResultPhysValueCur", "Мгновенное значение.", 0, float.MaxValue, 0, "read");
        #endregion
        #region Усредненное значение ФВ
        /// <summary>
        /// Усредненное значение ФВ
        /// </summary>
        public Parameter<float> PhysValueAvg { get; } = new Parameter<float>("MeasResultPhysValueAvg", "Усредненное значение.", 0, float.MaxValue, 0, "read");
        #endregion

        #region Номер измерительного процесса
        public Parameter<ushort> MeasProcessNum { get; } = new Parameter<ushort>("MeasResultProcNum", "Номер измерительного процесса", 0, MainModel.MeasProcNum, 0, "");
        #endregion

        #region Тип Калибровки
        public Parameter<ushort> CalibrType { get; } = new Parameter<ushort>("MeasResultType", "Тип измерения", 0, 10, 0, "");
        #endregion

        #region Активность
        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set { Set(ref _isActive, value); }
        } 
        #endregion

        public void ClearResult()
        {
            PhysValueCur.Value = 0;
            PhysValueAvg.Value = 0;
        }

        
    }
}
