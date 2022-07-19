using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Models
{
    public class SpectrLogSettings:PropertyChangedBase
    {

        #region Частота логирования спектра, раз/мин
        /// <summary>
        /// Частота логирования спектра, раз/мин
        /// </summary>
        private int _logFreq = 60;
        /// <summary>
        /// Частота логирования спектра, раз/мин
        /// </summary>
        public int LogFreq
        {
            get => _logFreq;
            set 
            {
                if(value>=10)
                Set(ref _logFreq, value);
            } 
        }
        #endregion

        #region Циклическое логирование спектра
        /// <summary>
        /// Циклическое логирование спектра
        /// </summary>
        private bool _cyclicLog;
        /// <summary>
        /// Циклическое логирование спектра
        /// </summary>
        public bool CyclicLog
        {
            get => _cyclicLog;
            set => Set(ref _cyclicLog, value);
        }
        #endregion

        #region Чатота обнуления спектра, сек
        /// <summary>
        /// Чатота обнуления спектра, сек
        /// </summary>
        private int _freqClearSpectr = 1200;
        /// <summary>
        /// Чатота обнуления спектра, сек
        /// </summary>
        public int FreqClearSpectr
        {
            get => _freqClearSpectr;
            set 
            { 
                if(value>60)
                Set(ref _freqClearSpectr, value); 
            } 
        }
        #endregion


    }
}
