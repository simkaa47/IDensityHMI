using IDensity.AddClasses;
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
        public MainModel()
        {
            Init();// Инициализация параметров
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
        #region ФВ усредненное по диапазонам мгновенное
        public Parameter<float> PhysValueCur { get; } = new Parameter<float>("PhysValueCur", "ФВ усредненное по диапазонам мгновенное", 0, float.PositiveInfinity, 0, "read");

        #endregion

        #region ФВ усредненное по диапазонам усредненное
        public Parameter<float> PhysValueAvg { get; } = new Parameter<float>("PhysValueAvg", "ФВ усредненное по диапазонам усредненное", 0, float.PositiveInfinity, 2, "read");
        #endregion

        #region Статус циклических измерений
        public Parameter<bool> CycleMeasStatus { get; } = new Parameter<bool>("CycleMeasStatus", "Статус циклических измерений", false, true, 1, "hold");
        #endregion

        #region Данные тлеметрии HV
        public HvTelemetry TelemetryHV { get; } = new HvTelemetry();
        #endregion        

        #region Данные телеметрии платы питания(темпратуры)
        public TempBoardTelemetry TempTelemetry { get; } = new TempBoardTelemetry();
        #endregion

        #region Номер текущкго измерительного процесса
        public Parameter<ushort> CurMeasProcessNum { get; } = new Parameter<ushort>("CurMeasProcessNum", "Номер текущего измерительного процесса", 0, (ushort)MainModel.measProcessNum, 25, "hold");
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

        #region Настройки измерительных процессов
        #region Количество измерительных процессов
        /// <summary>
        /// Количество измерительных процессов
        /// </summary>
        public static int measProcessNum = 4;
        #endregion

        #region Текущий процесс
        MeasProcess _curMeasProcess = new MeasProcess();
        public MeasProcess CurMeasProcess { get => _curMeasProcess; set => Set(ref _curMeasProcess, value); }
        #endregion

        #region Настройки измерительных процессов
        public MeasProcess[] MeasProcesses { get; set; } = Enumerable.Range(0, measProcessNum).Select(z => new MeasProcess()).ToArray();
        #endregion

        #endregion

        #region Данные стандартизаций

        public StandData[] StandSettings { get; } = Enumerable.Range(0, 12).Select(i => new StandData()).ToArray();
        #endregion

        #region Параметры полседовательного порта платы
        #region Баудрейт
        public Parameter<int> PortBaudrate { get; } = new Parameter<int>("PortBaudrate", "Скорость передачи данных", 1200, 115200, 48, "hold");
        #endregion

        #region Режим работы последовательного порта
        public Parameter<ushort> PortSelectMode { get; } = new Parameter<ushort>("PortSelectMode", "Режим работы последовательного порта", 0, 1, 50, "hold");
        #endregion
#endregion

        #region Данные прочитаны
        public bool SettingsReaded { get; set; }
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
                    PhysValueAvg.Value = 0;
                    PhysValueCur.Value = 0;
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
        public async void SwitchMeas()
        {
            var value = CycleMeasStatus.Value ? 0 : 1;
            if (value > 0 && !TelemetryHV.HvOn.Value)
            {

                SwitchHv();
                await Task.Run(() =>
                {
                    while (Connecting.Value && !TelemetryHV.HvOn.Value)
                    {
                        Thread.Sleep(100);
                    }
                });
            }

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
        public void SetMeasProcessSettings(MeasProcess process, int index)
        {
            if (CommMode.EthEnable) Tcp.SetMeasProcessSettings(process,index);
            else if((CommMode.RsEnable))rs.SetMeasProcessSettings(process, index);
        }

        #endregion

        #region Сменить номер измерительного процесса
        public void ChangeMeasProcess(int index)
        {
            if (CommMode.EthEnable) Tcp.ChangeMeasProcess(index);
            else if (CommMode.RsEnable) rs.ChangeMeasProcess(index);
        }
        #endregion
        #endregion

        #region Команды настроек последовательного порта
        #region Записать бадрейт
        public void ChangeBaudrate(int value)
        {            
            if (PortBaudrate.Value!= PortBaudrate.WriteValue)
            {
                if (CommMode.EthEnable) Tcp.ChangeBaudrate(value);
                else if ((CommMode.RsEnable)) rs.ChangeBaudrate(value);
            }
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

        #endregion


    }
}
