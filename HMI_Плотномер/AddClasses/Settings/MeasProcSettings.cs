using IDensity.Models;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;

namespace IDensity.AddClasses.Settings
{
    class MeasProcSettings:PropertyChangedBase
    {
        public MeasProcSettings(int num)
        {
            Num = (ushort)num;
            // Действие по измерению значения счетчика
            MeasProcCounterNum.CommandEcecutedEvent += (o) => OnWriteCommandExecuted($"cntr={MeasProcCounterNum.WriteValue}");
            // Подписка на события настроек стандартизаций
            foreach (var std in MeasStandSettings)
            {
                std.NeedWriteEvent += s => OnWriteCommandExecuted(s);
                std.NeedMakeStand += () => NeedMakeStand?.Invoke(Num, (ushort)std.Id);
                std.StandFinishEvent += () => StandFinishEvent?.Invoke(Num);
            }
            // Подписка на события настроек данных еденичных измерений
            foreach (var src in SingleMeasResults)
            {
                src.NeedWriteEvent += (arg, i) => 
                {
                    var str = "calib_src=";
                    for (int j = 0; j < SingleMeasResCount; j++)
                    {
                        if (j != i) str += $"{SingleMeasResults[j].Date.Value.ToString("dd:MM:yy")},{SingleMeasResults[j].Weak.Value.ToStringPoint()},{SingleMeasResults[j].CounterValue.Value.ToStringPoint()}";
                        else str += arg;
                        if (j < SingleMeasResCount - 1) str += ",";
                    }
                    OnWriteCommandExecuted(str);
                };
            }
            // Подписка на собтия записи данных калибровочгных кривых
            CalibrCurve.NeedWriteEvent += OnWriteCommandExecuted;
            //Действие по изменению продолжительности и глубины измерения
            MeasDuration.CommandEcecutedEvent += (o) => OnWriteCommandExecuted($"duration={MeasDuration.WriteValue*10}");
            MeasDeep.CommandEcecutedEvent += (o) => OnWriteCommandExecuted($"aver_depth={MeasDeep.WriteValue}");
            // Действия по изменению настроек плотности
            DensityLiq.NeedWriteEvent+=(s)=> OnWriteCommandExecuted($"dens_liq={s}");
            DensitySol.NeedWriteEvent += (s) => OnWriteCommandExecuted($"dens_solid={s}");
            // Действия по изменению настроек компенсаций
            TempCompensation.NeedWriteEvent += (s) => OnWriteCommandExecuted($"comp_temp={s}");
            SteamCompensation.NeedWriteEvent += (s) => OnWriteCommandExecuted($"comp_steam={s}");
            // Подписка на изменение типа измерения
            MeasType.CommandEcecutedEvent += (o) => OnWriteCommandExecuted($"type={MeasType.WriteValue}");
            // Подписка на изменение настроек быстрых измерений
            FastChange.NeedWriteEvent+= OnWriteCommandExecuted;
            //Подписка на измерение диаметра трубы
            PipeDiameter.CommandEcecutedEvent+=(o)=>OnWriteCommandExecuted($"pipe_diam={(ushort)(PipeDiameter.WriteValue*10)}");
            // Настройка таймера
            singleMeasTimer.Elapsed += (o, e) =>
            {                
                if (--SingleMeasTimeLeft <= 0)
                {
                    singleMeasTimer?.Stop();
                    SingleMeasFlag = false;
                    SingleMeasEventFinishedEvent?.Invoke(Num);
                } 
            };
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
        public Parameter<ushort> MeasProcCounterNum { get; } = new Parameter<ushort>("MeasProcCounterNum", "Номер счетчика", 0, 8, 0, "hold");
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
            .Select(i => new CalibCurveCrcValue(i))
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
        public Compensation TempCompensation { get; } = new Compensation();
        #endregion
        #region Компенсация паровой фазы
        /// <summary>
        /// Компенсация паровой фазы
        /// </summary>
        public Compensation SteamCompensation { get; } = new Compensation();
        #endregion
        #region Тип измерения
        /// <summary>
        /// Тип измерения
        /// </summary>
        public Parameter<int> MeasType { get; } = new Parameter<int>("MeasType", "Тип измерения", 0, 10, 0, "");
        #endregion
        #region Быстрые изменения
        /// <summary>
        /// Быстрые изменения
        /// </summary>
        public FastChange FastChange { get; } = new FastChange();
        #endregion
        #region Длительность измерения
        /// <summary>
        /// Длительность измерения
        /// </summary>
        public Parameter<float> MeasDuration { get; } = new Parameter<float>("MeasDuration", "Длительность измерения, c", 0, 1000, 0, "");
        #endregion
        #region Глубина усреднения
        /// <summary>
        /// Глубина усреднения
        /// </summary>
        public Parameter<ushort> MeasDeep { get; } = new Parameter<ushort>("MeasDeep", "Глубина усреднения", 0, ushort.MaxValue, 0, "");
        #endregion
        #region Выходная ЕИ
        /// <summary>
        /// Выходная ЕИ
        /// </summary>
        //public Parameter<ushort> OutMeasNum { get; } = new Parameter<ushort>("OutMeasNum", "Выходная ЕИ", 0, ushort.MaxValue, 0, "");
        private MeasUnitSettings _outMeasNum;
        public MeasUnitSettings OutMeasNum
        {
            get => _outMeasNum;
            set => Set(ref _outMeasNum, value);
        }
        private RelayCommand _outMeasNumWriteCommand;

        public RelayCommand OutMeasNumWriteCommand => _outMeasNumWriteCommand ?? (_outMeasNumWriteCommand = new RelayCommand(par => OnWriteCommandExecuted($"ei={OutMeasNum.Id.Value}"), o => true));

        #endregion
        #region Диаметрт трубы
        public Parameter<float> PipeDiameter { get; } = new Parameter<float>("PipeDiameter", "Диаметр трубы, мм", 0, float.MaxValue, 0, "");
        #endregion
        #region Активность
        public Parameter<bool> IsActive { get; } = new Parameter<bool>("MeasProcActive", "Активность измерительного процесса", false, true, 0, "hold");
        #endregion

        #region Настройки единичного измерения
        Timer singleMeasTimer = new Timer();
        #region Время единичного измерния
        public Parameter<ushort> SingleMeasTime { get; } = new Parameter<ushort>("SingleMeasTime", "Время еденичного измерения, c.", 1, ushort.MaxValue, 0, "");
        #endregion

        #region Осталось времени еденичного измерения
        private int _singleMeasTimeLeft;

        public int SingleMeasTimeLeft
        {
            get { return _singleMeasTimeLeft; }
            set { Set(ref _singleMeasTimeLeft, value); }
        }

        #endregion

        #region Степень полинома
        private int _singleMeasDeg = 1;
        public int SingleMeasDeg
        {
            get { return _singleMeasDeg; }
            set
            {
                Calibration.PolDegree = value;
                Set(ref _singleMeasDeg, Calibration.PolDegree);
            }
        }
        #endregion

        #region ФВ
        private float _singleMeasPhysValue;

        public float SingleMeasPhysValue
        {
            get { return _singleMeasPhysValue; }
            set { Set(ref _singleMeasPhysValue, value); }
        }
        #endregion

        #region Флаг измерения
        private bool _singleMeasFlag;

        public bool SingleMeasFlag
        {
            get { return _singleMeasFlag; }
            set { Set(ref _singleMeasFlag, value); }
        }

        #endregion

        #region Команда - произвести еденичное измерение
        private RelayCommand _singleMeasCommand;

        public RelayCommand SingleMeasCommand
        {
            get
            {
                return _singleMeasCommand ?? (_singleMeasCommand = new RelayCommand(par => 
                {
                    if (!SingleMeasFlag)
                    {
                        NeedMakeSingleMeasEvent?.Invoke(SingleMeasTime.Value, Num, SingleMeasIndex);                        
                        singleMeasTimer.Interval = 1000;                       
                        singleMeasTimer.Start();
                        SingleMeasFlag = true;
                        SingleMeasTimeLeft = SingleMeasTime.Value/10 + 4; 
                    }

                }, can => IsActive.Value));
            }
        }

        #endregion

        #region Выбранная ячейка для записи
        private byte _singleMeasIndex;

        public byte SingleMeasIndex
        {
            get { return _singleMeasIndex; }
            set { Set(ref _singleMeasIndex, value); }
        }
        #endregion



        #endregion

        #region Расчет к-тов полинома
        RelayCommand _calculatePolinomCommand;
        public RelayCommand CalculatePolinomCommand => _calculatePolinomCommand ?? (_calculatePolinomCommand = new RelayCommand(par => 
        {
            var measPoints = SingleMeasResults.Where(smr => smr.Selected)
                .Select(smr => new Point(smr.Weak.Value, smr.CounterValue.Value))
                .ToList();
            if (measPoints.Count > 0)
            {
                CalculatedCoeefs.Clear();
                var result = Calibration.GetCoeffs(measPoints);
                for (int i = 0; i < result.Count; i++)
                {
                    CalculatedCoeefs.Add((new CalibrationCoeff(i,result[i])));
                }
            }

        }, o => true));

        public ObservableCollection<CalibrationCoeff> CalculatedCoeefs { get; } = new ObservableCollection<CalibrationCoeff>();

        #endregion

        #region Команда посчитать график для проверки полинома
        RelayCommand _showPolinomTrend;
        public RelayCommand ShowPolinomTrendCommand => _showPolinomTrend ?? (_showPolinomTrend = new RelayCommand(par =>
        {
            var measList = SingleMeasResults.Where(sm => sm.Selected).
            OrderBy(sm => sm.Weak.Value).Select(sm => new Point(sm.Weak.Value, sm.CounterValue.Value)).ToList();
            if (measList.Count>=2)
            {
                var startWeak = measList[0].X;
                var finishWeak = measList[measList.Count - 1].X;
                if (startWeak != finishWeak)
                {
                    int cnt = 50;
                    double diff = (finishWeak - startWeak) / cnt;
                    var calcList = Enumerable.Range(0, cnt).
                    Select(i => new Point(startWeak + i * diff, GetPhysvalueByWeak(startWeak + i * diff))).ToList();
                    MeasuredPointsCollection = measList;
                    CalculatedMeasCollection = calcList;
                } 
            }
        }, canExecPar => true));

        double GetPhysvalueByWeak(double weak)
        {
            double result = 0;
            for (int i = 0; i < CalculatedCoeefs.Count; i++)
            {
                result += (Math.Pow(weak, i) * CalculatedCoeefs[i].Coeff);
            }
            return result;
        }

        #region Коллекция измеренных значений для тренда
        private List<Point> _measuredPointsCollection;

        public List<Point> MeasuredPointsCollection
        {
            get { return _measuredPointsCollection; }
            set { Set(ref _measuredPointsCollection, value); }
        }
        #endregion

        #region Коллекция рассичтанных значений для тренда
        private List<Point> _сalculatedMeasCollection;

        public List<Point> CalculatedMeasCollection
        {
            get { return _сalculatedMeasCollection; }
            set { Set(ref _сalculatedMeasCollection, value); }
        }
        #endregion


        #endregion

        #region КОманда записать рассчитанные к-ты в память
        private RelayCommand _writeCalibrCoeefsCommand;

        public RelayCommand WriteCalibrCoeefsCommand => _writeCalibrCoeefsCommand ?? (_writeCalibrCoeefsCommand = new RelayCommand(par => 
        {
            var arg = $"calib_curve={CalibrCurve.Type.Value},{CalibrCurve.MeasUnit.Id.Value}";
            for (int i = 0; i < 6; i++)
            {
                arg += "," + (i < CalculatedCoeefs.Count ? ((float)CalculatedCoeefs[i].Coeff).ToStringPoint() : "0");
            }
            OnWriteCommandExecuted(arg);

        }, o => true));

        #endregion


        #region Команда - скопировать настройки на другой измерительный процесс
        /// <summary>
        /// Команда - скопировать настройки на другой измерительный процесс
        /// </summary>
        RelayCommand _copyAllCommand;
        /// <summary>
        /// Команда - скопировать настройки на другой измерительный процесс
        /// </summary>
        public RelayCommand CopyAllCommand => _copyAllCommand ?? (_copyAllCommand = new RelayCommand(execPar => 
        {
            int par = (int)execPar;
            var arg = CopyAll(par);
            if(par!=Num) NeedWriteEvent?.Invoke($"*SETT,meas_proc={par},{arg}#", (ushort)par);

        }, canExecPar => true));
        #endregion


        void OnWriteCommandExecuted(string argument)
        {
            NeedWriteEvent?.Invoke($"*SETT,meas_proc={Num},{argument}#", Num);
        }
        /// <summary>
        /// Необходимо записать настройки измерительных процессов
        /// </summary>
        public event Action<string,ushort> NeedWriteEvent;

        /// <summary>
        /// Необходимо произвести стандартизацию
        /// </summary>
        public event Action<ushort, ushort> NeedMakeStand;
        /// <summary>
        /// Стандартизация закончена
        /// </summary>
        public event Action<ushort> StandFinishEvent;

        /// <summary>
        /// Необходимо произвести еденичное измерение
        /// </summary>
        public event Action<int,ushort,ushort> NeedMakeSingleMeasEvent;

        /// <summary>
        /// Закончилось ЕИ
        /// </summary>
        public event Action<ushort> SingleMeasEventFinishedEvent;

        public string CopyAll(int number)
        {
            var str = $"cntr={MeasProcCounterNum.Value},";
            foreach (var std in MeasStandSettings)
            {
                str += std.Copy()+",";
            }
            str += CalibrCurve.Copy() + ",";
            str += "calib_src=";
            for (int j = 0; j < SingleMeasResCount; j++)
            {
                str += $"{SingleMeasResults[j].Date.Value.ToString("dd:MM:yy")},{SingleMeasResults[j].Weak.Value.ToStringPoint()},{SingleMeasResults[j].CounterValue.Value.ToStringPoint()},";
                
            }
            str += $"dens_liq={DensityLiq.Copy()},dens_solid={DensitySol.Copy()},";
            str += $"comp_temp={TempCompensation.Copy()},comp_steam={SteamCompensation.Copy()},";
            str += $"aver_depth={MeasDeep.Value},";
            str += $"type={MeasType.Value},";
            str += FastChange.Copy()+",";
            str += $"pipe_diam={(ushort)(PipeDiameter.Value * 10)}";
            return str;
        }

    }
}
