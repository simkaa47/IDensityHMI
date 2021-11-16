using HMI_Плотномер.AddClasses;
using HMI_Плотномер.Models.XML;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HMI_Плотномер.Models
{
    class MainModel : PropertyChangedBase
    {
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
        public Parameter<DateTime> Rtc { get; } = new Parameter<DateTime>("Часы реального времени", "hold", 19);
        #endregion

        #region Статус соединения с платой
        bool _connecting;
        /// <summary>
        /// Статус соединения с платой
        /// </summary>
        public bool Connecting {
            get => _connecting;
            set
            {
                Set(ref _connecting, value);
                if (!value) SettingsReaded = false;
            } }
        #endregion

        #region Данные измерения
        #region ФВ усредненное по диапазонам мгновенное
        public Parameter<float> PhysValueCur { get; } = new Parameter<float>("ФВ усредненное по диапазонам мгновенное", "read", 0);
        #endregion

        #region ФВ усредненное по диапазонам мгновенное
        public Parameter<float> PhysValueAvg { get; } = new Parameter<float>("ФВ усредненное по диапазонам усредненное", "read", 2);
        #endregion

        #region Статус циклических измерений
        public Parameter<bool> CycleMeasStatus { get; } = new Parameter<bool>("Статус циклических измерений", "hold", 1);
        #endregion


        #endregion

        #region Данные тлеметрии HV
        public HvTelemetry TelemetryHV { get; } = new HvTelemetry();
        #endregion

        #region Данные телеметрии платы питания(темпратуры)
        public TempBoardTelemetry TempTelemetry { get; } = new TempBoardTelemetry();
        #endregion

        #region Номер текущкго измерительного процесса
        public Parameter<ushort> CurMeasProcessNum { get; } = new Parameter<ushort>("Номер текущего измерительного процесса", "hold", 25);
        #endregion

        #region Текущий процесс
        MeasProcess _curMeasProcess = new MeasProcess();
        public MeasProcess CurMeasProcess{ get => _curMeasProcess; set => Set(ref _curMeasProcess, value); }
        #endregion

        #region Настройки измерительных процессов
        #region Количество измерительных процессов
        /// <summary>
        /// Количество измерительных процессов
        /// </summary>
        public static int measProcessNum = 4;
        #endregion

        

        #region 

        #endregion
        public MeasProcess[] MeasProcesses { get; set; } = Enumerable.Range(0, measProcessNum).Select(z => new MeasProcess()).ToArray();
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
                if (CommMode.RsEnable) rs.GetData(this);
                else if (CommMode.EthEnable) Tcp.GetData(this);
                else Connecting = false;
                if (CycleMeasStatus.Value && Connecting)
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
                    while (Connecting && !TelemetryHV.HvOn.Value)
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
        }

        #endregion

        #region Сменить номер измерительного процесса
        public void ChangeMeasProcess(int index)
        {
            if (CommMode.EthEnable) Tcp.ChangeMeasProcess(index);
        }
        #endregion
        #endregion

        #endregion


    }
}
