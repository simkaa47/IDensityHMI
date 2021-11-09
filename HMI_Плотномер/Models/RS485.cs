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
        ushort[] holdRegs = new ushort[40];
        #endregion
        #region Массив Reading регистров
        /// <summary>
        /// Массив Reading регистров
        /// </summary>
        ushort[] readRegs = new ushort[80];
        #endregion
        #endregion

        #region Стэк для хранения команд, которые надо выполнить
        Stack<DoSomeClass> commands = new Stack<DoSomeClass>();

        class DoSomeClass
        {
            public DoSomeClass(Action<int,int> act, int p1,int p2)
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
                    commands.Push(new DoSomeClass(RecognizePacket, 0, 0));
                    // Прочитать данные Reading регистров
                    for (int i = 0; i < 5; i++) commands.Push(new DoSomeClass(ReadInputRegs, i * 15, 15));
                    // Прочитать данные Holding регистров
                    for (int i = 0; i < 2; i++) commands.Push(new DoSomeClass(ReadHoldRegs, i * 15, 15));

                    while (commands.Count > 0)
                    {
                        var command = commands.Pop();
                        command.f.Invoke(command.par1, command.par2);
                        model.Connecting = true;
                        Thread.Sleep(20);
                    }
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
            var hjh = SerialPort.GetPortNames();
            client = new ModbusClient("COM2");
            client.ConnectionTimeout = 500;
            client.Baudrate = 115200;
            client.UnitIdentifier = MbAddr;
            client.Parity = System.IO.Ports.Parity.None;
            client.Connect();            
        }
        #endregion

        #region Чтение input регистров
        void ReadInputRegs(int offset, int size)
        {
            var arr = client?.ReadInputRegisters(offset, size);
            Array.Copy(arr.Select(num => (ushort)num).ToArray(), 0, readRegs, offset, size);
        }
        #endregion

        #region Чтение холдинг регистров
        void ReadHoldRegs(int offset, int size)
        {
            var arr = client?.ReadHoldingRegisters(offset, size);
            Array.Copy(arr.Select(num => (ushort)num).ToArray(), 0, holdRegs, offset, size);
        } 
        #endregion

        #region Функция распознавания данных из  регистров
        void RecognizePacket(int p1, int p2)
        {
            GetRtc();
            GetCurMeas();
            GetHVTelemetry();
            GetTempTelemetry();
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

        #region Команды
        #region Включить-выключить HV
        public void SwitchHv(int value)
        {
            commands.Push(new DoSomeClass((offset, value) => client.WriteSingleRegister(offset, value), (int)Holds.SwitchHv, value));
        }
        #endregion

        #region Включить-выключить измерения
        public void SwitchMeas(int value)
        {
            commands.Push(new DoSomeClass((offset, value) => client.WriteSingleRegister(offset, value), (int)Holds.SwitchMeas, value));
        }
        #endregion

        #endregion



    }
}
