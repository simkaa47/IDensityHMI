using IDensity.AddClasses;
using IDensity.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Core.Models.Trends
{
    public class GraphSettings : PropertyChangedBase
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
                if (value >= PlotHighLimit) PlotHighLimit = value + 1;
                Set(ref _plotLowLimit, value);
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
                if (value <= PlotLowLimit) PlotLowLimit = value - 1;
                Set(ref _plotHighlimit, value);
            }
        }
        #endregion

        #region Настройка записи в график, если не в режиме измерения
        /// <summary>
        /// Настройка записи в график, если не в режиме измерения
        /// </summary>
        private bool _writeIfNoMeasState;
        /// <summary>
        /// Настройка записи в график, если не в режиме измерения
        /// </summary>
        public bool WriteIfNoMeasState
        {
            get => _writeIfNoMeasState;
            set => Set(ref _writeIfNoMeasState, value);
        }
        #endregion

    }
}
