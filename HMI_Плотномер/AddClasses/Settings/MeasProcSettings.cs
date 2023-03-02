using IDensity.Core.AddClasses.Settings;
using IDensity.Models;
using IDensity.Services.Calibration;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Timers;

namespace IDensity.AddClasses.Settings
{
    [DataContract]
    public class MeasProcSettings : PropertyChangedBase
    {
        public MeasProcSettings(int num)
        {
            Num = (ushort)num; 
            // Действия по изменению настроек плотности
            DensityLiqD1.NeedWriteEvent += (s) => OnWriteCommandExecuted($"dens_liq={s}");
            DensitySolD2.NeedWriteEvent += (s) => OnWriteCommandExecuted($"dens_solid={s}");
            // Действия по изменению настроек компенсаций            
            SteamCompensation.NeedWriteEvent += (s) => OnWriteCommandExecuted($"comp_steam={s}");            
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
        [DataMember]
        public Parameter<ushort> MeasProcCounterNum { get; set; } = new Parameter<ushort>("MeasProcCounterNum", "Номер счетчика", 0, 2, 0, "hold");
        #endregion
        #region Данные стандартизаций
        /// <summary>
        /// Данные стандартизаций
        /// </summary>
        [DataMember]
        public StandSettings[] MeasStandSettings { get; set; } = Enumerable.Range(0, StandCount).Select(i => new StandSettings(i)).ToArray();
        #endregion
        #region Массив значений еденичных измерений
        /// <summary>
        /// Массив значений еденичных измерений
        /// </summary>
        [DataMember]
        public CalibCurveCrcValue[] SingleMeasResults { get; set; } = Enumerable.Range(0, SingleMeasResCount)
            .Select(i => new CalibCurveCrcValue(i))
            .ToArray();
        #endregion
        #region Данные коэффициентов калибровочной кривой
        /// <summary>
        /// Данные коэффициентов калибровочной кривой
        /// </summary>
        [DataMember]
        public CalibrData CalibrCurve { get; set; } = new CalibrData();
        #endregion
        #region Плотность жидкости
        /// <summary>
        /// Плотность жидкости
        /// </summary>
        [DataMember]
        public DensitySett DensityLiqD1 { get; set; } = new DensitySett();
        #endregion
        #region Плотность твердого
        /// <summary>
        /// Плотность твердого
        /// </summary>
        [DataMember]
        public DensitySett DensitySolD2 { get; set; } = new DensitySett();
        #endregion
        #region Компенсация по температуре для твердого
        /// <summary>
        /// Компенсация по температуре твердого
        /// </summary>
        [DataMember]
        public List<TempCompensation> TempCompensations { get; set; } = Enumerable.Range(0, 3).Select(i => new TempCompensation { Index = i }).ToList();
        #endregion
        #region Компенсация паровой фазы
        /// <summary>
        /// Компенсация паровой фазы
        /// </summary>
        [DataMember]
        public Compensation SteamCompensation { get; set; } = new Compensation();
        #endregion
        #region Тип измерения
        /// <summary>
        /// Тип измерения
        /// </summary>
        [DataMember]
        public Parameter<int> MeasType { get; set; } = new Parameter<int>("MeasType", "Тип измерения", 0, 10, 0, "");
        #endregion
        #region Быстрые изменения
        /// <summary>
        /// Быстрые изменения
        /// </summary>
        [DataMember]
        public FastChange FastChange { get; set; } = new FastChange();
        #endregion
        #region Длительность измерения
        /// <summary>
        /// Длительность измерения
        /// </summary>
        [DataMember]
        public Parameter<float> MeasDuration { get; set; } = new Parameter<float>("MeasDuration", "Длительность измерения, c", 0, 1000, 0, "");
        #endregion
        #region Глубина усреднения
        /// <summary>
        /// Глубина усреднения
        /// </summary>
        [DataMember]
        public Parameter<ushort> MeasDeep { get; set; } = new Parameter<ushort>("MeasDeep", "Глубина усреднения", 0, ushort.MaxValue, 0, "");
        #endregion       
        #region Диаметрт трубы
        [DataMember]
        public Parameter<float> PipeDiameter { get; set; } = new Parameter<float>("PipeDiameter", "Диаметр трубы, мм", 0, float.MaxValue, 0, "");
        #endregion
        #region Активность
        [DataMember]
        public Parameter<bool> IsActive { get; set; } = new Parameter<bool>("MeasProcActive", "Активность измерительного процесса", false, true, 0, "hold");
        #endregion
        #region К-ты ослабления
        [DataMember]
        public List<Parameter<float>> AttCoeffs { get; set; } = Enumerable.Range(0, 2).Select(i => new Parameter<float>($"MeasProcAttCoeff{i}", $"К-т ослабления {i}", float.MinValue, float.MaxValue, 0, "")).ToList();
        #endregion
        #region К-ты расчета обьема
        [DataMember]
        public List<Parameter<float>> VolumeCoeefs { get; set; } = Enumerable.Range(0, 4).Select(i => new Parameter<float>($"VolumeCoeff{i}", $"К-т расчета обьема {i}", float.MinValue, float.MaxValue, 0, "")).ToList();
        #endregion               

        void OnWriteCommandExecuted(string argument)
        {
            NeedWriteEvent?.Invoke($"*SETT,meas_proc={Num},{argument}#", Num);
        }
        /// <summary>
        /// Необходимо записать настройки измерительных процессов
        /// </summary>
        public event Action<string, ushort> NeedWriteEvent;  

        #region Тип расчета
        [DataMember]
        public Parameter<ushort> CalculationType { get; set; } = new Parameter<ushort>("CalculationType", "Тип расчета", 0, 3, 0, "");
        #endregion        

    }
}
