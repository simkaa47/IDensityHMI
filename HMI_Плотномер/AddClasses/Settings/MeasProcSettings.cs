using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDensity.AddClasses.Settings
{
    class MeasProcSettings:PropertyChangedBase
    {
        public MeasProcSettings(int num)
        {
            Num = (ushort)num;
        }
        #region Константы
        #region Количество стандартизаций
        /// <summary>
        /// Количество стандартизаций
        /// </summary>
        public const int StandCount = 3;
        #endregion
        #region Количество измеренных точек для построения калибровочеого полинома
        /// <summary>
        /// Количество измеренных точек для построения калибровочеого полинома
        /// </summary>
        public const int SingleMeasResCount = 10; 
        #endregion
        #endregion
        #region Номер текущего измерительного процесса
        private ushort _num;
        /// <summary>
        /// Номер текущего измерительного процесса
        /// </summary>
        public ushort Num
        {
            get { return _num; }
            set { Set(ref _num, value); }
        }
        #endregion
        #region Номер счетчика
        /// <summary>
        /// Номер счетчика
        /// </summary>
        public Parameter<ushort> MeasProcCounterNum { get; } = new Parameter<ushort>("MeasProcCounterNum", "Номер счетчика", 0, Counters.CountersNum, 0, "hold");
        #endregion
        #region Данные стандартизаций
        /// <summary>
        /// Данные стандартизаций
        /// </summary>
        public StandSettings[] MeasStandSettings { get; } = Enumerable.Range(0, StandCount).Select(i => new StandSettings(i)).ToArray();
        #endregion
        #region Массив значений еденичных измерений
        /// <summary>
        /// Массив значений еденичных измерений
        /// </summary>
        public CalibCurveCrcValue[] SingleMeasResults { get; } = Enumerable.Range(0, SingleMeasResCount)
            .Select(i => new CalibCurveCrcValue())
            .ToArray();
        #endregion
        #region Данные коэффициентов калибровочной кривой
        /// <summary>
        /// Данные коэффициентов калибровочной кривой
        /// </summary>
        public CalibrData CalibrCurve { get; } = new CalibrData();
        #endregion
        #region Плотность жидкости
        /// <summary>
        /// Плотность жидкости
        /// </summary>
        public DensitySett DensityLiq { get; } = new DensitySett();
        #endregion
        #region Плотность твердого
        /// <summary>
        /// Плотность твердого
        /// </summary>
        public DensitySett DensitySol { get; } = new DensitySett();
        #endregion
        #region Компенсация по температуре
        /// <summary>
        /// Компенсация по температуре
        /// </summary>
        public Compensation TempCompensation => new Compensation();
        #endregion
        #region Компенсация паровой фазы
        /// <summary>
        /// Компенсация паровой фазы
        /// </summary>
        public Compensation SteamCompensation => new Compensation();
        #endregion
        #region Тип измерения
        /// <summary>
        /// Тип измерения
        /// </summary>
        public Parameter<int> MeasType => new Parameter<int>("MeasType", "Тип измерения", 0, 10, 0, "");
        #endregion
        #region Быстрые изменения
        /// <summary>
        /// Быстрые изменения
        /// </summary>
        public FastChange FastChange => new FastChange();
        #endregion
        #region Длительность измерения
        /// <summary>
        /// Длительность измерения
        /// </summary>
        public Parameter<ushort> MeasDuration => new Parameter<ushort>("MeasDuration", "Длительность измерения, x0.1 c.", 0, ushort.MaxValue, 0, "");
        #endregion
        #region Глубина усреднения
        /// <summary>
        /// Глубина усреднения
        /// </summary>
        public Parameter<ushort> MeasDeep => new Parameter<ushort>("MeasDeep", "Глубина усреднения", 0, ushort.MaxValue, 0, "");
        #endregion
        #region Выходная ЕИ
        /// <summary>
        /// Выходная ЕИ
        /// </summary>
        public Parameter<ushort> OutMeasNum => new Parameter<ushort>("OutMeasNum", "Выходная ЕИ", 0, ushort.MaxValue, 0, ""); 
        #endregion

    }
}
