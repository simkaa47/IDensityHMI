using IDensity.AddClasses;
using IDensity.Models;
using IDensity.Services.CheckServices;
using IDensity.Services.ComminicationServices;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.ViewModels.MasrerSettings
{
    public class CheckMasterVm : PropertyChangedBase
    {
        public CheckMasterVm(VM vM)
        {
            VM = vM;
            _model = vM.mainModel;
            _commService = vM.CommService;
            Describe();
        }
        private readonly MainModel _model;
        private readonly CommunicationService _commService;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        #region Статусы процесса

        #region Основной статус процессв
        /// <summary>
        /// Основной статус процессв
        /// </summary>
        private string _mainStatus;
        /// <summary>
        /// Основной статус процессв
        /// </summary>
        public string MainStatus
        {
            get => _mainStatus;
            set => Set(ref _mainStatus, value);
        }
        #endregion

        #region Вспомогательный статус процесса
        /// <summary>
        /// Вспомогательный статус процесса
        /// </summary>
        private string _subStatus;
        /// <summary>
        /// Вспомогательный статус процесса
        /// </summary>
        public string SubStatus
        {
            get => _subStatus;
            set => Set(ref _subStatus, value);
        }
        #endregion

        #endregion

        #region Список результатов
        /// <summary>
        /// Список результатов
        /// </summary>
        private List<DeviceCheckResult> _results = new List<DeviceCheckResult>();
        /// <summary>
        /// Список результатов
        /// </summary>
        public List<DeviceCheckResult> Results
        {
            get => _results;
            set => Set(ref _results, value);
        }
        #endregion

        #region Сервисы

        #endregion

        #region Дата последней проверки прибора
        /// <summary>
        /// Дата последней проверки прибора
        /// </summary>
        private DateTime _lastCheckDate;
        /// <summary>
        /// Дата последней проверки прибора
        /// </summary>
        public DateTime LastCheckDate
        {
            get => _lastCheckDate;
            set => Set(ref _lastCheckDate, value);
        }
        #endregion

        public VM VM { get; }

        #region Стадия проверки
        /// <summary>
        /// Стадия проверки
        /// </summary>
        private CheckMasterStates _stage;
        /// <summary>
        /// Стадия проверки
        /// </summary>
        public CheckMasterStates Stage
        {
            get => _stage;
            set => Set(ref _stage, value);
        }
        #endregion 

        #region Отмена 
        /// <summary>
        /// Отмена 
        /// </summary>
        RelayCommand _cancelCommand;
        /// <summary>
        /// Отмена 
        /// </summary>
        public RelayCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(execPar =>
        {
            Cancel();
            Stage = CheckMasterStates.CancelByUser;

        }, canExecPar => true));
        #endregion

        #region Старт
        /// <summary>
        /// Старт
        /// </summary>
        RelayCommand _startCheckCommandCommand;
        /// <summary>
        /// Старт
        /// </summary>
        public RelayCommand StartCheckCommandCommand => _startCheckCommandCommand ?? (_startCheckCommandCommand = new RelayCommand(execPar =>
        {
            Check();
        }, canExecPar => true));
        #endregion

        public async void Check()
        {
            if (Stage != CheckMasterStates.Start) return;
            Stage = CheckMasterStates.Process;
            try
            {
                MainStatus = "Проверка работы аналоговых выходов...";
                await StartService(new AnalogCheckService(_cancellationTokenSource, VM));
                MainStatus = "Проверка корректности импульсов от ФЭУ...";
                await StartService(new PulseCheckService(_cancellationTokenSource, VM));
                MainStatus = "Проверка корректности работы модуля RTC...";
                await StartService(new RtcCheckService(_cancellationTokenSource, VM));
                MainStatus = "Проверка контрольной суммы ПО прибора...";
                await StartService(new CheckCheckSumService(_cancellationTokenSource, VM));
                LastCheckDate = DateTime.Now;
                Stage = CheckMasterStates.Success;
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        async Task StartService(ICheckService service)
        {
            if (service is null) return;
            service.ProcessEvent += s => SubStatus = s;
            var results = await service.Check();
            Results.AddRange(results);
        }

        void Print()
        {
            //States.Add($"Проверка аналогового модуля {moduleNum}:");
            //foreach (var result in results)
            //{
            //    States.Add($"Уровень тока {result.SetLevel} mA:");
            //    States.Add($"Измеренное значение {result.CurrentLevel.ToString("f3")}");
            //    States.Add($"Отклонение {((Math.Abs(result.CurrentLevel-result.SetLevel))/result.SetLevel*100).ToString("f3")}%");                
            //}
        }

        void Describe()
        {
            _model.Connecting.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(_model.Connecting.Value))
                {
                    if (!_model.Connecting.Value)
                    {
                        Cancel();
                        Stage = CheckMasterStates.CancelByCommunicate;
                    }
                }
            };
        }

        void Cancel()
        {
            if (_cancellationTokenSource.Token.CanBeCanceled)
            {
                _cancellationTokenSource.Cancel();
            }
        }


    }
}
