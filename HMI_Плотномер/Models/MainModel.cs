using IDensity.AddClasses;
using IDensity.AddClasses.AdcBoardSettings;
using IDensity.AddClasses.Settings;
using IDensity.AddClasses.Standartisation;
using IDensity.Models.XML;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IDensity.Models
{
    class MainModel : PropertyChangedBase
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
        public static readonly int CountCounters = 8;
        #endregion

        #region Количество измерительных процессов
        /// <summary>
        /// Количество измерительных процессов
        /// </summary>
        public const int MeasProcNum = 8; 
        #endregion

        
        public MainModel()
        {
            Init();// Инициализация параметров  
            MeasProccessInit();
            AdcSettingsEventDescribe();
            MeasUnitSettingsDescribe();
            // Дествия по изменению счетчиков
            foreach (var diap in CountDiapasones)
            {
                diap.NeedWriteEvent += WriteCounterSettings;
            }
        }


        #region События
        #region Обновились данные
        public event Action UpdateDataEvent;
        #endregion
        #endregion

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
        public MeasResult[] MeasResults { get; } = Enumerable.Range(0, 2).Select(i => new MeasResult()).ToArray();
        /// <summary>
        /// Привязка настроек измерения к результатам измерения
        /// </summary>
        public void SetMeasResultData()
        {            
            for (int i = 0; i < 2; i++)
            {
                MeasResults[i].Settings = MeasProcSettings[MeasResults[i].MeasProcessNum.Value];
                if (MeasResults[i].Settings.IsActive.Value) MeasResults[i].IsActive = true;
            }
        }


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
                    .Select(i =>
                    {
                        var mp = new MeasProcSettings(i);
                        mp.NeedWriteEvent += WriteMeasProcSettings;
                        mp.IsActive.CommandEcecutedEvent += (s) => SetMeasProcActivity();
                        mp.NeedMakeStand += MakeStand;
                        mp.StandFinishEvent += (num) => Tcp.GetMeasSettingsExternal(num);
                        mp.NeedMakeSingleMeasEvent += MakeSingleMeasure;
                        mp.SingleMeasEventFinishedEvent += (num) => Tcp.GetMeasSettingsExternal(num);
                        return mp;
                    })
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
                    _analogGroups = Enumerable.Range(0, 2).
                        Select(z =>
                        {
                            var gr = new AnalogGroup(z);
                            gr.AI.SwitchPwrEvent += SwitchAm;
                            gr.AI.ChangeSettCallEvent += ChangeAdcAct;
                            gr.AO.SwitchPwrEvent += SwitchAm;
                            gr.AO.SetTestValueCallEvent += SetTestValueAm;
                            gr.AO.ChangeSettCallEvent += ChangeDacAct;
                            return gr;
                        }).ToArray();

                }
                return _analogGroups;
            }
        }
        #endregion               

        #region Данные стандартизаций

        public StandData[] StandSettings { get; } = Enumerable.Range(0, 12).Select(i => new StandData()).ToArray();
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
        public MeasUnitSettings[] MeasUnitSettings { get; } = Enumerable.Range(0, 5).Select(i => new MeasUnitSettings()).ToArray();
        void MeasUnitSettingsDescribe()
        {
            foreach (var sett in MeasUnitSettings)
            {
                sett.Writing += SetMeasUnitsSettings;
            }
        }
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
        #region Подписка на события класса AdcBoardSettings
        /// <summary>
        /// Подписка на события класса AdcBoardSettings
        /// </summary>
        void AdcSettingsEventDescribe()
        {
            AdcBoardSettings.SettingsChangedEvent += SetAdcBoardSettings;
            AdcBoardSettings.AdcModeChangedEvent += SetAdcMode;
            AdcBoardSettings.AdcProcModeChangedEvent += SetAdcProcMode;
            AdcBoardSettings.AdcSyncLevelChangedEvent += SetAdcSyncLevel;
            AdcBoardSettings.AdcSyncModeChangedEvent += SetAdcSyncMode;
            AdcBoardSettings.PreampGainChangedEvent += SetPreampGain;
            AdcBoardSettings.TimerMaxChangedEvent += SetAdcTimerMax;
        }
        #endregion
        #endregion


        #endregion


        #region Данные прочитаны
        public bool SettingsReaded { get; set; }
        #endregion 
        #endregion

        public RS485 rs { get; private set; }
        public TCP Tcp { get; private set; }

        public async void ModelProcess()
        {
            await Task.Run(() => MainProcess());
        }

        void MainProcess()
        {
            while (true)
            {
                if (CommMode.RsEnable) rs?.GetData(this);
                else if (CommMode.EthEnable) Tcp?.GetData(this);
                else Connecting.Value = false;
                if (CycleMeasStatus.Value && Connecting.Value)
                    UpdateDataEvent?.Invoke();                
                Thread.Sleep(100);
            }
        }

        #region Инициализация
        void Init()
        {
            rs = ClassInit<RS485>();
            Tcp = ClassInit<TCP>();
            CommMode = ClassInit<CommMode>();
            CycleMeasStatus.PropertyChanged += (obj, args) =>
            {
                if (args.PropertyName == "Value")
                {                    
                    UpdateDataEvent?.Invoke();                    
                }
            };
            Connecting.PropertyChanged += (obj, args) => SettingsReaded = false;
           
        }

        T ClassInit<T>() where T : PropertyChangedBase
        {
            T cell = XmlMethods.GetParam<T>().FirstOrDefault();
            if (cell == null)
            {
                cell = (T)Activator.CreateInstance(typeof(T));
                XmlMethods.AddToXml<T>(cell);
            }
            cell.PropertyChanged += (sender, e) => XmlMethods.EditParam<T>(cell, e.PropertyName);
            return cell;
        }
        #endregion

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

        #region Команды
        #region Старт-стоп циклических измерений        
        public  void SwitchMeas()
        {
            var value = CycleMeasStatus.Value ? 0 : 1;            

            if (CommMode.EthEnable) Tcp.SwitchMeas(value);
            else if (CommMode.RsEnable) rs.SwitchMeas(value);
        }
        #endregion

        #region Включитть-выключить HV 
        public void SwitchHv()
        {
            var value = TelemetryHV.HvOn.Value ? 0 : 1;
            if (CommMode.EthEnable) Tcp.SwitchHv(value);
            else if (CommMode.RsEnable) rs.SwitchHv(value);
        }
        #endregion
    
        #region Настройки измерительных процессов

        #region Записать данные измерительных процессов
        public void WriteMeasProcSettings(string tcpArg, ushort measProcNum)
        {
           if (CommMode.EthEnable) Tcp.WriteMeasProcSettings(tcpArg, measProcNum);
        }

        #endregion
        
        #endregion

        #region Записать активности измерительных процессов
        void SetMeasProcActivity()
        {
            string cmd = "SETT,meas_prc_ndx=";
            for (int i = 0; i < MeasProcNum; i++)
            {
                if (MeasProcSettings[i].IsActive.WriteValue) cmd += $"{i},";
            }
            cmd = cmd.Remove(cmd.Length - 1) + "#";
            if (CommMode.EthEnable) Tcp.SetMeasProcActivity(cmd);
        }

        #endregion

        #region Команды настроек последовательного порта
        #region Записать бадрейт
        public void ChangeBaudrate(uint value)
        {
            if (CommMode.EthEnable) Tcp.ChangeBaudrate(value);
            else if ((CommMode.RsEnable)) rs.ChangeBaudrate(value);
        }
        #endregion       

        #region Изменить режим работы последовательного порта
        public void ChangeSerialSelect(int value)
        {
            ushort temp = (ushort)(value > 0 ? 1 : 0);
            if (CommMode.EthEnable) Tcp.ChangeSerialSelect(temp);
            else if ((CommMode.RsEnable)) rs.ChangeSerialSelect(temp);
            
        }
        #endregion
        #endregion

        #region Установить дату-время
        public void SetRtc(DateTime dt)
        {
            if (CommMode.EthEnable) Tcp.SetRtc(dt);
            else if (CommMode.RsEnable) rs.SetRtc(dt);
        }

        #endregion

        #region Установить напряжение HV
        public void SetHv(ushort value)
        {
            if (CommMode.EthEnable) Tcp.SetHv(value);
            else if (CommMode.RsEnable) rs.SetHv(value);
        }
        #endregion

        #region Управление питанием аналоговых модулей
        void SwitchAm(int groupNum, int moduleNum, bool value)
        {
            if (CommMode.EthEnable) Tcp.SwitchAm(groupNum, moduleNum, value);
            else if (CommMode.RsEnable) rs.SwitchAm(groupNum, moduleNum, value);
        }
        #endregion

        #region Отправить значение тестовой величины
        void SetTestValueAm(int groupNum, int moduleNum, ushort value)
        {
            if (CommMode.EthEnable) Tcp.SetTestValueAm(groupNum, moduleNum, value);
            else if (CommMode.RsEnable) rs.SetTestValueAm(groupNum, moduleNum, value);
        }
        #endregion

        #region Команда "Изменить активность аналогового выхода"
        void ChangeDacAct(int groupNum, int moduleNum, AnalogOutput value)
        {
            if (CommMode.EthEnable) Tcp.SendAnalogOutSwttings(groupNum, moduleNum, value);
            else if (CommMode.RsEnable) rs.SendAnalogOutSwttings(groupNum, moduleNum, value);
        }
        #endregion

        #region Команда "Изменить активность аналогового входа"
        void ChangeAdcAct(int groupNum, int moduleNum, AnalogInput value)
        {
            if (CommMode.EthEnable) Tcp.SendAnalogInSwttings(groupNum, moduleNum, value);
            else if (CommMode.RsEnable) rs.SendAnalogInSwttings(groupNum, moduleNum, value);
        }
        #endregion        

        #region Команда "Произвести стандартизацию"
        /// <summary>
        /// Произвести стандартизацию
        /// </summary>
        /// <param name="index">Номер набора стандартизации</param>
        public void MakeStand(ushort measProcNum, ushort standNum)
        {
            if (CommMode.EthEnable) Tcp.MakeStand(measProcNum, standNum);            
        }
        #endregion

        #region Команда принудиельного запроса набора стандартизации после стандартизации
        public void GetStdSelection(ushort index)
        {
            if (CommMode.EthEnable) Tcp.GetMeasSettingsExternal(index);
            else if (CommMode.RsEnable) rs.GetStdSelection(index);
        }
        #endregion

        #region Команда "Записать настройки счечиков"
        public void WriteCounterSettings(CountDiapasone diapasone)
        {
            if (diapasone.Num.Value < MainModel.CountStand)
            {
                if (CommMode.EthEnable) Tcp.WriteCounterSettings(diapasone);
                else if (CommMode.RsEnable) rs.WriteCounterSettings(diapasone);
            }
        }
        #endregion        

        #region Команда "Поменять UDP адрес источника"
        public void SetUdpAddr(byte[] addr,int portNum)
        {
           if (CommMode.EthEnable) Tcp.SetUdpAddr(addr, portNum);
           //else if (CommMode.RsEnable) rs.SetUdpAddr(addr);
        }

        #endregion

        #region Команды изменения настроек платы АЦП
        public void SetAdcBoardSettings(AdcBoardSettings settings)
        {            
            if(CommMode.RsEnable) rs.SetAdcBoardSettings(settings);
        }
        public void SetAdcMode(ushort value)
        {
            if (CommMode.EthEnable) Tcp.SetAdcMode(value);
        }
        public void SetAdcProcMode(ushort value)
        {
            if (CommMode.EthEnable) Tcp.SetAdcProcMode(value);
        }
        public void SetAdcSyncMode(ushort value)
        {
            if (CommMode.EthEnable) Tcp.SetAdcSyncMode(value);
        }
        public void SetAdcSyncLevel(ushort value)
        {
            if (CommMode.EthEnable) Tcp.SetAdcSyncLevel(value);
        }
        public void SetAdcTimerMax(ushort value)
        {
            if (CommMode.EthEnable) Tcp.SetAdcTimerMax(value);
        }
        public void SetPreampGain(ushort value)
        {
            if (CommMode.EthEnable) Tcp.SetPreampGain(value);
        }
        #endregion

        #region Команда "Запуск-останов платы АЦП"
        public void SwitchAdcBoard(ushort value)
        {
            if (CommMode.EthEnable) Tcp.SwitchAdcBoard(value);
            else if (CommMode.RsEnable) rs.SwitchAdcBoard(value);
        }
        #endregion

        #region Команда "Запуск/останов выдачи данных АЦП "
        public void StartStopAdcData(ushort value)
        {
            if (CommMode.EthEnable) Tcp.StartStopAdcData(value);
            else if (CommMode.RsEnable) rs.StartStopAdcData(value);
        }
        #endregion

        #region Команда "Произвести еденичное измеренине"
        public void MakeSingleMeasure(int time, ushort measProcNdx, ushort index)
        {
            if (CommMode.EthEnable) Tcp.MakeSingleMeasure((ushort)time, measProcNdx, index);
            //else if (CommMode.RsEnable) rs.MakeSingleMeasure((ushort)time);
        }
        #endregion

        #region Команда "Записать настройки едениц измерерия"
        public void SetMeasUnitsSettings(MeasUnitSettings settings)
        {
            if (CommMode.EthEnable) Tcp.SetMeasUnitsSettings(settings);
            else if (CommMode.RsEnable) rs.SetMeasUnitsSettings(settings);
        }

        #endregion

        #region Команда "Переключить реле"
        public void SwitchRelay(ushort value)
        {
            if (CommMode.EthEnable) Tcp.SwitchRelay(value);
            //else if (CommMode.RsEnable) rs.SetMeasUnitsSettings(settings);
        }
        #endregion

        #region Команда "Перезагрузить плату"
        public void RstBoard()
        {
            if (CommMode.EthEnable) Tcp.RstBoard();
        }
        #endregion
        #endregion

    }
}
