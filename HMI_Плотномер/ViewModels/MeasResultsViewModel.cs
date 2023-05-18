using IDensity.Core.Models.Trends;
using IDensity.DataAccess;
using IDensity.DataAccess.Models;
using IDensity.DataAccess.Repositories;
using IDensity.Services.InitServices;
using IDensity.ViewModels;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using IDensity.ViewModels.Commands;
using System.Windows.Controls;
using IDensity.Models;

namespace IDensity.Core.ViewModels
{
    public class MeasResultsViewModel:PropertyChangedBase
    {
        #region Репозитории
        private readonly EfRepository<MeasResultLog> _measResultRepository = new EfRepository<MeasResultLog>();
        private readonly EfRepository<MeasResultViewSett> _measResultViewRepository = new EfRepository<MeasResultViewSett>();
        #endregion

        public MeasResultsViewModel(VM vM)
        {
            VM = vM;
            Init();
        }

        public VM VM { get; }

        private void Init()
        {
            VM.CommService.UpdateDataEvent += AddDataToCollection;
            _trendSettings = XmlInit.ClassInit<GraphSettings>();
            _trendsVisible = XmlInit.ClassInit<TrendVisible>();
            InitViewSettings();
            GetMeasDates();
        }

        private void InitViewSettings()
        {
            MeasResultSetts = _measResultViewRepository.Init(new List<MeasResultViewSett>
            {
                new MeasResultViewSett() {AvgVisibility = true, CurVisibility = true},
                new MeasResultViewSett() {AvgVisibility = true, CurVisibility = true}
            });
            foreach (var sett in MeasResultSetts)
            {
                sett.PropertyChanged += (o, e) => 
                {
                    var edited = o as MeasResultViewSett;
                    _measResultViewRepository.Update(edited);
                };
            }
        }

        #region Команды
        #region Команда плказать архивный тренд
        RelayCommand _showArchivalTrendCommand;
        public RelayCommand ShowArchivalTrendCommand { get => _showArchivalTrendCommand ?? (_showArchivalTrendCommand = new RelayCommand(o => ShowArchivalTrend(), o => true)); }
        #endregion
        #region Команда "Записать архивный тренд в файл"
        RelayCommand _writeLogCommand;
        public RelayCommand WriteLogCommand { get => _writeLogCommand ?? (_writeLogCommand = new RelayCommand(o => WriteArchivalTrendToText(), o => true)); }
        #endregion
        #endregion

        #region Настройки видимости результатов измерения
        /// <summary>
        /// Настройки видимости результатов измерения
        /// </summary>
        private IEnumerable<MeasResultViewSett> _measResultSetts;
        /// <summary>
        /// Настройки видимости результатов измерения
        /// </summary>
        public IEnumerable<MeasResultViewSett> MeasResultSetts
        {
            get => _measResultSetts;
            set => Set(ref _measResultSetts, value);
        }
        #endregion


        #region Настройки тренда
        GraphSettings _trendSettings;
        public GraphSettings TrendSettings => _trendSettings;
        #endregion

        #region Данные для текущего тренда
        ObservableCollection<MeasResultLog> _plotCollection;
        public ObservableCollection<MeasResultLog> PlotCollection
        {
            get => _plotCollection ?? (_plotCollection = new ObservableCollection<MeasResultLog>());
        }
        #endregion

        #region Настройки видимости графиков
        private TrendVisible _trendsVisible;
        public TrendVisible TrendsVisible => _trendsVisible;
        #endregion

        #region Данные для архивного тренда
        #region Коллекция
        IEnumerable<MeasResultLog> _archivalDataPotnts = new List<MeasResultLog> { new MeasResultLog { Time = new DateTime(2020, 1, 1) }, new MeasResultLog { Time = DateTime.Now } };
        public IEnumerable<MeasResultLog> ArchivalDataPotnts { get => _archivalDataPotnts; private set { Set(ref _archivalDataPotnts, value); } }
        #endregion
        #region Стартовая точка отображаемого тренда
        DateTime _displayDateStart = DateTime.Today.AddDays(-1);
        public DateTime DisplayDateStart
        {
            get => _displayDateStart;
            set
            {
                Set(ref _displayDateStart, value);
                if (value >= DisplayDateEnd) DisplayDateEnd = value.AddMinutes(1);
                if (value.AddDays(2) < DisplayDateEnd) DisplayDateEnd = value.AddDays(2);
            }
        }
        #endregion
        #region Конечная точка отображаемого тренда
        DateTime _displayDateEnd = DateTime.Today;
        public DateTime DisplayDateEnd
        {
            get => _displayDateEnd;
            set
            {
                Set(ref _displayDateEnd, value);
                if (value <= DisplayDateStart) DisplayDateStart = value.AddMinutes(-1);
                if (value.AddDays(-2) > DisplayDateStart) DisplayDateStart = value.AddDays(-2);
            }
        }
        #endregion
        #region Путь к логируемому файлу
        string _logPath = "Выберите файл";
        public string LogPath { get => _logPath; set => Set(ref _logPath, value); }
        #endregion
         #region Список дат, когда происходили измерения
        List<DateTime> MeasDates { get; set; } = new List<DateTime>();
        void GetMeasDates()
        {
            MeasDates = _measResultRepository
                .GetAll().Select(m => m.Time.Date)
                .Distinct()
                .ToList();
        }
        #endregion
        #region Состояние загрузки из ДБ
        bool _archivalTrendDownloading;
        public bool ArchivalTrendDownloading { get => _archivalTrendDownloading; private set { Set(ref _archivalTrendDownloading, value); } }
        #endregion
        #region Состояние загрузки в текстовый файл
        bool _archivalTrendUploading;
        public bool ArchivalTrendUploading { get => _archivalTrendUploading; private set { Set(ref _archivalTrendUploading, value); } }
        #endregion
        #endregion

