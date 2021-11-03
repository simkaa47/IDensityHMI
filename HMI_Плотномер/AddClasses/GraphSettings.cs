using System;
using System.Collections.Generic;
using System.Text;

namespace HMI_Плотномер.AddClasses
{
    class GraphSettings:PropertyChangedBase
    {
        #region Временной интервал отображаемых данных
        int _plotTime = 1;
        public int PlotTime
        {
            get => _plotTime;
            set
            {
                int temp = 0;
                if (value > 60) temp = 60;
                else if (value < 1) temp = 1;
                else temp = value;
                Set(ref _plotTime, temp);
            }
        }
        #endregion

        #region Минимальный предел
        float _plotLowLimit;
        public float PlotLowLimit
        {
            get => _plotLowLimit;
            set
            {
                float temp;
                if (value >= PlotHighLimit) temp = 0;
                else temp = value;
                Set(ref _plotLowLimit, temp);
            }
        }
        #endregion

        #region Максимальный предел
        float _plotHighlimit;
        public float PlotHighLimit
        {
            get => _plotHighlimit;
            set
            {
                float temp;
                if (value <= PlotLowLimit) temp = PlotLowLimit + 1;
                else temp = value;
                Set(ref _plotHighlimit, temp);
            }
        } 
        #endregion
    }
}
