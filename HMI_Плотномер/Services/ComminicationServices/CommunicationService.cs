using IDensity.AddClasses;
using IDensity.Models;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IDensity.Services.ComminicationServices
{
    public class CommunicationService
    {
        private readonly MainModel _mainModel;

        public CommunicationService(MainModel mainModel)
        {
            _mainModel = mainModel;
            Tcp = new TcpService(_mainModel);
        }
        #region События
        #region Обновились данные
        public event Action UpdateDataEvent;
        #endregion
        #endregion

        public TcpService Tcp { get; set; }

        Stopwatch stopWatch = new Stopwatch();

        #region Основной метод получения данных
        public async void MainProcessExecute()
        {
            await Task.Run(() =>
            {
                stopWatch.Start();
                while (true)
                {
                    Tcp.GetData();
                    if (_mainModel.Connecting.Value && stopWatch.ElapsedMilliseconds > 1000)
                    {
                        UpdateDataEvent?.Invoke();
                        stopWatch.Restart();
                    }
                }
            });
        }
        #endregion

        #region Записать общие настройки платы
        public void WriteCommonSettings(string arg)
        {
            Tcp.WriteCommonSettings(arg);
        }
        #endregion

        #region Управление переключением HV
        public void SwitchHv()
        {
            var value = _mainModel.TelemetryHV.HvOn.Value ? 0 : 1;
            Tcp.SwitchHv(value);            
        }
        #endregion

        #region Включение-выключение измерения
        public void SwitchMeas()
        {
            var value = _mainModel.CycleMeasStatus.Value ? 0 : 1;
            Tcp.SwitchMeas(value);            
        }
        #endregion

        #region Установить дату-время
        public void SetRtc(DateTime dt)
        {
            Tcp.SetRtc(dt);            
        }

        #endregion

        #region Команды настроек последовательного порта
        #region Записать бадрейт
        public void ChangeBaudrate(uint value)
        {
            Tcp.ChangeBaudrate(value);            
        }
        #endregion

        #region Изменить режим работы последовательного порта
        public void ChangeSerialSelect(int value)
        {
            ushort temp = (ushort)(value > 0 ? 1 : 0);
            Tcp.ChangeSerialSelect(temp);
        }
        #endregion
        #endregion

        #region Установить напряжение HV
        public void SetHv(ushort value)
        {
            Tcp.SetHv(value);            
        }
        #endregion

        #region Команда "Поменять UDP адрес источника"
        public void SetUdpAddr(byte[] addr, int portNum)
        {
            Tcp.SetUdpAddr(addr, portNum);            
        }

        #endregion

        #region Команда "Запуск-останов платы АЦП"
        public void SwitchAdcBoard(ushort value)
        {
            Tcp.SwitchAdcBoard(value);           
        }
        #endregion

        #region Команда "Запуск/останов выдачи данных АЦП "
        public void StartStopAdcData(ushort value)
        {
            Tcp.StartStopAdcData(value);           
        }
        #endregion

        #region Установить режим синхронизации данных АЦП
        public void SetAdcSyncMode(ushort value)
        {
            Tcp.SetAdcSyncMode(value);
        }
        #endregion

        #region Установить уровень синхронизации
        public void SetAdcSyncLevel(ushort value)
        {
            Tcp.SwitchAdcBoard(0);
            Tcp.SetAdcSyncLevel(value);
            Tcp.SwitchAdcBoard(1);
        }
        #endregion

        #region Установить таймер выдачи даннх
        public void SetAdcTimerMax(ushort value)
        {
            Tcp.SetAdcTimerMax(value);
        }
        #endregion

        #region Установить к-т предусиления
        public void SetPreampGain(ushort value)
        {
            Tcp.SetPreampGain(value);
        } 
        #endregion

        #region Установить режим выдачи данных АЦП
        public void SetAdcMode(ushort value)
        {
            Tcp.SetAdcMode(value);
        }
        #endregion

        #region Управление режимом (макс ампдитуд vs счетчики)
        public void SetAdcProcMode(ushort value)
        {
            Tcp.SetAdcProcMode(value);
        }
        #endregion

        #region запись параметров Ethernet параметров платы
        public void SetTcpSettings(string ip, string mask, string gateway)
        {
            Tcp.SetIPAddr(ip, mask, gateway);
        }
        #endregion

        #region Очистить спектр
        public void ClearSpectr()
        {
            Tcp.ClearSpectr();
        }
        #endregion

        #region Команда "Записать настройки счечиков"
        public void WriteCounterSettings(CountDiapasone diapasone)
        {
            if (diapasone.Num.Value < MainModel.CountStand)
            {
                Tcp.WriteCounterSettings(diapasone);                
            }
        }
        #endregion 

        #region Записать данные измерительных процессов
        public void WriteMeasProcSettings(string tcpArg, ushort measProcNum)
        {
            Tcp.WriteMeasProcSettings(tcpArg, measProcNum);
        }

        #endregion

        #region Записать активности измерительных процессов
        public void SetMeasProcActivity()
        {
            string cmd = "*SETT,meas_prc_ndx=";
            for (int i = 0; i < MainModel.MeasProcNum; i++)
            {
                if (_mainModel.MeasProcSettings[i].IsActive.WriteValue) cmd += $"{i},";
            }
            cmd = cmd.Remove(cmd.Length - 1) + "#";
            Tcp.SetMeasProcActivity(cmd);
        }

        #endregion

        #region Команда "Произвести еденичное измеренине"
        public void MakeSingleMeasure(int time, ushort measProcNdx, ushort index)
        {
            Tcp.MakeSingleMeasure((ushort)time, measProcNdx, index);
            
        }
        #endregion

        #region Команда "Произвести стандартизацию"
        /// <summary>
        /// Произвести стандартизацию
        /// </summary>
        /// <param name="index">Номер набора стандартизации</param>
        public void MakeStand(ushort measProcNum, ushort standNum)
        {
            Tcp.MakeStand(measProcNum, standNum);
        }
        #endregion

        #region Команда "Перезагрузить плату"
        public void RstBoard()
        {
            Tcp.RstBoard();
        }
        #endregion

        #region Управление питанием аналоговых модулей
        public void SwitchAm(int groupNum, int moduleNum, bool value)
        {
            Tcp.SwitchAm(groupNum, moduleNum, value);            
        }
        #endregion

        #region Отправить значение тестовой величины
        public void SetTestValueAm(int groupNum, int moduleNum, ushort value)
        {
            Tcp.SetTestValueAm(groupNum, moduleNum, value);
            
        }
        #endregion

        #region Команда "Изменить активность аналогового выхода"
        public void ChangeDacAct(int groupNum, int moduleNum, AnalogOutput value)
        {
            Tcp.SendAnalogOutSwttings(groupNum, moduleNum, value);           
        }
        #endregion

        #region Команда "Изменить активность аналогового входа"
        public void ChangeAdcAct(int groupNum, int moduleNum, AnalogInput value)
        {
            Tcp.SendAnalogInSwttings(groupNum, moduleNum, value);            
        }
        #endregion                

        #region Запрос контрольной суммы ПО плотномера
        public void GetCheckSum()
        {
            Tcp.GetResponce("*CMND,CHS#", (s) =>
            {
                if (string.IsNullOrEmpty(s)) return;
                uint temp = 0;
                var strNum = s.Replace("0x","")
                .Split(new char[] { ',', '#','x' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => uint.TryParse(s, NumberStyles.HexNumber, null, out temp))
                .Select(s => temp)
                .FirstOrDefault();
                _mainModel.CurCheckSum = strNum;

            }
            );
        }
        #endregion

    }
}
