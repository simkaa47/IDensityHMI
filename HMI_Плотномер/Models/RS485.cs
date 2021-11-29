using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using EasyModbus;
using HMI_Плотномер.AddClasses;

namespace HMI_Плотномер.Models
{
    /// <summary>
    /// Представляет набор свойств и методов, необходимых для связи с платой по RS485 (Modbus RTU)
    /// </summary>
    class RS485: PropertyChangedBase
    {
        #region Перечисления номеров holding регистров, представляющих команды
        enum Holds
        { 
            SwitchMeas = 1,
            SwitchHv = 4
        }
        #endregion

        #region Свойства
        #region Адрес в сети Modbus
        byte _mbAddr = 1;
        public byte MbAddr
        {
            get => _mbAddr;
            set => Set(ref _mbAddr, value);
        }
        #endregion

        #region Название COM порта
        string _comName="COM1";
        public string ComName { get => _comName; set => Set(ref _comName, value); }
        #endregion        

        #region Баудрейт
        int _baudrate = 115200;
        public int BaudRate { get => _baudrate; set => Set(ref _baudrate, value); }
        #endregion        

        #region Проверка четности
        Parity _parity = Parity.None;
        public string Par { get => (_parity).ToString(); set => Set(ref _parity, GetParity(value)); }
        Parity GetParity(string name)
        {
            switch (name.ToLower())
            {
                case "even": return Parity.Even;
                case "odd": return Parity.Odd;
                default: return Parity.None;                    
            }
        }
        #endregion

        #endregion

        #region Поля
        #region Ссылка на модель
        MainModel model;
        #endregion
        ModbusClient client;
        #region Массив Holding регистров
        /// <summary>
        /// Массив Holding регистров
        /// </summary>
        ushort[] holdRegs = new ushort[100];
        #endregion
        #region Массив Reading регистров
        /// <summary>
        /// Массив Reading регистров
        /// </summary>
        ushort[] readRegs = new ushort[100];
        #endregion
        #endregion

        #region Номер процесса для выбора
        public Parameter<int> SelectMeasNum = new Parameter<int>("SelectMeasNum", "Номер процесса для выбора", 0, MainModel.measProcessNum - 1, 26, "hold");        
        #endregion

        #region Очередь для хранения команд, которые надо выполнить
        Queue<Command> commands = new Queue<Command>();

        class Command
        {
            public Command(Action<int,int> act, int p1,int p2)
            {
                f = act;
                par1 = p1;
                par2 = p2;
            }
            public Action<int, int> f;
            public int par1;
            public int par2;
        }
        #endregion

        #region Метод GetData  -  основной метод
        public void GetData(MainModel model)
        {
            this.model = model;
            try
            {
                if ((client == null) || (!client.Connected)) PortInit();
                else
                {
                    // Прочитать данные Reading регистров
                    for (int i = 0; i < 5; i++)ReadInputRegs( i * 15, 15);
                    // Прочитать данные Holding регистров
                    for (int i = 0; i < 2; i++)ReadHoldRegs(i * 15, 15);
                    RecognizePacket();
                    ReadSettings();// Чтение настроек, если надо

                    while (commands.Count > 0)
                    {
                        var command = commands.Dequeue();
                        command.f.Invoke(command.par1, command.par2);
                                              
                    }
                    model.Connecting = true;
                }
            }
            catch (Exception ex)
            {
                commands.Clear();
                client.Disconnect();
                model.Connecting = false;
            }
        }
        #endregion

        #region Инициализаця порта
        void PortInit()
        {           
            client = new ModbusClient(ComName);
            client.ConnectionTimeout = 500;
            client.Baudrate = BaudRate;
            client.UnitIdentifier = MbAddr;
            client.Parity = System.IO.Ports.Parity.None;
            client.Connect();            
        }
        #endregion

        #region Чтение input регистров
        void ReadInputRegs(int offset, int size)
        {
            var arr = client.ReadInputRegisters(offset, size);
            Array.Copy(arr.Select(num => (ushort)num).ToArray(), 0, readRegs, offset, size);
            //Thread.Sleep(20);
        }
        #endregion

