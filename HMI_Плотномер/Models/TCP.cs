using IDensity.AddClasses;
using IDensity.AddClasses.Standartisation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.Models
{
    class TCP : PropertyChangedBase
    {
        #region События
        public event Action<string> TcpEvent;
        #endregion

        #region Свойства
        #region IP адрес платы
        string _ip = "192.168.1.177";
        /// <summary>
        /// IP адрес платы
        /// </summary>
        public string IP
        {
            get => _ip;
            set
            {
                if (CheckIp(value)) Set(ref _ip, value);
            }
        }
        #endregion

        #region Номер порта
        int _portNum;
        public int PortNum { get => _portNum; set => Set(ref _portNum, value); }
        #endregion
        #endregion

        #region Поля       

        MainModel model;

        int indexAm = 0;

        #region Клиент
        TcpClient client;
        #endregion

        #region Буффер входящих данных
        /// <summary>
        /// Буффер входящих данных
        /// </summary>
        byte[] inBuf = new byte[2000];
        #endregion

        #region Stream
        NetworkStream stream;
        #endregion

        Queue<TcpWriteCommand> commands = new Queue<TcpWriteCommand>();
        #endregion

        #region Метод проверки корректности ip
        /// <summary>
        /// Проверка корректности ip
        /// </summary>
        /// <param name="ip">Проверяемая строка</param>
        /// <returns>true, если ip корректно</returns>
        bool CheckIp(string ip)
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

        #region Соединение
        void Connect()
        {
            commands?.Clear();
            client = new TcpClient();
            client.ReceiveTimeout = 2000;
            TcpEvent?.Invoke(($"Выполняется подключение к {IP}:{PortNum}"));
            client.Connect(IP, PortNum);

            model.Connecting.Value = client.Connected;
            TcpEvent?.Invoke(($"Произведено подключение к {IP}:{PortNum}"));
            stream = client.GetStream();

        }
        #endregion

        #region Дисконнект
        public void Disconnect()
        {
            if (client != null)
            {
               
                TcpEvent?.Invoke($"{IP}:{PortNum}: соединение завершено пользователем");
                client.Close();
                client.Dispose();
                model.Connecting.Value = client.Connected;
            }

        }
        #endregion        

        #region основной метод для получения данных из tcp соединения
        public void GetData(MainModel model)
        {
            try
            {
                this.model = model;
                if (client == null || !client.Connected)
                {
                    Connect();
                    return;
                }
                GetDeviceStatus();
                GetCurDateTime();
                GetCurMeas();
                GetPeriphTelemetry();
                //GetAmTelemetry();
                //GetHVTelemetry();
                //GetTempTelemetry();                
                GetSetiings();
                while (commands.Count > 0)
                {
                    var command = commands.Dequeue();
                    command.Action?.Invoke(command.Parameter);
                }

            }
            catch (Exception ex)
            {
                TcpEvent?.Invoke(ex.Message);
                commands?.Clear();
                Disconnect();
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region Получить данные телеметрии
        void GetAmTelemetry()
        {
            var numGroup = indexAm / 2;
            var numModule = indexAm % 2;
            var bytes = AskResponseBytes(Encoding.ASCII.GetBytes($"CMND,AMT,{numGroup},{numModule}"));
            if (bytes != 11) return;
            if (numModule == 0)// Если телеметрия аналогового выхода
            {
                model.AnalogGroups[numGroup].AO.VoltageTest.Value = (ushort)(inBuf[3] + inBuf[4] * 256);
                model.AnalogGroups[numGroup].AO.AdcValue.Value = (ushort)(inBuf[5] + inBuf[6] * 256);
                model.AnalogGroups[numGroup].AO.VoltageDac.Value = (ushort)(inBuf[7] + inBuf[9] * 256);
            }
            else// Если телеметрия аналогового входа
            {
                model.AnalogGroups[numGroup].AI.AdcValue.Value = (ushort)(inBuf[5] + inBuf[6] * 256);
            }
            indexAm = indexAm++ <= 3 ? indexAm : 0;

        }
        #endregion

        #region Отправить телеграмму без требования ответа без ожидания
        void SendTlg(byte[] buffer)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    StreamClear();
                    stream?.Write(buffer, 0, buffer.Length);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                TcpEvent?.Invoke(ex.Message);
                commands?.Clear();
                Disconnect();
            }

        }
        #endregion

        #region Функция выполняющая запрос и ожидающая ответа
        /// <summary>
        ///  Функция выполняющая запрос и ожидающая ответа
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        string AskResponse(byte[] buffer)
        {
            StreamClear();
            stream.Write(buffer, 0, buffer.Length);
            int num = 0;
            do
            {
                num = stream.Read(inBuf, 0, inBuf.Length);

            } while (stream.DataAvailable);
            model.Connecting.Value = true;
            Thread.Sleep(100);
            return Encoding.ASCII.GetString(inBuf, 0, num);// Получаем строку из байт;            
        }

        #endregion

        #region Получить ответ в байтах
        int AskResponseBytes(byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
            int num = 0;
            do
            {
                num = stream.Read(inBuf, 0, inBuf.Length);

            } while (stream.DataAvailable);
            Thread.Sleep(100);
            model.Connecting.Value = true;
            return num;
        }
        #endregion

        #region Запрос текущего значения даты времени
        void GetCurDateTime()
        {
            var str = AskResponse(Encoding.ASCII.GetBytes("CMND,DTG"));
            if (str.Length < 23) return;
            str = str.Substring(5, 17);
            var dt = new DateTime();
            if (DateTime.TryParse(str, out dt)) model.Rtc.Value = dt;
        }
        #endregion

        #region Получить данные периферийных модулей
        void GetPeriphTelemetry()
        {
            var str = AskResponse(Encoding.ASCII.GetBytes("CMND,PPG"));
            str = str.TrimEnd(new char[] { '#' }).Substring(5);
            float temp = 0;
            var nums = str.Split(new char[] { ',' })
                .Where(str => float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out temp)).
                Select(str => temp).
                ToArray();
            if (nums.Length == 12)
            {
                model.TempTelemetry.TempInternal.Value = nums[0]/10;
                model.TelemetryHV.VoltageCurIn.Value = nums[1];
                model.TelemetryHV.VoltageCurOut.Value = (ushort)nums[2];
                model.TelemetryHV.HvOn.Value = model.TelemetryHV.VoltageCurOut.Value > 100;
            }
        }
        #endregion

        #region Запрос текущего значения измерения
        void GetCurMeas()
        {
            var str = AskResponse(Encoding.ASCII.GetBytes("CMND,AMC"));
            str = str.TrimEnd(new char[] { '#' }).Substring(5);
            float temp = 0;
            var nums = str.Split(new char[] { ',' })
                .Where(str => float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out temp)).
                Select(str => temp).
                ToArray();
            if (nums.Length == 9)
            {
                model.PhysValueCur.Value = nums[6];
                model.PhysValueAvg.Value = nums[7];
                model.CycleMeasStatus.Value = nums[8] > 0 ? true : false;
            }
        }
        #endregion

        #region Запрос телеметрии от Платы HV
        void GetHVTelemetry()
        {
            int byteNum = AskResponseBytes(Encoding.ASCII.GetBytes("CMND,HVT"));
            if (byteNum != 13) return;
            model.TelemetryHV.VoltageSV.Value = (ushort)((inBuf[2] + inBuf[3] * 256) * 0.05);
            model.TelemetryHV.VoltageCurIn.Value = (inBuf[4] + (float)inBuf[5] * 256) / 1000;
            model.TelemetryHV.VoltageCurOut.Value = (ushort)((inBuf[6] + inBuf[7] * 256) * 0.05);
            model.TelemetryHV.Current.Value = (ushort)(inBuf[8] + inBuf[9] * 256);
            model.TelemetryHV.HvOn.Value = model.TelemetryHV.VoltageCurOut.Value > 100;
        }
        #endregion

        #region Запрос телеметрии от платы температуры
        void GetTempTelemetry()
        {
            int byteNum = AskResponseBytes(Encoding.ASCII.GetBytes("CMND,TET"));
            if (byteNum == 9)
            {
                model.TempTelemetry.TempExternal.Value = ((float)(BitConverter.ToInt16(inBuf, 2) - 2730)) / 10;
                model.TempTelemetry.TempInternal.Value = ((float)(BitConverter.ToInt16(inBuf, 4) - 2730)) / 10;
            }
        }
        #endregion

        #region Запрос статусов устройств
        void GetDeviceStatus()
        {
            var str = AskResponse(Encoding.ASCII.GetBytes("CMND,DSR"));
            ushort temp = 0;
            var strNums = str.Split(new char[] { ',', '#' }, StringSplitOptions.RemoveEmptyEntries).Where(s => ushort.TryParse(s, NumberStyles.HexNumber,null, out temp)).Select(s => temp).ToArray();
            if (strNums.Length != 4) return;
            model.CommStates.Value = strNums[0];            
            model.AnalogStateGroups[0].Value = strNums[2];
            model.AnalogStateGroups[1].Value = strNums[3];
            model.GetDeviceData();            
        }
        #endregion

        #region Получить данные настроек
        void GetSetiings()
        {
            if (!model.SettingsReaded)
            {
                GetMeasProcessData();// Получить данные процеса измерений
                GetStdSettings();
                GetSettings7();
            } 
            
        }
        #region Настройки измерительных процессов
        void GetMeasProcessData()
        {
            model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("CMND,FSR,6"));
            if (!CheckFsrdPacket(str)) return;//Проверка пакета
            ushort temp = 0;
            var strParts = str.Split(",meas_prc_ndx=");
            if (strParts.Length != 2) return;
            if (!ushort.TryParse(strParts[1].Replace("#",""), out temp)) return;
            model.CurMeasProcessNum.Value = temp;// Получаем номер текущего измерительного процесса           
            var numStr = strParts[0].Replace("*FSRD,6,meas_proc=", "").Split(new char[] { ',', '#' }, StringSplitOptions.RemoveEmptyEntries);
            if (numStr.Length != 19 * MainModel.measProcessNum) return;            
            for (int i = 0; i < 19*MainModel.measProcessNum; i += 19)
            {
                int index = i / 19;
                var numUshort = numStr
                     .Skip(i)
                     .Take(16)
                     .Where(s => ushort.TryParse(s, out temp))
                     .Select(n => temp)
                     .ToArray();
                if (numUshort.Length != 16) return;
                for (int j = 0; j < MeasProcess.rangeNum; j++)
                {
                    model.MeasProcesses[index].Ranges[j].CalibCurveNum.Value = numUshort[2 + j * 4];// Номер калибровочной кривой
                    model.MeasProcesses[index].Ranges[j].StandNum.Value = numUshort[3 + j * 4];// Номер стандартизации ЕИ
                    model.MeasProcesses[index].Ranges[j].CounterNum.Value = numUshort[4 + j * 4];// Счетчик
                }
                model.MeasProcesses[index].BackStandNum.Value = numUshort[13];
                model.MeasProcesses[index].MeasDuration.Value = numUshort[14];
                model.MeasProcesses[index].MeasDeep.Value = numUshort[15];
                float tempFloat = 0;
                var numsFloat = numStr.Skip(i+16)
                    .Take(3)
                    .Where(s => float.TryParse(s.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out tempFloat))
                    .Select(n => tempFloat)
                    .ToArray();
                if (numsFloat.Length!=3) return;
                model.MeasProcesses[index].HalfLife.Value = numsFloat[0];
                model.MeasProcesses[index].DensityLiquid.Value = numsFloat[1];
                model.MeasProcesses[index].DensitySolid.Value = numsFloat[2];
            }
            if (model.CurMeasProcessNum.Value < MainModel.measProcessNum) model.CurMeasProcess = model.MeasProcesses[model.CurMeasProcessNum.Value];
            model.SettingsReaded = true;
        }
        #endregion

        #region Настройки № 7
        void GetSettings7()
        {
            model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("CMND,FSR,7#"));
            var list = GetNumber("serial_baudrate", 1,1);
            if (list==null) return;
            model.PortBaudrate.Value = (int)list[0][0];
            list = GetNumber("hv_target", 1, 1);
            if (list == null) return;
            model.TelemetryHV.VoltageSV.Value = (ushort)(list[0][0]*0.05);
            list = GetNumber("serial_select", 1,1);
            if (list==null) return;
            model.PortSelectMode.Value = (ushort)list[0][0];
            list = GetNumber("am_out_sett", 7, 2);
            if (list == null) return;
            for (int i = 0; i < 2; i++)
            {
                model.AnalogGroups[i].AO.Activity.Value = (ushort)list[i][1];
                model.AnalogGroups[i].AO.DacType.Value = (ushort)list[i][2];
                model.AnalogGroups[i].AO.DacEiNdx.Value = (ushort)list[i][3];
                model.AnalogGroups[i].AO.DacVarNdx.Value = (ushort)list[i][4];
                model.AnalogGroups[i].AO.DacLowLimit.Value = list[i][5];
                model.AnalogGroups[i].AO.DacHighLimit.Value = list[i][6];

            }
            // локальная функция
            List<List<float>> GetNumber(string id, int parNum, int count)
            {
                var strTemp = str;
                float temp = 0;
                List<List<float>> list = new List<List<float>>();
                for (int i = 0; i < count; i++)
                {
                    int index = strTemp.LastIndexOf(id);
                    if (index == 0)
                    {
                        return null;
                    }
                    var sepStrs = strTemp.Substring(index, strTemp.Length - index).Split(new char[] { ',', '#' }, StringSplitOptions.RemoveEmptyEntries).Take(parNum);
                    var nums = sepStrs.SelectMany(s => s.Split("=", StringSplitOptions.RemoveEmptyEntries))
                        .Where(str => float.TryParse(str.Replace(".", ","), out temp))
                        .Select(str => temp).ToList();
                    if (nums == null || nums.Count != parNum)
                    {
                        return null;
                    }
                    list.Insert(0, nums);
                    strTemp = strTemp.Remove(index, strTemp.Length - index);
                }
                return list;
            }
            model.SettingsReaded = true;
        }
        #endregion

        #region Настройки стандартизации
        void GetStdSettings()
        {
            for (ushort i = 0; i < 12; i++)
            {
                if (!GetStdSelection(i))break;
            }
        }

        bool GetStdSelection(ushort num)
        {
            model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes($"CMND,ASR,{num}#"));
            if (str.Length < 10 || str.Substring(0, 4) != "*ASR" || str[str.Length - 1] != '#') return false;
            var parts = str.Substring(4).Split(new char[] { ',', '#' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 13) return false;
            ushort tempUshort = 0;
            model.StandSettings[num].Duration.Value = ushort.TryParse(parts[1], out tempUshort) ? tempUshort : model.StandSettings[num].Duration.Value;
            model.StandSettings[num].Type.Value = ushort.TryParse(parts[2], out tempUshort) ? tempUshort : model.StandSettings[num].Type.Value;
            float tempFloat = 0;
            model.StandSettings[num].Value.Value = float.TryParse(parts[3].Replace(".",","), out tempFloat) ? tempFloat : model.StandSettings[num].Value.Value;
            for (int i = 0; i < 8; i++)
            {
                model.StandSettings[num].Results[i].Value = float.TryParse(parts[4 + i].Replace(".", ","), out tempFloat) ? tempFloat : model.StandSettings[num].Results[i].Value;
            }
            var dateParts = parts[12].Split(":").Where(s => ushort.TryParse(s, out tempUshort)).Select(s => tempUshort).ToArray();
            if (dateParts.Length != 3) return false;
            dateParts[0] = (ushort)(2000 + dateParts[0]);
            dateParts[1] = dateParts[1] > 0 && dateParts[1] <= 12 ? dateParts[1] : (ushort)1;
            dateParts[2] = dateParts[2] > 0 && dateParts[2] <= 31 ? dateParts[2] : (ushort)1;
            model.StandSettings[num].Date.Value = new DateTime(dateParts[0], dateParts[1], dateParts[2]);
            model.SettingsReaded = true;
            return true;
        }
        #endregion

        #region Проверка корректности пакета FSRD
        /// <summary>
        /// Проверка корректности пакета FSRD
        /// </summary>
        bool CheckFsrdPacket(string str)
        {
            //Проверка корректности пришедшего пакета
            if (str.Length < 50) return false; ;// Проверка на длину
            if (str.Substring(1, 4) != "FSRD") return false; ;// Проверка на заголовок..
            if (str[str.Length - 1] != '#') return false; ;// Проверка на окончание
            return true;
        } 
        #endregion

        #endregion

        #region Команды
        #region Включить-выключить HV
        public void SwitchHv(int value)
        {
            commands.Enqueue(new TcpWriteCommand(SendTlg, Encoding.ASCII.GetBytes($"CMND,HVO,{value.ToString()}")));
        }
        #endregion

        #region Запуск-останов циклических измерений
        public void SwitchMeas(int value)
        {
            commands.Enqueue(new TcpWriteCommand(SendTlg, Encoding.ASCII.GetBytes($"CMND,AMM,{value.ToString()}")));
        }
        #endregion

        #region Записать данные измерений
        public void SetMeasProcessSettings(MeasProcess process, int index)
        {
            string cmd = $"SETT,meas_proc={index},";
            // Добавлям данные диапазонов
            for (int i = 0; i < process.Ranges.Length; i++)
            {
                cmd = cmd + $"{i},";
                cmd = cmd + $"{process.Ranges[i].CalibCurveNum.Value},";// Номер калибровочной кривой
                cmd = cmd + $"{process.Ranges[i].StandNum.Value},";// Номер стандартизации
                cmd = cmd + $"{process.Ranges[i].CounterNum.Value},";// Счетчик
            }
            cmd = cmd + $"{process.BackStandNum.Value},";
            cmd = cmd + $"{process.MeasDuration.Value},";
            cmd = cmd + $"{process.MeasDeep.Value},";
            cmd = cmd + $"{process.HalfLife.Value.ToString().Replace(",", ".")},";
            cmd = cmd + $"{process.DensityLiquid.Value.ToString().Replace(",", ".")},";
            cmd = cmd + $"{process.DensitySolid.Value.ToString().Replace(",", ".")},";            
            commands.Enqueue(new TcpWriteCommand((buf)=> { SendTlg(buf); model.SettingsReaded = false; }, Encoding.ASCII.GetBytes(cmd)));
        }
        #endregion

        #region Изменить номер текущего процесса
        public void ChangeMeasProcess(int index)
        {
            if(index>=0 && index<MainModel.measProcessNum)commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); model.SettingsReaded = false; }, Encoding.ASCII.GetBytes($"SETT,meas_prc_ndx={index}#")));
        }
        #endregion

        #region Изменить режим работы последовательного порта
        public void ChangeSerialSelect(ushort value)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); model.SettingsReaded = false; }, Encoding.ASCII.GetBytes($"SETT,serial_select={value}#")));
        }
        #endregion

        #region Изменить скорость последовательного порта
        public void ChangeBaudrate(int value)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); model.SettingsReaded = false; }, Encoding.ASCII.GetBytes($"SETT,serial_baudrate={value}#")));
        }
        #endregion

        #region Установить RTC
        public void SetRtc(DateTime dt)
        {
            var str = $"SETT,rtc_set={dt.Day},{dt.Month},{dt.Year%100},{dt.Hour},{dt.Minute},{dt.Second}#";
            commands.Enqueue(new TcpWriteCommand((buf) =>  SendTlg(buf), Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Уставка HV
        public void SetHv(ushort value)
        {
            var str = $"SETT,hv_target={value*20}#";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf);model.SettingsReaded = false; }, Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Управление питанием аналоговых модулей
        public void SwitchAm(int groupNum, int moduleNum, bool value)
        {
            var str = $"CMND,AMP,{groupNum},{moduleNum},{(value ? 1 : 0)}";
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Отправить значение тестовой величины
        public void SetTestValueAm(int groupNum, int moduleNum, ushort value)
        {
            var str = $"CMND,AMV,{groupNum},{moduleNum},{value}";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); model.SettingsReaded = false;}, Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Отправить настройки аналоговых выходов
        public void SendAnalogOutSwttings(int groupNum, int moduleNum, AnalogOutput value)
        {
            var str = $"SETT,am_out_sett={groupNum},{value.Activity.Value},{value.DacType.Value},{value.DacEiNdx.Value},{value.DacVarNdx.Value},{value.DacLowLimit.Value.ToString().Replace(",",".")},{value.DacHighLimit.Value.ToString().Replace(",", ".")}#";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); model.SettingsReaded = false; } , Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Записать настройки набора стандартизации
        public void WriteStdSettings(ushort index, StandData stand)
        {
            var str = $"SETT,std={index},{stand.Duration.Value},{stand.Type.Value},{stand.Value.Value.ToString().Replace(",",".")}";
            for (int i = 0; i < 8 ; i++)
            {
                str = str + $",{stand.Results[i].Value.ToString().Replace(",", ".")}";
            }
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetStdSelection(index); }, Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Команда "Произвести стандартизацию"
        /// <summary>
        /// Произвести стандартизацию
        /// </summary>
        /// <param name="index">Номер набора стандартизации</param>
        public void MakeStand(int index)
        {
            var str = $"CMND,ASM,index";
            commands.Enqueue(new TcpWriteCommand((buf) =>  SendTlg(buf), Encoding.ASCII.GetBytes(str + "#")));
        }
        #endregion

        #region Команда принудиельного запроса набора стандартизации после стандартизации
        public void GetStdSel(ushort index)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => GetStdSelection(index) , null));
        }
        #endregion

        #endregion

        #region Очистка буфера
        void StreamClear()
        {
            while (stream.DataAvailable)
            {
                stream.Read(inBuf, 0, 1);
            }
        }
        #endregion


    }
}