        #region Вывести данные из БД
        async void ShowArchivalTrend()
        {
            try
            {
                ArchivalTrendDownloading = true;
                await Task.Run(() =>
                {
                    var list = _measResultRepository.GetWhere(m => m.Time >= DisplayDateStart && m.Time <= DisplayDateEnd);
                    for (int j = 0; j < 2; j++)
                    {
                        var unit = GetCoeff(VM.mainModel.MeasResults[j].MeasUnitMemoryId);
                        foreach (var point in list)
                        {
                            if (j == 0)
                            {
                                point.CurValue1 = point.CurValue1 * unit.K + unit.Offset;
                                point.AvgValue1 = point.AvgValue1 * unit.K + unit.Offset;
                            }
                            else
                            {
                                point.CurValue2 = point.CurValue2 * unit.K + unit.Offset;
                                point.AvgValue2 = point.AvgValue2 * unit.K + unit.Offset;
                            }
                        }
                        MeasResultMeasUnits[j] = unit;
                    }
                    ArchivalDataPotnts = list;
                    ArchivalTrendDownloading = false;
                });

            }
            catch (Exception)
            {

            }
        }
        #endregion        

        #region Добавление данных в график
        void AddDataToCollection()
        {
            var mainModel = VM.mainModel;
            if (!(mainModel.CycleMeasStatus.Value || TrendSettings.WriteIfNoMeasState)) return;
            App.Current?.Dispatcher?.Invoke(() =>
            {
                var tp = new MeasResultLog
                {
                    Time = DateTime.Now,
                    Pulses = mainModel.MeasResults[0].CounterValue.Value,
                    CurValue1 = mainModel.MeasResults[0].PhysValueCur.Value,
                    AvgValue1 = mainModel.MeasResults[0].PhysValueAvg.Value,
                    CurValue2 = mainModel.MeasResults[1].PhysValueCur.Value,
                    AvgValue2 = mainModel.MeasResults[1].PhysValueAvg.Value,
                    Current1 = mainModel.AnalogGroups[0].AI.AdcValue.Value,
                    Current2 = mainModel.AnalogGroups[1].AI.AdcValue.Value,
                    HvValue1 = mainModel.TelemetryHV.VoltageCurOut.Value,
                    Temperature = mainModel.TempTelemetry.TempInternal.Value
                };
                _measResultRepository.Add(tp);
                tp.CurValue1 = mainModel.MeasResults[0].PhysValueCur.Value * MeasResultMeasUnits[0].K + MeasResultMeasUnits[0].Offset;
                tp.CurValue2 = mainModel.MeasResults[0].PhysValueAvg.Value * MeasResultMeasUnits[0].K + MeasResultMeasUnits[0].Offset;
                tp.AvgValue1 = mainModel.MeasResults[1].PhysValueCur.Value * MeasResultMeasUnits[1].K + MeasResultMeasUnits[1].Offset;
                tp.AvgValue2 = mainModel.MeasResults[1].PhysValueAvg.Value * MeasResultMeasUnits[1].K + MeasResultMeasUnits[1].Offset;
                PlotCollection.Add(tp);
                int i = 0;
                while (PlotCollection.Count > 0 && PlotCollection[0].Time < DateTime.Now.AddMinutes(TrendSettings.PlotTime * (-1)) && i < 10)
                {
                    PlotCollection.RemoveAt(0);
                    i++;
                }
                for (int j = 0; j < 2; j++)
                {
                    var unit = GetCoeff(mainModel.MeasResults[j].MeasUnitMemoryId);
                    if (!(MeasUnit.CompareMeasUnits(unit, MeasResultMeasUnits[j])))
                    {
                        foreach (var point in PlotCollection)
                        {
                            if (j == 0)
                            {
                                point.CurValue1 = ((point.CurValue1 - MeasResultMeasUnits[j].Offset) / MeasResultMeasUnits[j].K) * unit.K + unit.Offset;
                                point.AvgValue1 = ((point.AvgValue1 - MeasResultMeasUnits[j].Offset) / MeasResultMeasUnits[j].K) * unit.K + unit.Offset;
                            }
                            else
                            {
                                point.CurValue2 = ((point.CurValue2 - MeasResultMeasUnits[j].Offset) / MeasResultMeasUnits[j].K) * unit.K + unit.Offset;
                                point.AvgValue2 = ((point.AvgValue2 - MeasResultMeasUnits[j].Offset) / MeasResultMeasUnits[j].K) * unit.K + unit.Offset;
                            }
                        }
                        MeasResultMeasUnits[j] = unit;
                    }
                }

            });

        }
        #endregion     