        #region Чтение холдинг регистров
        void ReadHoldRegs(int offset, int size)
        {
            var arr = client?.ReadHoldingRegisters(offset, size);
            Array.Copy(arr.Select(num => (ushort)num).ToArray(), 0, holdRegs, offset, size);
            //Thread.Sleep(20);
        }
        #endregion

        #region Записть в регистры
        void WriteRegs(int offset, int size)
        {
            client.WriteMultipleRegisters(offset, holdRegs.Skip(offset).Take(size).Select(u16 => (int)u16).ToArray());
            Thread.Sleep(20);
        } 
        #endregion

        #region Функция распознавания данных из  регистров
        void RecognizePacket()
        {
            GetRtc();
            GetCurMeas();
            GetHVTelemetry();
            GetTempTelemetry();
            GetDeviceStatus();
        }
        #endregion

        #region Статусы устройств
        void GetDeviceStatus()
        {
            model.CommStates.Value = SelectRegs(model.CommStates.RegType)[model.CommStates.RegNum];
            model.AnalogStateGroup1.Value = SelectRegs(model.AnalogStateGroup1.RegType)[model.AnalogStateGroup1.RegNum]; 
            model.AnalogStateGroup2.Value = SelectRegs(model.AnalogStateGroup2.RegType)[model.AnalogStateGroup2.RegNum];
            model.GetDeviceData();
        } 
        #endregion

        #region Функция чтения времени-даты
        void GetRtc()
        {
            model.Rtc.Value = new DateTime(
                holdRegs[model.Rtc.RegNum]+2000,
                holdRegs[model.Rtc.RegNum+1],
                holdRegs[model.Rtc.RegNum+2],
                holdRegs[model.Rtc.RegNum+3],
                holdRegs[model.Rtc.RegNum+4],
                holdRegs[model.Rtc.RegNum+5]
                );
        }
        #endregion

        #region Получить данные текущих измерений
        void GetCurMeas()
        {                     
            model.PhysValueCur.Value = GetFloatFromUshorts(SelectRegs(model.PhysValueCur.RegType), model.PhysValueCur.RegNum);
            model.PhysValueAvg.Value = GetFloatFromUshorts(SelectRegs(model.PhysValueAvg.RegType), model.PhysValueAvg.RegNum);
            model.CycleMeasStatus.Value = SelectRegs(model.CycleMeasStatus.RegType)[model.CycleMeasStatus.RegNum] == 0 ? false : true;
        }
        
        ushort[] SelectRegs(string reg)
        {
            if (reg == "hold") return holdRegs;
            return readRegs;
        }
        #endregion

        #region Получить float из 2х регистров
        float GetFloatFromUshorts(ushort[] regs, int regNum)
        {
            ushort[] arr = new ushort[] { regs[regNum], regs[regNum + 1] };
            return BitConverter.ToSingle(arr.SelectMany(num => BitConverter.GetBytes(num)).ToArray(), 0);
        }
        #endregion

        #region Получить int32 из 2х регистров
        int GetInt32FromUshorts(ushort[] regs, int regNum)
        {
            try
            {
                ushort[] arr = new ushort[] { regs[regNum], regs[regNum + 1] };
                return BitConverter.ToInt32(arr.SelectMany(num => BitConverter.GetBytes(num)).ToArray(), 0);
            }
            catch (Exception)
            {
                return 0;
            }
           
           
        }
        #endregion

        #region Получить 2 регистра из float
        void GetUshortsFromFloat(ushort[] destination, int destinationIndex, float source)
        {
            var bytes = BitConverter.GetBytes(source);
            for (int i = 0; i < 2; i++)
            {
                destination[i + destinationIndex] = BitConverter.ToUInt16(bytes, i * 2);
            }  
        }
        #endregion

        #region Получить 2 регистра из int32
        void GetUshortsFromInt32(ushort[] destination, int destinationIndex, int source)
        {
            var bytes = BitConverter.GetBytes(source);
            for (int i = 0; i < 2; i++)
            {
                destination[i + destinationIndex] = BitConverter.ToUInt16(bytes, i * 2);
            }
        }
        #endregion

