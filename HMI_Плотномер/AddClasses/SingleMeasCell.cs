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
        private double _physVal;

        public double PhysVal
        {
            get { return _physVal; }
            set { Set(ref _physVal, value); }
        }
        #endregion

        #region Значение Ослабление
        private double weak;

        public double Weak
        {
            get { return weak; }
            set { Set(ref weak, value); }
        }

        #endregion




    }
}
