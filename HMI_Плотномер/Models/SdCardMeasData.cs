using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDensity.Models
{
    public class SdCardMeasData:PropertyChangedBase
    {



        #region Дата и время измерения
        /// <summary>
        /// Дата и время измерения
        /// </summary>
        private DateTime _time;
        /// <summary>
        /// Дата и время измерения
        /// </summary>
        public DateTime Time
        {
            get => _time;
            set => Set(ref _time, value);
        }
        #endregion

        #region Данные измерений
        /// <summary>
        /// Данные измерений
        /// </summary>
        public SdMeasResult[] MeasResults { get; } = Enumerable.Range(0,2).Select(i=>new SdMeasResult()).ToArray();
        #endregion

        #region Температура внутреннего датчика
        /// <summary>
        /// Температура внутреннего датчика
        /// </summary>
        private float _tempInt;
        /// <summary>
        /// Температура внутреннего датчика
        /// </summary>
        public float TempInt
        {
            get => _tempInt;
            set => Set(ref _tempInt, value);
        }
        #endregion

        #region HV - входное напряжение, вольт
        /// <summary>
        /// HV - входное напряжение, вольт
        /// </summary>
        private float _hvInput;
        /// <summary>
        /// HV - входное напряжение, вольт
        /// </summary>
        public float HvInput
        {
            get => _hvInput;
            set => Set(ref _hvInput, value);
        }
        #endregion

        #region HV - выходное напряжение, вольт
        /// <summary>
        /// HV - выходное напряжение, вольт
        /// </summary>
        private float _hvOutU;
        /// <summary>
        /// HV - выходное напряжение, вольт
        /// </summary>
        public float HvOutU
        {
            get => _hvOutU;
            set => Set(ref _hvOutU, value);
        }
        #endregion

        #region HV - ток
        /// <summary>
        /// HV - ток
        /// </summary>
        private float _hvCurrent;
        /// <summary>
        /// HV - ток
        /// </summary>
        public float HvCurrent
        {
            get => _hvCurrent;
            set => Set(ref _hvCurrent, value);
        }
        #endregion

        #region Телеметрия аналогов
        /// <summary>
        /// Телеметрия аналогов
        /// </summary>
        public SdAnalogData[] AnalogData { get; } = Enumerable.Range(0, 2).Select(i => new SdAnalogData()).ToArray();

        #endregion

        #region Значение состояний физических параметров
        /// <summary>
        /// Значение состояний физических параметров
        /// </summary>
        private ushort _physParamState;
        /// <summary>
        /// Значение состояний физических параметров
        /// </summary>
        public ushort PhysParamState
        {
            get => _physParamState;
            set => Set(ref _physParamState, value);
        }
        #endregion

        #region Значение состояний связи
        /// <summary>
        /// Значение состояний связи
        /// </summary>
        private ushort _commState;
        /// <summary>
        /// Значение состояний связи
        /// </summary>
        public ushort CommState
        {
            get => _commState;
            set => Set(ref _commState, value);
        }
        #endregion

    }
    public class SdMeasResult : PropertyChangedBase
    {
        #region Номер процесса
        /// <summary>
        /// Номер процесса
        /// </summary>
        private int _procNum;
        /// <summary>
        /// Номер процесса
        /// </summary>
        public int ProcNum
        {
            get => _procNum;
            set => Set(ref _procNum, value);
        }
        #endregion

        #region Значение счетчика
        /// <summary>
        /// Значение счетчика
        /// </summary>
        private int _counterValue;
        /// <summary>
        /// Значение счетчика
        /// </summary>
        public int CounterValue
        {
            get => _counterValue;
            set => Set(ref _counterValue, value);
        }
        #endregion

        #region Мгновенное значение ФВ
        /// <summary>       
        /// Мгновенное значение ФВ
        /// </summary>
        private float _physValueCur;
        /// <summary>
        /// Мгновенное значение ФВ
        /// </summary>
        public float PhysValueCur
        {
            get => _physValueCur;
            set => Set(ref _physValueCur, value);
        }
        #endregion

        #region Усредненное значение ФВ
        /// <summary>
        /// Усредненное значение ФВ
        /// </summary>
        private float _phusValueAvg;
        /// <summary>
        /// Усредненное значение ФВ
        /// </summary>
        public float PhusValueAvg
        {
            get => _phusValueAvg;
            set => Set(ref _phusValueAvg, value);
        }
        #endregion

        #region Активность  измерительного процесса
        /// <summary>
        /// Активность  измерительного процесса
        /// </summary>
        private bool _isActive;
        /// <summary>
        /// Активность  измерительного процесса
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => Set(ref _isActive, value);
        }
        #endregion
        
    }

    public class SdAnalogData : PropertyChangedBase
    {

        #region Аналоговый выход - значение ЦАП
        /// <summary>
        /// Аналоговый выход - значение ЦАП
        /// </summary>
        private int _dac;
        /// <summary>
        /// Аналоговый выход - значение ЦАП
        /// </summary>
        public int Dac
        {
            get => _dac;
            set => Set(ref _dac, value);
        }
        #endregion

        #region Аналоговый выход  - значение RX
        /// <summary>
        /// Аналоговый выход  - значение RX
        /// </summary>
        private int _rx;
        /// <summary>
        /// Аналоговый выход  - значение RX
        /// </summary>
        public int Rx
        {
            get => _rx;
            set => Set(ref _rx, value);
        }
        #endregion

        #region Аналоговый выход - значение test
        /// <summary>
        /// Аналоговый выход - значение test
        /// </summary>
        private int _test;
        /// <summary>
        /// Аналоговый выход - значение test
        /// </summary>
        public int test
        {
            get => _test;
            set => Set(ref _test, value);
        }
        #endregion

        #region Аналоговый вход - значение АЦП
        /// <summary>
        /// Аналоговый вход - значение АЦП
        /// </summary>
        private int _adc;
        /// <summary>
        /// Аналоговый вход - значение АЦП
        /// </summary>
        public int Adc
        {
            get => _adc;
            set => Set(ref _adc, value);
        }
        #endregion


    }

}