        #region Получить данные телеметрии HV
        void GetHVTelemetry()
        {
            model.TelemetryHV.VoltageSV.Value = (ushort)(SelectRegs(model.TelemetryHV.VoltageSV.RegType)[model.TelemetryHV.VoltageSV.RegNum]/20);
            model.TelemetryHV.VoltageCurIn.Value = ((float)SelectRegs(model.TelemetryHV.VoltageCurIn.RegType)[model.TelemetryHV.VoltageCurIn.RegNum]) / 1000;
            model.TelemetryHV.VoltageCurOut.Value = (ushort)(SelectRegs(model.TelemetryHV.VoltageCurOut.RegType)[model.TelemetryHV.VoltageCurOut.RegNum] / 20);
            model.TelemetryHV.HvOn.Value = model.TelemetryHV.VoltageCurOut.Value > 100;
        }
        #endregion

        #region Запрос телеметрии от платы температуры
        void GetTempTelemetry()
        {
            model.TempTelemetry.TempExternal.Value = ((float)(SelectRegs(model.TempTelemetry.TempExternal.RegType)[model.TempTelemetry.TempExternal.RegNum] - 2730)) / 10;
            model.TempTelemetry.TempInternal.Value = ((float)(SelectRegs(model.TempTelemetry.TempInternal.RegType)[model.TempTelemetry.TempInternal.RegNum] - 2730)) / 10;
        }
        #endregion

        #region Чтение настроек
        void ReadSettings()
        {
            if (!model.SettingsReaded)
            {
                // Прочитать данные измерительных процессов
                GetMeasProcessData();
                GetSerialSettings();
            }
        }
        #region Данные измерительных настроек
        void GetMeasProcessData()
        {
            model.SettingsReaded = false;
            // Читаем данные процессов
            for (int i = 0; i < MainModel.measProcessNum; i++)
            {
                client.WriteSingleRegister(SelectMeasNum.RegNum, i);
                for (int j = 0; j < 2; j++)
                {
                    ReadHoldRegs(j * 10 + MeasProcess.ModbRegNum, 10);
                }
                for (int j = 0; j < 3; j++)
                {
                    model.MeasProcesses[i].Ranges[j].CalibCurveNum.Value = holdRegs[MeasProcess.ModbRegNum + j * 3];
                    model.MeasProcesses[i].Ranges[j].StandNum.Value = holdRegs[MeasProcess.ModbRegNum + j * 3 + 1];
                    model.MeasProcesses[i].Ranges[j].CounterNum.Value = holdRegs[MeasProcess.ModbRegNum + j * 3 + 2];
                }
                model.MeasProcesses[i].BackStandNum.Value = holdRegs[MeasProcess.ModbRegNum + 9];
                model.MeasProcesses[i].MeasDuration.Value = holdRegs[MeasProcess.ModbRegNum + 10];
                model.MeasProcesses[i].MeasDeep.Value = holdRegs[MeasProcess.ModbRegNum + 11];
                model.MeasProcesses[i].HalfLife.Value = GetFloatFromUshorts(holdRegs, MeasProcess.ModbRegNum + 12);
                model.MeasProcesses[i].DensityLiquid.Value = GetFloatFromUshorts(holdRegs, MeasProcess.ModbRegNum + 14);
                model.MeasProcesses[i].DensitySolid.Value = GetFloatFromUshorts(holdRegs, MeasProcess.ModbRegNum + 16);
            }
            //Текущий номер измерений
            model.CurMeasProcessNum.Value = SelectRegs(model.CurMeasProcessNum.RegType)[model.CurMeasProcessNum.RegNum];
            if (model.CurMeasProcessNum.Value < MainModel.measProcessNum) model.CurMeasProcess = model.MeasProcesses[model.CurMeasProcessNum.Value];
            model.SettingsReaded = true;
        }
        #endregion

        #region Настройки порта
        void GetSerialSettings()
        {
            model.SettingsReaded = false;
            // Чтение баудрейта
            ReadHoldRegs(model.PortBaudrate.RegNum, 2);
            model.PortBaudrate.Value = GetInt32FromUshorts(holdRegs, model.PortBaudrate.RegNum);
            // Чтение режима работы
            ReadHoldRegs(model.PortSelectMode.RegNum, 1);
            model.PortBaudrate.Value = holdRegs[model.PortSelectMode.RegNum];
            model.SettingsReaded = true;
        }        
        #endregion
        #endregion

