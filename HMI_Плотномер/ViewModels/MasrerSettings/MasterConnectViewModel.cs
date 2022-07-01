using IDensity.AddClasses;
using IDensity.Models;
using IDensity.Services.MasterSettings;
using IDensity.ViewModels.Commands;
using IDensity.Views.SettingsMaster.ConnectionMaster;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.ViewModels.MasrerSettings
{
    public  class MasterConnectViewModel:PropertyChangedBase
    {
        public MasterConnectViewModel(VM vM)
        {
            VM = vM;
            
        }
        public VM VM { get; }

        ConnectService _connectService = new ConnectService();

        #region Состояние мастера
        /// <summary>
        /// Состояние мастера
        /// </summary>
        private StateConnectionMaster _state;
        /// <summary>
        /// Состояние мастера
        /// </summary>
        public StateConnectionMaster State
        {
            get => _state;
            set => Set(ref _state, value);
        }
        #endregion

        #region Доступные адаптеры
        /// <summary>
        /// DescriptionДоступные адаптеры
        /// </summary>
        private List<NetworkAdapterData> _adapters;
        /// <summary>
        /// DescriptionДоступные адаптеры
        /// </summary>
        public List<NetworkAdapterData> Adapters
        {
            get => _adapters;
            set => Set(ref _adapters, value);
        }
        #endregion


        #region Список найденных Ip 
        /// <summary>
        /// Список найденных Ip 
        /// </summary>
        private List<string> _foundedDevicesIp;
        /// <summary>
        /// Список найденных Ip 
        /// </summary>
        public List<string> FoundedDevicesIp
        {
            get => _foundedDevicesIp;
            set => Set(ref _foundedDevicesIp, value);
        }
        #endregion

        #region Выбранный IP
        /// <summary>
        /// Выбранный IP
        /// </summary>
        private string _selectedIP;
        /// <summary>
        /// Выбранный IP
        /// </summary>
        public string SelectedIP
        {
            get => _selectedIP;
            set => Set(ref _selectedIP, value);
        }
        #endregion

        #region Строка попыток сканирования
        /// <summary>
        /// Строка попыток сканирования
        /// </summary>
        private string _tryScanStatus;
        /// <summary>
        /// Строка попыток сканирования
        /// </summary>
        public string TryScanStatus
        {
            get => _tryScanStatus;
            set => Set(ref _tryScanStatus, value);
        }
        #endregion

        #region Состояние(%) сканирования ip адресов
        /// <summary>
        /// Состояние(%) сканирования ip адресов
        /// </summary>
        private double _scanCompleted;
        /// <summary>
        /// Состояние(%) сканирования ip адресов
        /// </summary>
        public double ScanCompleted
        {
            get => _scanCompleted;
            set => Set(ref _scanCompleted, value);
        }
        #endregion

        #region В процессе
        /// <summary>
        /// В процессе
        /// </summary>
        private bool _isProcessing;
        /// <summary>
        /// В процессе
        /// </summary>
        public bool IsProcessing
        {
            get => _isProcessing;
            set => Set(ref _isProcessing, value);
        }
        #endregion

        /// <summary>
        /// Отменить работу мастера
        /// </summary>
        public void CancelMaster()
        {
            FailureSearchEvent?.Invoke();
            CancelMasterEvent?.Invoke();
            CancelScan();
            CancelWaiting();            
        }

        /// <summary>
        /// Получить данные сетевых адаптеров компьютера
        /// </summary>
        public void GetAdapters()
        {
            Adapters = _connectService.GetAdapterData();
            
        }


        #region Выполнение команды Next
        /// <summary>
        /// Выполнение команды Next
        /// </summary>
        RelayCommand _nextCommand;
        /// <summary>
        /// Выполнение команды Next
        /// </summary>
        public RelayCommand NextCommand => _nextCommand ?? (_nextCommand = new RelayCommand(execPar => 
        {
            OnNextCommand();
        
        }, canExecPar => true));
        #endregion

        CancellationTokenSource CancleScanTokenSource;

        CancellationTokenSource CancelWaitConnectTokenSource;

        void OnNextCommand()
        {
            if (VM.mainModel.Connecting.Value && State != StateConnectionMaster.ConnectionReady)
            {
                State = StateConnectionMaster.ConnectionReady;
                IsProcessing = false;
                return;
            } 
            switch (State)
            {
               case StateConnectionMaster.ConnectionReady:
                    State = StateConnectionMaster.Exit;
                    break;
               case StateConnectionMaster.Start:
                    GetAdapters();
                    if (Adapters.Count() > 0)
                    {
                        State = StateConnectionMaster.SearchInterfaces;
                        DescribeOnCheckedNetwork();
                        IsProcessing = true;
                    }
                    else
                    {
                        State = StateConnectionMaster.NoInterfaces;
                        IsProcessing = false;
                    }
                    break;
                case StateConnectionMaster.NoInterfaces:
                    State = StateConnectionMaster.Exit;
                    break;
                case StateConnectionMaster.SearchInterfaces:
                    State = StateConnectionMaster.ScaningIP;
                    Scan();
                    break;
                case StateConnectionMaster.ScaningIP:
                    State = StateConnectionMaster.WaitingForConnect;
                    WaitConnectingAsync();
                    break;

            }
        }

        void DescribeOnCheckedNetwork()
        {
            foreach (var adapter in Adapters)
            {
                adapter.PropertyChanged += (o, e) => { IsProcessing = !(Adapters.Where(a => a.Selected).Count() > 0); };
            }
        }

        #region События
        public event Action<string> SuccessSearchEvent = delegate { };
        public event Action FailureSearchEvent = delegate { };
        public event Action CancelMasterEvent = delegate { };
        #endregion

        

        async void Scan()
        {
            IsProcessing = true;
            CancleScanTokenSource = new CancellationTokenSource();
            _connectService.TryScanEvent += (s) =>
            {
                TryScanStatus = $"Сканирование {s}";
                ScanCompleted = _connectService.processCurrent;
            };
            this.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(SelectedIP))
                {
                    IsProcessing = string.IsNullOrEmpty(SelectedIP) ? true : false;
                }
            };
            _connectService.FoundDeviceEvent += (s) => FoundedDevicesIp = _connectService.FoundedDevicesIP;            
            await _connectService.Scan(Adapters.Where(a=>a.Selected), CancleScanTokenSource.Token);
            ScanCompleted = 100;
            if (!(FoundedDevicesIp!=null && FoundedDevicesIp.Count > 0))
            {
                State = StateConnectionMaster.NoInterfaces;
                IsProcessing = false;
            }           
        }

        async void WaitConnectingAsync()
        {
            IsProcessing = true;
            VM.mainModel.Tcp.IP = SelectedIP;
            CancelScan();
            CancelWaitConnectTokenSource = new CancellationTokenSource();
            await Task.Run(()=>
            {
                if (WaitingConnectAsync().Wait(30000, CancelWaitConnectTokenSource.Token))
                {
                    State = StateConnectionMaster.ConnectionReady;
                }
                else
                    State = StateConnectionMaster.FailureSearch;
                IsProcessing = false;
                    
            });
        }

        async Task WaitingConnectAsync()
        {
            await Task.Run(() => 
            {
                while (!VM.mainModel.Connecting.Value)
                {
                    Thread.Sleep(1000);
                }
            });
        }

        void CancelScan()
        {
            if (CancleScanTokenSource != null && CancleScanTokenSource.Token.CanBeCanceled)
            {
                CancleScanTokenSource.Cancel();
            }
        }

        void CancelWaiting()
        {
            if (CancelWaitConnectTokenSource != null && CancelWaitConnectTokenSource.Token.CanBeCanceled)
            {
                CancelWaitConnectTokenSource.Cancel();
            }
        }
    }
}
