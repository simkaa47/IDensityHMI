using IDensity.AddClasses;
using IDensity.Models;
using IDensity.Services.Calibration;
using IDensity.ViewModels;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Timers;

namespace IDensity.ViewModels
{
    public class CalibrationVm:PropertyChangedBase
    {
        private readonly VM _vM;

        public CalibrationVm(VM vM)
        {
            _vM = vM;
            // Настройка таймера
            singleMeasTimer.Elapsed += (o, e) =>
            {
                if (--SingleMeasTimeLeft <= 0)
                {
                    singleMeasTimer?.Stop();
                    SingleMeasFlag = false;
                    _vM.CommService.Tcp.GetMeasSettingsExternal(_vM.MeasProcessVm.SelectedProcess.Num);                    
                }
            };
        }

		CalibrationService _calibrationService = new CalibrationService();

        #region ID ЕИ
        private string _measUnitMemoryId = "SingleMeasMemoryId";
        public string MeasUnitMemoryId
        {
            get => _measUnitMemoryId;
            set => Set(ref _measUnitMemoryId, value);
        }
        #endregion

        Timer singleMeasTimer = new Timer();

        #region Single Meas Time
        /// <summary>
        /// Single Meas Time
        /// </summary>
        private int _singleMeasTime = 30;
		/// <summary>
		/// Single Meas Time
		/// </summary>
		public int SingleMeasTime
		{
			get => _singleMeasTime;
			set 
			{
               if(value>0)Set(ref _singleMeasTime, value);
            } 
		}
		#endregion

		#region Степень полинома
		/// <summary>
		/// Степень полинома
		/// </summary>
		private int _polinomDeg = 1;
		/// <summary>
		/// Степень полинома
		/// </summary>
		public int PolinomDeg
		{
			get => _polinomDeg;
            set
            {
                CalibrationService.PolDegree = value;
                Set(ref _polinomDeg, CalibrationService.PolDegree);
            }
        }
        #endregion

        #region Осталось времени еденичного измерения
        private int _singleMeasTimeLeft;

        public int SingleMeasTimeLeft
        {
            get { return _singleMeasTimeLeft; }
            set { Set(ref _singleMeasTimeLeft, value); }
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
                        _vM.CommService.MakeSingleMeasure(SingleMeasTime, _vM.MeasProcessVm.SelectedProcess.Num, SingleMeasIndex);
                        singleMeasTimer.Interval = 1000;
                        singleMeasTimer.Start();
                        SingleMeasFlag = true;
                        SingleMeasTimeLeft = SingleMeasTime / 10 + 4;
                    }

                }, can => _vM.MeasProcessVm.SelectedProcess.IsActive.Value));
            }
        }

        #endregion

        #region Расчет к-тов полинома
        RelayCommand _calculatePolinomCommand;
        public RelayCommand CalculatePolinomCommand => _calculatePolinomCommand ?? (_calculatePolinomCommand = new RelayCommand(par =>
        {
            var proc = _vM.MeasProcessVm.SelectedProcess;
            try
            {
                var measPoints = proc.SingleMeasResults.Where(smr => smr.Selected)
                                 .Select(smr => new Point(smr.Weak.Value, smr.CounterValue.Value))
                                 .ToList();
                if (measPoints.Count > 0)
                {
                    CalculatedCoeefs.Clear();
                    var result = CalibrationService.GetCoeffs(measPoints);
                    for (int i = 0; i < result.Count; i++)
                    {
                        CalculatedCoeefs.Add((new CalibrationCoeff(i, result[i])));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }, o => true));

        

        #endregion

        #region Команда посчитать график для проверки полинома
        RelayCommand _showPolinomTrend;
        public RelayCommand ShowPolinomTrendCommand => _showPolinomTrend ?? (_showPolinomTrend = new RelayCommand(par =>
        {
            var proc = _vM.MeasProcessVm.SelectedProcess;
            var measList = proc.SingleMeasResults.Where(sm => sm.Selected).
            OrderBy(sm => sm.Weak.Value).Select(sm => new Point(sm.Weak.Value, sm.CounterValue.Value)).ToList();
            if (measList.Count >= 2)
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

        public ObservableCollection<CalibrationCoeff> CalculatedCoeefs { get; } = new ObservableCollection<CalibrationCoeff>();

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
            var proc = _vM.MeasProcessVm.SelectedProcess;
            var arg = $"*SETT,meas_proc={proc.Num},calib_curve={proc.CalibrCurve.Type.Value},0";
            for (int i = 0; i < 6; i++)
            {
                arg += "," + (i < CalculatedCoeefs.Count ? ((float)CalculatedCoeefs[i].Coeff).ToStringPoint() : "0");
            }
            arg += "#";
            _vM.CommService.Tcp.WriteMeasProcSettings(arg, proc.Num);

        }, o => true));

        #endregion 

        #region Выбранная ячейка для записи
        private byte _singleMeasIndex;

        public byte SingleMeasIndex
        {
            get { return _singleMeasIndex; }
            set { Set(ref _singleMeasIndex, value); }
        }
        #endregion
    }
}
