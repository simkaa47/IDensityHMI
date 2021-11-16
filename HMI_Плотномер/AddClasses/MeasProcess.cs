using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HMI_Плотномер.AddClasses
{
    class MeasProcess : PropertyChangedBase, IDataErrorInfo, ICloneable
    {
        Dictionary<string, string> errorsDict = new Dictionary<string, string>();
        public string this[string columnName] => errorsDict[columnName];

        public string Error { get; }
        #region Количество диапазонов в процессе
        /// <summary>
        /// Количество диапазонов в процессе
        /// </summary>
        public static int rangeNum = 3;
        #endregion

        #region Сведения о диапазонах
        public Diapasone[] Ranges { get; set; } = Enumerable.Range(0, rangeNum).Select(z => new Diapasone()).ToArray();
        #endregion

        #region Номер фоновой стандартизации
        ushort _backStandNum;
        public ushort BackStandNum
        {
            get => _backStandNum;
            set
            {
                if (value < 0 || value > 3) errorsDict["BackStandNum"] = "Номер стандартизации должен быть от 0 до 3!";
                else
                {
                    if (Set(ref _backStandNum, value)) errorsDict["BackStandNum"] = null;

                }
            }
        }
        #endregion

        #region Длительность измерения одной точки
        ushort _measDuration;
        public ushort MeasDuration
        {
            get => _measDuration;
            set
            {
                if (value < 1) errorsDict["MeasDuration"] = "Длительность измерения должна быть больше 0!";
                else
                {
                    if (Set(ref _measDuration, value)) errorsDict["MeasDuration"] = null;

                }
            }            
        }
        #endregion

        #region Глубина усреднения
        ushort _measDeep;
        public ushort MeasDeep
        {
            get => _measDeep;
            set
            {
                if (value < 1) errorsDict["MeasDeep"] = "Глубина измерения должна быть больше 0!";
                else
                {
                    if (Set(ref _measDeep, value)) errorsDict["MeasDeep"] = null;

                }
            }
        }
        #endregion

        #region Период полупаспада
        float _halfLife;
        public float HalfLife { 
            get => _halfLife; 
            set => Set(ref _halfLife, value); }
        #endregion

        #region Плотность жидкости
        float _densityLiquid;
        public float DensityLiquid { get => _densityLiquid; set => Set(ref _densityLiquid, value); }
        #endregion

        #region Плотность твердости
        float _densitySolid;
        public float DensitySolid { get => _densitySolid; set => Set(ref _densitySolid, value); }
        #endregion



        public object Clone()
        {
            return new MeasProcess
            {
                Ranges = this.Ranges.Select(r => (Diapasone)r.Clone()).ToArray(),
                BackStandNum = this.BackStandNum,
                MeasDuration = this.MeasDuration,
                MeasDeep = this.MeasDeep,
                HalfLife = this.HalfLife,
                DensityLiquid = this.DensityLiquid,
                DensitySolid = this.DensitySolid
            };
        }
    }
}
