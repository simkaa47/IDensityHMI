using IDensity.DataAccess;
using System;
using System.Collections.Generic;
using System.Windows;

namespace IDensity.Services.AdcServices
{
    public class CheckPulseService : PropertyChangedBase
    {

        #region Общее количество испульсов
        /// <summary>
        /// Общее количество испульсов
        /// </summary>
        private int _commontCount;
        /// <summary>
        /// Общее количество испульсов
        /// </summary>
        public int CommontCount
        {
            get => _commontCount;
            set => Set(ref _commontCount, value);
        }
        #endregion

        #region Количество импульсов с пересветом
        /// <summary>
        /// Количество импульсов с пересветом
        /// </summary>
        private int _overValuePulses;
        /// <summary>
        /// Количество импульсов с пересветом
        /// </summary>
        public int OverValuePulses
        {
            get => _overValuePulses;
            set => Set(ref _overValuePulses, value);
        }
        #endregion

        #region Количесвто шумов
        /// <summary>
        /// Количесвто шумов
        /// </summary>
        private int _noisePulses;
        /// <summary>
        /// Количесвто шумов
        /// </summary>
        public int NoisePulses
        {
            get => _noisePulses;
            set => Set(ref _noisePulses, value);
        }
        #endregion

        #region Процент удачных импульсов
        /// <summary>
        /// Процент удачных импульсов
        /// </summary>
        private double _successDeviation;
        /// <summary>
        /// Процент удачных импульсов
        /// </summary>
        public double SuccessDeviation
        {
            get => _successDeviation;
            set => Set(ref _successDeviation, value);
        }
        #endregion

        #region Количество проверяемых точек
        /// <summary>
        /// Количество проверяемых точек
        /// </summary>
        private int _checkSize = 120;
        /// <summary>
        /// Количество проверяемых точек
        /// </summary>
        public int CheckSize
        {
            get => _checkSize;
            set
            {
                if (value > 0 && value < 1000)
                    Set(ref _checkSize, value);
            }
        }
        #endregion

        #region Глубина усреднения
        /// <summary>
        /// Глубина усреднения
        /// </summary>
        private int _avgDeep = 10;
        /// <summary>
        /// Глубина усреднения
        /// </summary>
        public int AvgDeep
        {
            get => _avgDeep;
            set
            {
                Set(ref _avgDeep, value);
            }
        }
        #endregion


        public void Calculate(List<Point> arr)
        {
            Run(arr);
            Calculate();
        }
        void Run(List<Point> arr)
        {
            int overFullCount = 0;
            if (arr.Count < 1000) return;
            CommontCount++;
            double sum = 0;
            double min = 0, avgMin = 0, max = 0, avgMax = 0, actValue = 0;
            for (int i = 0; i < CheckSize; i++)
            {
                if (arr[i].Y == 4095) overFullCount++;
                if (overFullCount >= 5)
                {
                    OverValuePulses++;
                    return;
                }
                if (i < AvgDeep)
                {
                    sum = sum + arr[i].Y;
                    actValue = sum / (i + 1);
                }
                else
                {
                    sum = sum + arr[i].Y - arr[i - AvgDeep].Y;
                    actValue = sum / AvgDeep;
                }
                min = Math.Min(min, arr[i].Y);
                max = Math.Max(max, arr[i].Y);
                avgMin = Math.Min(avgMin, actValue);
                avgMax = Math.Max(avgMax, actValue);
            }
            if ((max - min) / (avgMax - avgMin) > 1.5) NoisePulses++;

        }

        void Calculate()
        {
            SuccessDeviation = (double)(CommontCount - NoisePulses - OverValuePulses) / CommontCount * 100;
        }

        public void Clear()
        {
            SuccessDeviation = 100;
            CommontCount = 0;
            NoisePulses = 0;
            OverValuePulses = 0;
        }


    }
}
