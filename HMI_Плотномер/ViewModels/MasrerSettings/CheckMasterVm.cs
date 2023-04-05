using IDensity.AddClasses;
using IDensity.Core.Services.CheckServices;
using IDensity.Core.Services.CheckServices.ElectronicUnit;
using IDensity.Core.Services.CheckServices.PrepareChecking;
using IDensity.Core.Services.CheckServices.Process;
using IDensity.Core.Services.CheckServices.Sensor;
using IDensity.Models;
using IDensity.Services.CheckServices;
using IDensity.Services.ComminicationServices;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region Процент выполнения
        /// <summary>
        /// Процент выполнения
        /// </summary>
        private double _processPercent;
        /// <summary>
        /// Процент выполнения
        /// </summary>
        public double ProcessPercent
        {
            get => _processPercent;
            set => Set(ref _processPercent, value);
        }
        #endregion


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

        #region Информация об устройстве
        public DeviceInformation DeviceInfo { get; set; } = new DeviceInformation();
        #endregion

        #region Предварительная проверка
        public PrepareCheck PrepareCheckInformation { get; } = new PrepareCheck();

        #endregion

        #region Результаты проверки 
        private ElectronicUnitCheck _electronicUnitCheck;
        public ElectronicUnitCheck ElectronicUnitCheck
        {
            get => _electronicUnitCheck;
            set => Set(ref _electronicUnitCheck, value);
        }

        private SensorCheck _sensorCheck;
        public SensorCheck SensorCheck
        { 
            get => _sensorCheck;
            set => Set(ref _sensorCheck, value);
        }


        #endregion

        #region Список параметров процесса
        /// <summary>
        /// Список результатов
        /// </summary>
        private List<ProcessParameter> _processes = new List<ProcessParameter>();
        /// <summary>
        /// Список результатов
        /// </summary>
        public List<ProcessParameter> Processes
        {
            get => _processes;
            set => Set(ref _processes, value);
        }
        #endregion

        #region Результат проверки
        /// <summary>
        /// Результат проверки
        /// </summary>
        private bool _result;
        /// <summary>
        /// Результат проверки
        /// </summary>
        public bool Result
        {
            get => _result;
            set => Set(ref _result, value);
        }
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
                ProcessPercent = 0;
                MainStatus = "Предварительное условие проверки...";
                GetDeviceInformation();                
                new PrepareCheckService(PrepareCheckInformation, VM).Check();
                ProcessPercent = 10;
                MainStatus = "Проверка основного блока электроники...";
                var electronicCheckService = new ElectronicUnitCheckService(_cancellationTokenSource, VM);
                electronicCheckService.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == nameof(electronicCheckService.ProcessPercent))
                    {
                        ProcessPercent  = 10 + 80.0 / 100.0 * electronicCheckService.ProcessPercent;
                    }
                };
                ElectronicUnitCheck = await electronicCheckService.Check();
                MainStatus = "Проверка блока ФЭУ...";
                var sensorCheckService = new SensorCheckService(_cancellationTokenSource, VM);
                sensorCheckService.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == nameof(sensorCheckService.ProcessPercent))
                    {
                        ProcessPercent = 90 + 10.0 / 100.0 * sensorCheckService.ProcessPercent;
                    }
                };
                SensorCheck = await sensorCheckService.Check();

                Processes = new GetProcesesService(VM).GetProcessParameters();
                Stage = CheckMasterStates.Success;
                ProcessPercent = 100;
                Result = ElectronicUnitCheck.CheckResult && PrepareCheckInformation.CheckResult && SensorCheck.CheckResult && Processes.All(p => p.Value >= p.MinValue && p.Value <= p.MaxValue);
                LastCheckDate = DateTime.Now;
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
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

        void GetDeviceInformation()
        {            
            DeviceInfo.HmiSoftwareNumber = App.VersionNumber;
            DeviceInfo.SerialNumber = VM.mainModel.SerialNumber.Value;
        }


    }
}