        #region Команды
        #region Включить-выключить HV
        public void SwitchHv(int value)
        {
            commands.Enqueue(new Command((offset, value) => client.WriteSingleRegister(offset, value), (int)Holds.SwitchHv, value));
        }
        #endregion

        #region Включить-выключить измерения
        public void SwitchMeas(int value)
        {
            commands.Enqueue(new Command((offset, value) => client.WriteSingleRegister(offset, value), (int)Holds.SwitchMeas, value));
        }
        #endregion

        #region Записать данные измерительного процеса
        public void SetMeasProcessSettings(MeasProcess process, int index)
        {
            commands.Enqueue(new Command((p1, p2) =>
            {
                model.SettingsReaded = false;
                //Записываем регистр для записи
                client.WriteSingleRegister(SelectMeasNum.RegNum, index);
                //Записываем данные процесса измерений
                for (int i = 0; i < 3; i++)// Данные диапазонов
                {
                    holdRegs[MeasProcess.ModbRegNum + i * 3] = process.Ranges[i].CalibCurveNum.Value;
                    holdRegs[MeasProcess.ModbRegNum + i * 3 + 1] = process.Ranges[i].StandNum.Value;
                    holdRegs[MeasProcess.ModbRegNum + i * 3 + 2] = process.Ranges[i].CounterNum.Value;
                }
                holdRegs[MeasProcess.ModbRegNum + 9] = process.BackStandNum.Value;
                holdRegs[MeasProcess.ModbRegNum + 10] = process.MeasDuration.Value;
                holdRegs[MeasProcess.ModbRegNum + 11] = process.MeasDeep.Value;
                GetUshortsFromFloat(holdRegs, MeasProcess.ModbRegNum + 12, process.HalfLife.Value);
                GetUshortsFromFloat(holdRegs, MeasProcess.ModbRegNum + 14, process.DensityLiquid.Value);
                GetUshortsFromFloat(holdRegs, MeasProcess.ModbRegNum + 16, process.DensitySolid.Value);
                for (int i = 0; i < 2; i++)
                {
                    WriteRegs(MeasProcess.ModbRegNum + i * 9, 9);
                }
            }, 0, 0));
            
        }
        #endregion

        #region Смена номера измерительного процесса
        public void ChangeMeasProcess(int index)
        {
            commands.Enqueue(new Command((p1, p2) => 
            {
                model.SettingsReaded = false;
                client?.WriteSingleRegister(model.CurMeasProcessNum.RegNum, index);
            }, 0, 0));            
        }
        #endregion

        #region Команды настроек последовательного порта
        #region Записать бадрейт
        public void ChangeBaudrate(int value)
        {
            commands.Enqueue(new Command((p1, p2) =>
            {
              model.SettingsReaded = false;
                GetUshortsFromInt32(holdRegs, model.PortBaudrate.RegNum, value);
                WriteRegs(model.PortBaudrate.RegNum, 2);
            }, 0, 0));
        }
        #endregion

        #region Изменить режим работы последовательного порта
        public void ChangeSerialSelect(ushort value)
        {
            commands.Enqueue(new Command((p1, p2) =>
            {
                model.SettingsReaded = false;
                client?.WriteSingleRegister(model.PortSelectMode.RegNum, value);
            }, 0, 0));
        }
        #endregion
        #endregion

        #region Установка даты-времени
        public void SetRtc(DateTime dt)
        {            
            commands.Enqueue(new Command((p1, p2) =>
            {
                if (model.Rtc.RegNum >= holdRegs.Length - 6) return;
                int i = model.Rtc.RegNum;
                holdRegs[i] = (ushort)(dt.Year % 100);
                holdRegs[i + 1] = (ushort)(dt.Month);
                holdRegs[i + 2] = (ushort)(dt.Day);
                holdRegs[i + 3] = (ushort)(dt.Hour);
                holdRegs[i + 4] = (ushort)(dt.Minute);
                holdRegs[i + 5] = (ushort)(dt.Second);
                WriteRegs(i, 6); 
            }, 0, 0));
        }
        #endregion

        #region Уставка HV
        public void SetHv(ushort value)
        {

        } 
        #endregion

        #endregion



    }
}
