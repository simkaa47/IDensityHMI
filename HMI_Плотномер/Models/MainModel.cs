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
            }  }
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

        #region Настройки измерительных процессов
        public MeasProcess[] MeasProcesses { get; set; } = Enumerable.Range(0, 4).Select(z => new MeasProcess()).ToArray();
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
                GetMeasProcessData();
                Thread.Sleep(500);
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
        
        }
        
        #endregion
        public void GetMeasProcessData()
        {

            var str = "*FSRD,6,meas_proc=0,0,0,4,0,1,0,4,1,2,20,20,20,0,10,10,345.000000,1,0,0,0,0,1,0,0,0,2,0,0,0,0,0,0,1345.000000,2,0,0,0,0,1,0,0,0,2,0,0,0,0,0,0,2345.000000,3,0,0,4,1,1,1,4,1,2,1,4,1,0,15,12,3345.000000,meas_prc_ndx=0#";
            //Проверка корректности пришедшего пакета
            if (str.Length < 50) return;// Проверка на длину
            if (str.Substring(1, 4) != "FSRD") return;// Проверка на заголовок..
            if (str[str.Length - 1] != '#') return;// Проверка на окончание
            ushort temp = 0;
            if (!ushort.TryParse(str[str.Length - 2].ToString(), out temp)) return;
            CurMeasProcessNum.Value = temp;// Получаем номер текущего измерительного процесса
            str = str.Remove(str.Length - 16, 16);// Обрезаем строку с конца         
            str = str.Remove(0, 18);// Обрезаем начало строки
            var numStr = str.Split(new char[] { ',', '#' });
            if (numStr.Length != 68) return;
            for (int i = 0; i < 68; i += 17)
            {
                int index = i / 17;
                var numUshort = numStr
                     .Skip(i)
                     .Take(16)
                     .Where(s => ushort.TryParse(s, out temp))
                     .Select(n => temp)
                     .ToArray();
                if (numUshort.Length != 16) return;
                for (int j = 0; j < 3; j++)
                {
                    MeasProcesses[index].Ranges[j].CalibCurveNum = numUshort[2 + j * 4];// Номер калибровочной кривой
                    MeasProcesses[index].Ranges[j].StandNum = numUshort[3 + j * 4];// Номер стандартизации ЕИ
                    MeasProcesses[index].Ranges[j].CounterNum = numUshort[4 + j * 4];// Счетчик
                }
                MeasProcesses[index].BackStandNum = numUshort[13];
                MeasProcesses[index].MeasDuration = numUshort[14];
                MeasProcesses[index].MeasDeep = numUshort[15];
                float tempFloat = 0;
                if (!float.TryParse(numStr[i + 16].Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out tempFloat)) return;
                MeasProcesses[index].HalfLife = tempFloat;
            }

        }
        #endregion

        #endregion


    }
}
