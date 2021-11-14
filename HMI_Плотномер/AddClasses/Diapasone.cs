using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HMI_Плотномер.AddClasses
{
    class Diapasone : PropertyChangedBase, IDataErrorInfo, ICloneable
    {
        Dictionary<string, string> errorsDict = new Dictionary<string, string>();
        public string this[string columnName] => errorsDict.ContainsKey(columnName) ? errorsDict[columnName] : null;

        public string Error { get; }

        #region Номер калибровочной кривой
        ushort _calibCurveNum;
        public ushort CalibCurveNum
        {
            get => _calibCurveNum;
            set 
            {                
                if (value < 0 || value > 7) errorsDict["CalibCurveNum"] = "Номер калибровочной кривой должен быть от 0 до 7!";                
                else
                {
                   if(Set(ref _calibCurveNum, value)) errorsDict["CalibCurveNum"] = null;
                }
            } 
        }
        #endregion

        #region Номер стандартизации ЕИ
        ushort _standNum;
        public ushort StandNum
        {
            get => _standNum;
            set
            {
                if (value < 0 || value > 11) errorsDict["StandNum"] = "Номер cтандартизации должен быть от 0 до 7!";
                else
                {
                    if (Set(ref _standNum, value)) errorsDict["StandNum"] = null;

                }
            }
        }
        #endregion

        #region Номер счетчика
        ushort _counterNum;
        public ushort CounterNum
        {
            get => _counterNum;
            set => Set(ref _counterNum, value);            
        }
        #endregion

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
