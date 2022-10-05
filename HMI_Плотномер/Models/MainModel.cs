using IDensity.AddClasses;
using IDensity.AddClasses.AdcBoardSettings;
using IDensity.AddClasses.Settings;
using IDensity.Core.AddClasses.Settings;
using IDensity.Services.ComminicationServices;
using IDensity.Services.InitServices;
using IDensity.Services.XML;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IDensity.Models
{
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
        public string IP
        {
            get => _ip;
            set
            {
                if (MainModel.CheckIp(value)) Set(ref _ip, value);
            }
        }
        #endregion

        #region Маска
        string _mask = "255.255.255.0";
        /// <summary>
        /// IP адрес платы
        /// </summary>
        public string Mask
        {
            get => _mask;
            set
            {
                if (MainModel.CheckIp(value)) Set(ref _mask, value);
            }
        }
        #endregion

        #region GateWay
        string _gateway = "192.168.10.1";
        /// <summary>
        /// IP адрес платы
        /// </summary>
        public string GateWay
        {
            get => _gateway;
            set
            {
                if (MainModel.CheckIp(value)) Set(ref _gateway, value);
            }
        }
        #endregion
        #endregion

        public MainModel()
        {
            Init();// Инициализация параметров  
            MeasProccessInit();
        }

        #region Спсоб соединения с платой
        public CommMode CommMode { get; private set; }
        #endregion

        #region RTC контроллера платы
        public Parameter<DateTime> Rtc { get; } = new Parameter<DateTime>("Rtc", "Часы реального времени", DateTime.MinValue, DateTime.MaxValue, 19, "hold");
        #endregion

        #region Статус соединения с платой       
        /// <summary>
        /// Статус соединения с платой
        /// </summary>
        public Parameter<bool> Connecting { get; } = new Parameter<bool>("ConnectBoard", "Статус соединения с платой", false, true, 0, "");

        #endregion

        #region Данные измерения
        public MeasResult[] MeasResults { get; } = Enumerable.Range(0, 2).Select(i => new MeasResult($"MeasResult{i}")).ToArray();
        /// <summary>
        /// Привязка настроек измерения к результатам измерения
        /// </summary>
        //public void SetMeasResultData()
        //{            
        //    for (int i = 0; i < 2; i++)
        //    {
        //        MeasResults[i].Settings = MeasProcSettings[MeasResults[i].MeasProcessNum.Value];
        //        if (MeasResults[i].Settings.IsActive.Value) MeasResults[i].IsActive = true;
        //    }
        //}


        #region Статус циклических измерений
        public Parameter<bool> CycleMeasStatus { get; } = new Parameter<bool>("CycleMeasStatus", "Статус циклических измерений", false, true, 1, "hold");
        #endregion

        #region Данные тлеметрии HV
        public HvTelemetry TelemetryHV { get; } = new HvTelemetry();
        #endregion        

        #region Данные телеметрии платы питания(темпратуры)
        public TempBoardTelemetry TempTelemetry { get; } = new TempBoardTelemetry();
        #endregion        

        #region Статус связи с платой АЦП
        public Parameter<bool> AdcBoardCommState { get; } = new Parameter<bool>("AdcBoardCommState", "Статус связи с платой АЦП", false, true, 0, "");
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
        public MeasProcSettings[] MeasProcSettings
        {
            get=> _measProcSettings;            
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

        #region Настройки платы АЦП
        public AdcParameters AdcBoard { get; } = new AdcParameters();
        #endregion

        #region Настройки аналоговых модулей
        AnalogGroup[] _analogGroups;
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
        }
        #endregion 

        #region Данные счетчиков        
        public CountDiapasone[] CountDiapasones { get; } = Enumerable.Range(0, CountCounters).Select(i => new CountDiapasone()).ToArray();
        #endregion        

        #region Параметры полседовательного порта платы
        #region Баудрейт
        public Parameter<uint> PortBaudrate { get; } = new Parameter<uint>("PortBaudrate", "Скорость передачи данных", 1200, 115200, 48, "hold");
        #endregion

        #region Режим работы последовательного порта
        public Parameter<ushort> PortSelectMode { get; } = new Parameter<ushort>("PortSelectMode", "Режим работы последовательного порта", 0, 1, 50, "hold");
        #endregion
        #endregion

        #region Настройки едениц измерений
        public MeasUnitSettings[] MeasUnitSettings { get; } = Enumerable.Range(0, 21)
            .Select(i => new MeasUnitSettings())
            .ToArray();        
        #endregion        

        #region Настройки платы АЦП

        #region Адрес UDP приемника        
        private string _udpAddrString = "0.0.0.0";
        /// <summary>
        /// Адрес UDP приемника
        /// </summary>
        public string UdpAddrString
        {
            get { return _udpAddrString; }
            set
            {
                if (CheckIp(value))
                {
                    Set(ref _udpAddrString, value);
                }
            }
        }

        private int _portUdp;
        /// <summary>
        /// Номер порта Udp источника
        /// </summary>
        public int PortUdp
        {
            get { return _portUdp; }
            set { Set(ref _portUdp, value); }
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
        public AdcBoardSettings AdcBoardSettings { get; } = new AdcBoardSettings();        
        #endregion

        #endregion

        #region Значение полураспада
        public Parameter<float> HalfLife { get; } = new Parameter<float>("HalfLife", "Значение полураспада", float.MinValue, float.MaxValue, 0, "");
        #endregion

        #region Название прибора
        public Parameter<string> DeviceName { get; } = new Parameter<string>("DeviceName", "Название прибора", string.Empty, "zzzzzzzzzzz", 0, "");
        #endregion

        #region Имя изотопа
        public Parameter<string> IsotopName { get; } = new Parameter<string>("IsotopName", "Название изотопа", string.Empty, "zzzzzzzzzzz", 0, "");
        #endregion        

        #region Время установки источника
        public Parameter<DateTime> SourceInstallDate { get; } = new Parameter<DateTime>("SourceInstallDate", "Дата установки источника", DateTime.MinValue, DateTime.MaxValue, 0, "");
        #endregion

        #region Время истечения источника
        public Parameter<DateTime> SourceExpirationDate { get; } = new Parameter<DateTime>("SourceExpirationDate", "Дата истечения срока источника", DateTime.MinValue, DateTime.MaxValue, 0, "");
        #endregion

        #region Серийный номер источника
        public Parameter<string> SerialNumber { get; } = new Parameter<string>("SerialNumber", "Серийный номер", string.Empty, "zzzzzzzzzzzzzzz", 0, "", true);
        #endregion

        #region Номер заказа
        public Parameter<string> OrderNumber { get; } = new Parameter<string>("DeviceOrder", "Номер заказа", string.Empty, "zzzzzzzzzzzzzzz", 0, "", true);
        #endregion        

        #region Версия firmware
        public Parameter<string> FwVersion { get; } = new Parameter<string>("FwVersion", "Версия FW", string.Empty, "zzzzzzzzzzzzzzz", 0, "", true);
        #endregion

        #region Номер заказчика
        public Parameter<string> CustNumber { get; } = new Parameter<string>("CustNumber", "Номер заказчика", string.Empty, "zzzzzzzzzzzzzzz", 0, "", true);
        #endregion

        #region Данные прочитаны
        public bool SettingsReaded { get; set; }
        #endregion

        #region Настройки фильтра калмана
        public List<Kalman> KalmanSettings { get; } = Enumerable.Range(0, 2).Select(i => new Kalman(i)).ToList();
        #endregion

        #region Настройки получения температуры
        public GetTemperature GetTemperature { get; } = new GetTemperature();
        #endregion

        #region Тип устройства
        public Parameter<ushort> DeviceType { get; } = new Parameter<ushort>("DeviceType", "Тип устройства", 0, 1, 0, "");
        #endregion

        #region Длина уровнемена
        public Parameter<float> LevelLength { get; } = new Parameter<float>("LevelLength", "Длина уровнемера, мм", 0, float.MaxValue, 0, "");
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
            AdcBoard.CommState.Value = (CommStates.Value & 1) == 0;
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

        #region Команда принудиельного запроса набора стандартизации после стандартизации
        //public void GetStdSelection(ushort index)
        //{
        //    if (CommMode.EthEnable) Tcp.GetMeasSettingsExternal(index);           
        //}
        #endregion

        #region Команды изменения настроек платы АЦП
        //public void SetAdcBoardSettings(AdcBoardSettings settings)
        //{            
        //    if(CommMode.RsEnable) rs.SetAdcBoardSettings(settings);
        //}

        

        #region Команда "Переключить реле"
        public void SwitchRelay(ushort value)
        {
            //if (CommMode.EthEnable) Tcp.SwitchRelay(value);
            //else if (CommMode.RsEnable) rs.SetMeasUnitsSettings(settings);
        }
        #endregion
        #endregion
        #endregion

    }
}
