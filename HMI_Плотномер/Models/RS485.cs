using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using EasyModbus;
using HMI_Плотномер.AddClasses;

namespace HMI_Плотномер.Models
{
    /// <summary>
    /// Представляет набор свойств и методов, необходимых для связи с платой по RS485 (Modbus RTU)
    /// </summary>
    class RS485: PropertyChangedBase
    {
        #region Конструктор
        public RS485(MainModel model)
        {
            this.model = model;
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
        string _comName;
        public string ComName { get => _comName; set => Set(ref _comName, value); }
        #endregion

        #region Баудрейт
        int _baudrate = 115200;
        public int BaudRate { get => _baudrate; set => Set(ref _baudrate, value); }
        #endregion        

        #region Проверка четности
        Parity _parity = Parity.None;
        public Parity Par { get => _parity; set => Set(ref _parity, value); }
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
        int[] holdRegs = new int[40];
        #endregion
        #region Массив Reading регистров
        /// <summary>
        /// Массив Reading регистров
        /// </summary>
        int[] readRegs = new int[40];
        #endregion
        #endregion

        public void GetData()
        {
            try
            {
                if ((client==null) || (!client.Connected)) PortInit();
                else
                {
                    readRegs = client.ReadInputRegisters(0, 20);
                    model.Connecting = true;
                    model.Rtc = new DateTime(2000 + readRegs[0], readRegs[1], readRegs[2], readRegs[3], readRegs[4], readRegs[5]);
                }                    
            }
            catch (Exception ex)
            {

                client.Disconnect();
                model.Connecting = false;
            }
        }

        void PortInit()
        {
            client = new ModbusClient("COM2");
            client.Baudrate = 115200;
            client.UnitIdentifier = MbAddr;
            client.Parity = System.IO.Ports.Parity.None;                   
            client.Connect();            
        }




    }
}