        #region Запись в файл
        async void WriteArchivalTrendToText()
        {
            if (ArchivalTrendUploading) return;
            if (ArchivalDataPotnts == null) return;
            try
            {
                ArchivalTrendUploading = true;
                await Task.Run(() =>
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("Дата Время" + "\t" + "Счетчик" + "\t" + "Процесс 0: мгновенная ФВ" + "\t"
                             + "Процесс 0: усредненная ФВ" + "\t"
                             + "Процесс 1: мгновенная ФВ" + "\t" + "Процесс 1: усредненная ФВ" + "\t"
                             + "Ток AI0, мкA" + "\t" + "Ток AI1, мкA" + "\t" + "Значение HV out, V" + "\t" + "Температура" + "\n");
                    foreach (var item in ArchivalDataPotnts)
                    {
                        builder.Append(item.Time.ToString("dd/MM/yyyy HH:mm:ss:f") + "\t" + item.Pulses.ToString("0.000") + "\t" + item.CurValue1.ToString("0.000") + "\t"
                             + item.AvgValue1.ToString("0.000") + "\t"
                             + item.CurValue2.ToString("0.000") + "\t" + item.AvgValue2.ToString("0.000") + "\t"
                             + item.Current1.ToString("0.000") + "\t" + item.Current2.ToString("0.000") + "\t" + item.HvValue1.ToString("0.000") + "\t" + item.Temperature.ToString("0.000") + "\n");
                    }
                    using (StreamWriter sw = new StreamWriter(LogPath, false, System.Text.Encoding.Default))
                    {
                        sw.WriteLine(builder.ToString());
                    }
                });
                ArchivalTrendUploading = false;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion        

        /// <summary>
        /// Метод для расчета 
        /// </summary>
        /// <param name="memoryId"></param>
        /// <returns></returns>
        private MeasUnit GetCoeff(string memoryId)
        {
            MeasUnitMemory memory = null;
            MeasUnit measUnit = null;
            using (var _measUnitMemoryRepository = new EfRepository<MeasUnitMemory>())
            {
                memory = _measUnitMemoryRepository.GetAll().Where(m => m.Name == memoryId).FirstOrDefault();
            }
            if (memory is null) return new MeasUnit();
            using (var _measUnitRepository = new EfRepository<MeasUnit>())
            {
                measUnit = _measUnitRepository.GetAll().Where(mu => mu.Id == memory.MeasUnitId).FirstOrDefault();
            }
            if (measUnit is null) return new MeasUnit();
            return measUnit;
        }

        MeasUnit[] MeasResultMeasUnits { get; } = Enumerable.Range(0, 2).Select(i => new MeasUnit()).ToArray();

        void UpadateDates(object obj)
        {
            if (!(obj is Calendar calendar)) return;
            DateTime startDate = new DateTime(calendar.DisplayDate.Year, calendar.DisplayDate.Month, 1);
            var enabledDates = MeasDates
                .Where(dt => dt >= startDate && dt <= startDate.AddMonths(1));
            calendar.DisplayDateEnd = startDate.AddMonths(1);
            var tempDate = startDate;
            calendar.BlackoutDates.Clear();
            while (tempDate <= calendar.DisplayDateEnd)
            {
                if (tempDate != calendar.SelectedDate && !enabledDates.Any(dt => dt == tempDate))
                    calendar.BlackoutDates.Add(new CalendarDateRange(tempDate));
                tempDate = tempDate.AddDays(1);
            }
        }
        private RelayCommand _selectDatesCommand;

        public RelayCommand SelectDatesCommand
        {
            get { return _selectDatesCommand ?? (_selectDatesCommand = new RelayCommand(obj => UpadateDates(obj), obj => true)); }

        }

    }
}
