using IDensity.Models.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    //Характеризует набор значения счетчика после ед. измерения и физисечкой величины
    class SingleMeasCell : PropertyChangedBase, IDataBased
    {

        public long Id { get; set; }
        #region Физическая величина
        private float _physVal;

        public float PhysVal
        {
            get { return _physVal; }
            set { Set(ref _physVal, value); }
        }
        #endregion

        #region Значение счетчика
        private float _counterValue;

        public float CounterValue
        {
            get { return _counterValue; }
            set { Set(ref _counterValue, value); }
        }

        #endregion

    }
}
