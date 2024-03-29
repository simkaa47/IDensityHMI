﻿using IDensity.AddClasses.AdcBoardSettings;
using IDensity.Core.Models.Analogs;
using IDensity.Core.Models.CheckMaster;
using IDensity.Core.Models.Counters;
using IDensity.Core.Models.MeasProcess;
using IDensity.Core.Models.MeasResults;
using IDensity.Core.Models.Parameters;
using IDensity.Core.Models.Tcp;
using IDensity.Core.Models.Telemetry;
using IDensity.DataAccess;
using IDensity.Services.InitServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace IDensity.Models
{
    [DataContract]
    public class MainModel : PropertyChangedBase
    {
        #region Количество наборов стандартизаций
        /// <summary>
        /// Количество стандартизаций
        /// </summary>
        public static readonly int CountStand = 12;
        #endregion

        #region Количество счетчиков
        /// <summary>
        /// Количество счетчиков
        /// </summary>
        public static readonly int CountCounters = 3;
        #endregion

        #region Количество измерительных процессов
        /// <summary>
        /// Количество измерительных процессов
        /// </summary>
        public const int MeasProcNum = 8;
        #endregion        

        #region Значения TCP сервера
        #region IP адрес платы
        string _ip = "192.168.10.151";
        /// <summary>
        /// IP адрес платы
        /// </summary>
        [DataMember]
        public string IP
        {
            get => _ip;
            set
            {
                if (MainModel.CheckIp(value)) Set(ref _ip, value);
                TcpWriting = false;
            }
        }
        #endregion

        #region Маска
        string _mask = "255.255.255.0";
        /// <summary>
        /// IP адрес платы
        /// </summary>
        [DataMember]
        public string Mask
        {
            get => _mask;
            set
            {
                if (MainModel.CheckIp(value)) Set(ref _mask, value);
                TcpWriting = false;
            }
        }
        #endregion

        #region GateWay
        string _gateway = "192.168.10.1";
        /// <summary>
        /// IP адрес платы
        /// </summary>
        [DataMember]
        public string GateWay
        {
            get => _gateway;
            set
            {
                if (MainModel.CheckIp(value)) Set(ref _gateway, value);
                TcpWriting = false;
            }
        }
        #endregion


        #region Флаг записи
        /// <summary>
        /// Флаг записи
        /// </summary>
        private bool _tcpWriting = true;
        /// <summary>
        /// Флаг записи
        /// </summary>
        public bool TcpWriting
        {
            get => _tcpWriting;
            set => Set(ref _tcpWriting, value);
        }
        #endregion

        #endregion

        #region MAC адрес

        #region MAC
        /// <summary>
        /// MAC
        /// </summary>
        private byte[] _mac;
        /// <summary>
        /// MAC
        /// </summary>
        [DataMember]
        public byte[] Mac
        {
            get => _mac;
            set
            {
                if (Set(ref _mac, value))
                {
                    MacWriting = false;
                }
            }
        }
        #endregion

        #region Флаг записи MAc адреса
        /// <summary>
        /// Флаг записи MAc адреса
        /// </summary>
        private bool _macWriting;
        /// <summary>
        /// Флаг записи MAc адреса
        /// </summary>
        public bool MacWriting
        {
            get => _macWriting;
            set => Set(ref _macWriting, value);
        }
        #endregion

        #endregion

        public MainModel()
        {
            Init();// Инициализация параметров  
            MeasProccessInit();
        }

        #region RTC контроллера платы        
        public Parameter<DateTime> Rtc { get; set; } = new Parameter<DateTime>("Rtc", "Часы реального времени", DateTime.MinValue, DateTime.MaxValue, 19, "hold");
        #endregion

        #region Статус соединения с платой       
        /// <summary>
        /// Статус соединения с платой
        /// </summary>
        public Parameter<bool> Connecting { get; } = new Parameter<bool>("ConnectBoard", "Статус соединения с платой", false, true, 0, "") {Value=true };

        #endregion

        #region Данные измерения
        public MeasResult[] MeasResults { get; } = Enumerable.Range(0, 2).Select(i => new MeasResult($"MeasResult{i}")).ToArray();
        /// <summary>


        #region Статус циклических измерений
        public Parameter<bool> CycleMeasStatus { get; } = new Parameter<bool>("CycleMeasStatus", "Статус циклических измерений", false, true, 1, "hold");
        #endregion

        #region Данные тлеметрии HV
        [DataMember]
        public HvTelemetry TelemetryHV { get; set; } = new HvTelemetry();
        #endregion        

        #region Данные телеметрии платы питания(темпратуры)
        public TempBoardTelemetry TempTelemetry { get; } = new TempBoardTelemetry();
        #endregion        

        #region Статус связи с платой АЦП
        public Parameter<bool> AdcBoardCommState { get; } = new Parameter<bool>("AdcBoardCommState", "Статус связи с платой АЦП", false, true, 0, "") { Value = true};
        #endregion

        #region Переменные связи
        public Parameter<ushort> CommStates { get; } = new Parameter<ushort>("CommStates", "Переменные связи", ushort.MinValue, ushort.MaxValue, 63, "read")
        {
            OnlyRead = true
        };
        #endregion

        #region Битовые переменные состояния аналоговых модулей
        public Parameter<ushort>[] AnalogStateGroups { get; } = new Parameter<ushort>[]
        {
            new Parameter<ushort>("AnalogStateGroup1", "Состояние аналоговых модулей группы 1", 0, ushort.MaxValue, 65, "read"),
            new Parameter<ushort>("AnalogStateGroup2", "Состояние аналоговых модулей группы 2", 0, ushort.MaxValue, 66, "read")
         };
        #endregion



        #endregion

        #region Флаг записи на SD карту
        /// <summary>
        /// Флаг записи на SD карту
        /// </summary>
        private bool _isSdWriting;
        /// <summary>
        /// Флаг записи на SD карту
        /// </summary>
        public bool IsSdWriting
        {
            get => _isSdWriting;
            set => Set(ref _isSdWriting, value);
        }
        #endregion

        #region Настройки в плате
        #region Данные измерительных процессов
        MeasProcSettings[] _measProcSettings;
        [DataMember]
        public MeasProcSettings[] MeasProcSettings
        {
            get => _measProcSettings;
            set => _measProcSettings = value;
        }
        /// <summary>
        /// Инициализация измерительных процессов
        /// </summary>
        void MeasProccessInit()
        {
            if (_measProcSettings == null)
            {
                _measProcSettings = Enumerable.Range(0, MeasProcNum)
                    .Select(i => new MeasProcSettings(i))
                    .ToArray();
            }
        }
        #endregion                

        #region Настройки аналоговых модулей
        AnalogGroup[] _analogGroups;
        [DataMember]
        public AnalogGroup[] AnalogGroups
        {
            get
            {
                if (_analogGroups == null)
                {
                    _analogGroups = Enumerable.Range(0, 2)
                        .Select(z => new AnalogGroup(z))
                        .ToArray();
                }
                return _analogGroups;
            }
            set => _analogGroups = value;
        }
        #endregion 

        #region Данные счетчиков 
        [DataMember]
        public CountDiapasone[] CountDiapasones { get; set; } = Enumerable.Range(0, CountCounters).Select(i => new CountDiapasone()).ToArray();
        #endregion

        #region Текущий номер счетчика
        [DataMember]
        public Parameter<byte> CounterNum { get; set; } = new Parameter<byte>("CounterNum", "Текущий тип счетчика", 0, (byte)(CountCounters - 1), 0, "");
        #endregion

        #region Параметры полседовательного порта платы
        #region Баудрейт
        [DataMember]
        public Parameter<uint> PortBaudrate { get; set; } = new Parameter<uint>("PortBaudrate", "Скорость передачи данных", 1200, 115200, 48, "hold");
        #endregion

        #region Режим работы последовательного порта
        [DataMember]
        public Parameter<ushort> PortSelectMode { get; set; } = new Parameter<ushort>("PortSelectMode", "Режим работы последовательного порта", 0, 1, 50, "hold");
        #endregion
        #endregion

        #region Настройки платы АЦП

        #region Адрес UDP приемника 

        #region Флаг записи Udp настроек
        /// <summary>
        /// Флаг записи Udp настроек
        /// </summary>
        private bool _udpWriting = true;
        /// <summary>
        /// Флаг записи Udp настроек
        /// </summary>
        public bool UdpWriting
        {
            get => _udpWriting;
            set => Set(ref _udpWriting, value);
        }
        #endregion

        private string _udpAddrString = "0.0.0.0";
        /// <summary>
        /// Адрес UDP приемника
        /// </summary>
        [DataMember]
        public string UdpAddrString
        {
            get { return _udpAddrString; }
            set
            {
                if (CheckIp(value))
                {
                    Set(ref _udpAddrString, value);
                    UdpWriting = false;
                }
            }
        }

        private int _portUdp;
        /// <summary>
        /// Номер порта Udp источника
        /// </summary>
        [DataMember]
        public int PortUdp
        {
            get { return _portUdp; }
            set { Set(ref _portUdp, value); UdpWriting = false; }
        }


        #region Метод проверки корректности ip
        /// <summary>
        /// Проверка корректности ip
        /// </summary>
        /// <param name="ip">Проверяемая строка</param>
        /// <returns>true, если ip корректно</returns>
        public static bool CheckIp(string ip)
        {
            var arrStr = ip.Split(".");
            int temp = 0;
            if (arrStr.Length != 4) return false;
            foreach (var item in arrStr)
            {
                if (!int.TryParse(item, out temp)) return false;
                if ((temp < 0) || (temp > 255)) return false;
            }
            return true;
        }
        #endregion
        #endregion

        #region Настройки платы АЦП
        [DataMember]
        public AdcBoardSettings AdcBoardSettings { get; set; } = new AdcBoardSettings();
        #endregion

        #endregion  

        #region Имена изотопов
        /// <summary>
        /// Имена изотопов
        /// </summary>
        private string[] _isotopeNames;
        /// <summary>
        /// Имена изотопов
        /// </summary>
        public string[] IsotopeNames
        {
            get => _isotopeNames;
            set => Set(ref _isotopeNames, value);
        }
        #endregion

        #region Текущий изотоп
        [DataMember]
        public Parameter<byte> IsotopeIndex { get; set; } = new Parameter<byte>("IsotopeIndex", "Тип изотопа", 0, 10, 0, "");
        #endregion

        #region Время установки источника
        [DataMember]
        public Parameter<DateTime> SourceInstallDate { get; set; } = new Parameter<DateTime>("SourceInstallDate", "Дата установки источника", DateTime.MinValue, DateTime.MaxValue, 0, "");
        #endregion

        #region Время истечения источника
        [DataMember]
        public Parameter<DateTime> SourceExpirationDate { get; set; } = new Parameter<DateTime>("SourceExpirationDate", "Дата истечения срока источника", DateTime.MinValue, DateTime.MaxValue, 0, "");
        #endregion

        #region Серийный номер источника
        [DataMember]
        public Parameter<string> SerialNumber { get; set; } = new Parameter<string>("SerialNumber", "Серийный номер", string.Empty, "zzzzzzzzzzzzzzz", 0, "", true);
        #endregion

        #region Номер заказа
        [DataMember]
        public Parameter<string> OrderNumber { get; set; } = new Parameter<string>("DeviceOrder", "Номер заказа", string.Empty, "zzzzzzzzzzzzzzz", 0, "", true);
        #endregion        

        #region Версия firmware
        [DataMember]
        public Parameter<string> FwVersion { get; set; } = new Parameter<string>("FwVersion", "Версия встроенного ПО", string.Empty, "zzzzzzzzzzzzzzz", 0, "", true);
        #endregion

        #region Номер заказчика
        [DataMember]
        public Parameter<string> CustNumber { get; set; } = new Parameter<string>("CustNumber", "Номер проекта", string.Empty, "zzzzzzzzzzzzzzz", 0, "", true);
        #endregion

        #region Данные прочитаны
        public bool SettingsReaded { get; set; }
        #endregion

        #region Настройки фильтра калмана
        [DataMember]
        public List<Kalman> KalmanSettings { get; set; } = Enumerable.Range(0, 2).Select(i => new Kalman(i)).ToList();
        #endregion

        #region Настройки получения температуры
        [DataMember]
        public GetTemperature GetTemperature { get; set; } = new GetTemperature();
        #endregion

        #region Тип устройства
        [DataMember]
        public Parameter<ushort> DeviceType { get; set; } = new Parameter<ushort>("DeviceType", "Тип устройства", 0, 1, 0, "");
        #endregion

        #region Длина уровнемена
        [DataMember]
        public Parameter<float> LevelLength { get; set; } = new Parameter<float>("LevelLength", "Длина уровнемера, мм", 0, float.MaxValue, 0, "");
        #endregion

        public CheckSum CheckSum { get; set; }

        #region Текущая контрольная сумма ПО
        /// <summary>
        /// Текущая контрольная сумма ПО
        /// </summary>
        private uint _curCheckSum;
        /// <summary>
        /// Текущая контрольная сумма ПО
        /// </summary>
        public uint CurCheckSum
        {
            get => _curCheckSum;
            set => Set(ref _curCheckSum, value);
        }
        #endregion

        #endregion

        #region Физические параметры платы
        /// <summary>
        /// Физические параметры платы
        /// </summary>
        private ushort _physParamsState;
        /// <summary>
        /// Физические параметры платы
        /// </summary>
        public ushort PhysParamsState
        {
            get => _physParamsState;
            set => Set(ref _physParamsState, value);
        }
        #endregion

        public TcpConnectData TcpConnectData { get; private set; }

        #region Инициализация
        void Init()
        {
            TcpConnectData = XmlInit.ClassInit<TcpConnectData>();
            CheckSum = XmlInit.ClassInit<CheckSum>();
            Connecting.PropertyChanged += (obj, args) => SettingsReaded = false;
        }

        #region Парсинг битовых значений устройств
        public void GetDeviceData()
        {
            AdcBoardSettings.CommState.Value = (CommStates.Value & 1) == 0;
            TempTelemetry.TempBoardCommState.Value = (CommStates.Value & 2) == 0;
            TelemetryHV.HvCommState.Value = (CommStates.Value & 4) == 0;
            for (int i = 0; i < 2; i++)
            {
                AnalogGroups[i].AO.PwrState.Value = (AnalogStateGroups[i].Value & 1) != 1;
                AnalogGroups[i].AI.PwrState.Value = (AnalogStateGroups[i].Value & 2) != 2;
                AnalogGroups[i].AO.CommState.Value = (AnalogStateGroups[i].Value & 4) != 4;
                AnalogGroups[i].AI.CommState.Value = (AnalogStateGroups[i].Value & 8) != 8;

            }
        }
        #endregion

        #endregion


    }
}
