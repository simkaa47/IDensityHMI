﻿using IDensity.AddClasses;
using IDensity.AddClasses.AdcBoardSettings;
using IDensity.AddClasses.Settings;
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
                if (MainModel.CheckIp(value)) Set(ref _ip, value);
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

        /// <summary>
        /// Количество ошибок соединений
        /// </summary>
        int errCommCount = 0;

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

        #region Соединение
        void Connect()
        {
            commands?.Clear();
            client = new TcpClient();
            client.ReceiveTimeout = 2000;
            TcpEvent?.Invoke(($"Выполняется подключение к {IP}:{PortNum}"));
            client.Connect(IP, PortNum);
            
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
                GetSetiings();
                while (commands.Count > 0)
                {
                    var command = commands.Dequeue();
                    command.Action?.Invoke(command.Parameter);
                    Thread.Sleep(200);
                }
                errCommCount = 0;
                model.Connecting.Value = client.Connected;
            }
            catch (Exception ex)
            {
                if (++errCommCount >= 3)
                {                    
                    commands?.Clear();
                    Disconnect();
                    Thread.Sleep(1000);
                    errCommCount = 0;
                }
                else Thread.Sleep(200);
                TcpEvent?.Invoke(ex.Message);
            }

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
            int offset = 0;
            do
            {
                num = stream.Read(inBuf, offset, inBuf.Length);
                Thread.Sleep(100);
                offset += num;

            } while (stream.DataAvailable);
            model.Connecting.Value = true;
           
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
                for (int i = 0; i < 2; i++)
                {
                    model.AnalogGroups[i].AO.VoltageDac.Value = (ushort)nums[4 + i*4];
                    model.AnalogGroups[i].AO.VoltageTest.Value = (ushort)nums[4 + i * 4 + 1];
                    model.AnalogGroups[i].AO.AdcValue.Value = (ushort)nums[4 + i * 4 + 2];
                    model.AnalogGroups[i].AI.AdcValue.Value = (ushort)nums[4 + i * 4 + 3];
                }
            }
        }
        #endregion

        #region Запрос текущего значения измерения
        void GetCurMeas()
        {
            var str = AskResponse(Encoding.ASCII.GetBytes("CMND,AMC"));
            
            float temp = 0;
            var nums = str.Split(new char[] { ',','#' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(str => float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out temp)).
                Select(str => temp).
                ToArray();
            if (nums.Length>0 && nums.Length%5==0)
            {
                model.CycleMeasStatus.Value = nums[4] > 0 ? true : false;
                for (int i = 0; i < nums.Length; i+=5)
                {
                    int j = i/5;
                    model.MeasResults[j].MeasProcessNum.Value = (ushort)nums[i];
                    model.MeasResults[j].CounterValue.Value = nums[i + 1];
                    model.MeasResults[j].PhysValueCur.Value = nums[i + 2];
                    model.MeasResults[j].PhysValueAvg.Value = nums[i + 3];
                    model.MeasResults[j].IsActive = nums[i + 4] > 0;
                    model.SetMeasResultData();
                }
                

            }
            else
            {
                model.CycleMeasStatus.Value = false;
                for (int i = 0; i < 2; i++)
                {
                    model.MeasResults[i].ClearResult();
                }
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
                GetMeasProcessDataAll();// Получить данные процеса измерений
                GetSettings2();
                GetSettings7();
                GetSettings1();
                GetSettings4();
            }             
        }
        #region Настройки измерительных процессов
        /// <summary>
        /// Метод получения всех данных изменительных настроек
        /// </summary>
        void GetMeasProcessDataAll()
        {
            for (int i = 0; i < MainModel.MeasProcNum; i++)
                GetMeasProcessData(i);
            ReadActivity();
        }
        /// <summary>
        /// Прочитать активности измерительных процессов
        /// </summary>
        void ReadActivity()
        {
            model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes($"CMND,MPR,65536#"));
            var temp = 0;
            var mask = str.Split(new char[] { ',', '#' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => int.TryParse(s, out temp))
                .Select(s => temp)
                .FirstOrDefault();
            int max = 0;
            for (int i = 0; i < MainModel.MeasProcNum; i++)
            {
                model.MeasProcSettings[i].IsActive.Value = (mask & (int)Math.Pow(2,i)) > 0;
                if (model.MeasProcSettings[i].IsActive.Value && max <= 2)
                {
                    model.MeasResults[max].IsActive = true;
                    max++;
                }
            }
            model.SetMeasResultData();
            model.SettingsReaded = true;

        }
        /// <summary>
        /// Прочитать набор измерительных процессов по номеру
        /// </summary>
        /// <param name="index"></param>
        public void GetMeasProcessData(int index)
        {
            model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes($"CMND,MPR,{index}#"));
            var arr = GetNumericsFromString(str, new char[] { ',', '=', '#',':' });
            if (arr == null || arr.Length != 106) throw new Exception($"Сигнатура ответа на запрос настроек измерительных процессов №{index} не соответсвует заданной");
            model.MeasProcSettings[index].Num = (ushort)arr[0];
            model.MeasProcSettings[index].MeasProcCounterNum.Value = (ushort)arr[1];
            RecognizeStandDataFromArr(arr, index);
            RecognizeSingleMeasData(arr, index);
            RecognizeCalibrCurveFromArr(arr, index);
            RecognizeDensityFromArr(arr, model.MeasProcSettings[index].DensityLiq, 84);
            RecognizeDensityFromArr(arr, model.MeasProcSettings[index].DensitySol, 86);
            RecognizeCompensationFromArr(arr, model.MeasProcSettings[index].TempCompensation, 88);
            RecognizeCompensationFromArr(arr, model.MeasProcSettings[index].SteamCompensation, 94);
            model.MeasProcSettings[index].MeasType.Value = (ushort)arr[100];
            RecognizeFastChangeSett(arr, index);
            model.MeasProcSettings[index].MeasDuration.Value = (ushort)arr[103];
            model.MeasProcSettings[index].MeasDeep.Value = (ushort)arr[104];
            model.MeasProcSettings[index].OutMeasNum.Value = (ushort)arr[105];
            model.SettingsReaded = true;
        }

        /// <summary>
        /// Функция распознавания данных стандартизации изм. процессов из массива
        /// </summary>
        /// <param name="arr">массив чисел</param>
        /// /// <param name="num">НОмер измерительного процесса</param>
        void RecognizeStandDataFromArr(float[] arr, int num)
        {
            for (int i = 0; i < MeasProcSettings.StandCount; i++)
            {
                model.MeasProcSettings[num].MeasStandSettings[i].Id = i;
                model.MeasProcSettings[num].MeasStandSettings[i].StandDuration.Value = (ushort)arr[2 + i * 8];
                int day = (ushort)arr[3 + i * 8];
                day = day > 0 && day <= 31 ? day : 1;
                int month = (ushort)arr[4 + i * 8];
                month = month > 0 && month <= 12 ? month : 1;
                int year = ((ushort)arr[5 + i * 8]) + 2000;
                model.MeasProcSettings[num].MeasStandSettings[i].LastStandDate.Value = new DateTime(year, month, day);
                model.MeasProcSettings[num].MeasStandSettings[i].StandMeasUnitNum.Value = (ushort)arr[6 + i * 8];
                model.MeasProcSettings[num].MeasStandSettings[i].StandResult.Value = arr[7 + i * 8];
                model.MeasProcSettings[num].MeasStandSettings[i].StandPhysValue.Value = arr[8 + i * 8];
                model.MeasProcSettings[num].MeasStandSettings[i].HalfLifeCorr.Value = arr[9 + i * 8];
            }
        }
        /// <summary>
        /// Функция распознавания данных калибр. кривой изм. процессов из массива
        /// </summary>
        /// <param name="arr">Массив чисел</param>
        /// <param name="num">Номер изм. процесса</param>
        void RecognizeCalibrCurveFromArr(float[] arr, int num)
        {
            var offset = 76;
            model.MeasProcSettings[num].CalibrCurve.Type.Value = (ushort)arr[offset];
            model.MeasProcSettings[num].CalibrCurve.MeasUnitNum.Value = (ushort)arr[offset+1];
            for (int i = 0; i < 6; i++)
            {
                model.MeasProcSettings[num].CalibrCurve.Coeffs[i].Value = arr[offset + 2 + i];
            }
        }

        /// <summary>
        /// Метод распознавания данных еденичного измерния из массива чисел
        /// </summary>
        /// <param name="arr">Массив чисел</param>
        /// <param name="num">Номер изм. процесса</param>
        void RecognizeSingleMeasData(float[] arr, int num)
        {
            for (int i = 26; i < MeasProcSettings.SingleMeasResCount*5+26; i+=5)
            {
                var j = (i - 26) / 5;
                int day = (ushort)arr[i];
                day = day > 0 && day <= 31 ? day : 1;
                int month = (ushort)arr[i+1];
                month = month > 0 && month <= 12 ? month : 1;
                int year = ((ushort)arr[i+2]) + 2000;
                model.MeasProcSettings[num].SingleMeasResults[j].Date.Value = new DateTime(year, month, day);
                model.MeasProcSettings[num].SingleMeasResults[j].Weak.Value = arr[i + 3];
                model.MeasProcSettings[num].SingleMeasResults[j].CounterValue.Value = arr[i + 4];
            }
        }
        /// <summary>
        /// Метод распознавания настроек плотности из данных измерительного процесса
        /// </summary>
        /// <param name="arr">Массив чисел</param>        
        /// /// <param name="offset">смещение данных в массиве</param>
        /// <param name="density">Класс, в который</param>
        void RecognizeDensityFromArr(float[] arr, DensitySett density, int offset)
        {
            density.MeasValueNum.Value = (ushort)arr[offset];
            density.PhysValue.Value = arr[offset+1];
        }

        /// <summary>
        /// Метод распознавания настроек компенсации
        /// </summary>
        /// <param name="arr">Массив чисел</param>
        /// <param name="compensation">Класс определяющий данные компенсации</param>
        /// <param name="offset">Смещение данных компенсации в массиве чисел</param>
        void RecognizeCompensationFromArr(float[] arr, Compensation compensation, int offset)
        {
            compensation.Activity.Value = arr[offset] > 0;
            compensation.MeasUnitNum.Value = (ushort)arr[offset + 1];
            compensation.Sourse.Value = (ushort)arr[offset + 2];
            compensation.A.Value = arr[offset + 3];
            compensation.B.Value = arr[offset + 4];
            compensation.C.Value = arr[offset + 5];
        }
        /// <summary>
        /// Метод распознаания настроек быстрого измерения
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="num"></param>
        void RecognizeFastChangeSett(float[] arr, int num)
        {
            model.MeasProcSettings[num].FastChange.Activity.Value = arr[101] > 0;
            model.MeasProcSettings[num].FastChange.Threshold.Value = (ushort)arr[102];
        }

        #endregion
        /// <summary>
        /// Выполняет разбиение строки по переданным в качестве параметра разделителя
        /// </summary>
        /// <param name="str">строка для разделения</param>
        /// /// <param name="seps">Массив разделителей</param>
        /// <returns>Массив чисел</returns>
        float[] GetNumericsFromString(string str, char[] seps)
        {
            float temp = 0;
            return str.Replace("inf", "∞").Split(seps, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => float.TryParse(s.Replace(".", ","), out temp))
                .Select(s => temp)
                .ToArray();
        }



        #region Настройки №1
        void GetSettings1()
        {
            model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("CMND,FSR,1#"));
            var list = GetNumber("adc_mode", 1, 1);
            if (list == null) return;
            model.AdcBoardSettings.AdcMode.Value = (ushort)list[0][0];
            list = GetNumber("adc_sync_mode", 1, 1);
            if (list == null) return;
            model.AdcBoardSettings.AdcSyncMode.Value = (ushort)list[0][0];
            list = GetNumber("adc_sync_level", 1, 1);
            if (list == null) return;
            model.AdcBoardSettings.AdcSyncLevel.Value = (ushort)list[0][0];
            list = GetNumber("timer_max", 1, 1);
            if (list == null) return;
            model.AdcBoardSettings.TimerMax.Value = (ushort)list[0][0];
            // локальная функция
            List<List<float>> GetNumber(string id, int parNum, int count)
            {
                var strTemp = str;
                float temp = 0;
                List<List<float>> list = new List<List<float>>();
                for (int i = 0; i < count; i++)
                {
                    int index = strTemp.LastIndexOf(id);
                    if (index < 1)
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

        #region Настройки № 2
        void GetSettings2()
        {
            model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("CMND,FSR,2#"));
            var list = GetNumber("adc_proc_cntr", 3, MainModel.CountCounters);
            if (list == null) return;
            for (int i = 0; i < MainModel.CountCounters; i++)
            {
                model.CountDiapasones[i].Num.Value = (ushort)list[i][0];
                model.CountDiapasones[i].Start.Value = (ushort)list[i][1];
                model.CountDiapasones[i].Finish.Value = (ushort)list[i][2];
            }
            list = GetNumber("adc_proc_mode", 1, 1);
            if (list == null) return;
            model.AdcBoardSettings.AdcProcMode.Value = (ushort)list[0][0];
            list = GetNumber("adc_single_meas_time", 1, 1);
            if (list == null) return;
            foreach (var mp in model.MeasProcSettings)
            {
                mp.SingleMeasTime.Value = (ushort)list[0][0];
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
                    if (index < 1)
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

        #region Настройки 4
        void GetSettings4()
        {
            model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("CMND,FSR,4#"));
            var indexOfEqual = str.LastIndexOf('=');
            if (indexOfEqual < 1) return;
            float temp = 0;
            var nums = str.Substring(indexOfEqual).
                Split(new char[] { '#', ',', '=' }, StringSplitOptions.RemoveEmptyEntries).
                Where(s => float.TryParse(s.Replace(".", ","), out temp)).
                Select(s => temp).ToArray();
            if (nums.Length != 15) return;
            for (int i = 0; i < 5; i++)
            {
                model.MeasUnitSettings[i].Id.Value = (ushort)i;
                model.MeasUnitSettings[i].A.Value = nums[i * 3 + 1];
                model.MeasUnitSettings[i].B.Value = nums[i * 3 + 2];
            }

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
            model.PortBaudrate.Value = (uint)list[0][0];
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
            list = GetNumber("am_in_sett", 2, 2);
            if (list == null) return;
            model.AnalogGroups[0].AI.Activity.Value = (ushort)list[0][1];
            model.AnalogGroups[1].AI.Activity.Value = (ushort)list[1][1];
            
           
            list = GetNumber("preamp_gain", 1, 1);
            if (list == null) return;
            model.AdcBoardSettings.PreampGain.Value = (ushort)list[0][0];
            // локальная функция
            List<List<float>> GetNumber(string id, int parNum, int count)
            {
                var strTemp = str;
                float temp = 0;
                List<List<float>> list = new List<List<float>>();
                for (int i = 0; i < count; i++)
                {
                    int index = strTemp.LastIndexOf(id);
                    if (index < 1)
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

        #region Записать данные измерений()
        public void WriteMeasProcSettings(string tcpArg, ushort measProcNum)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetMeasProcessData(measProcNum); ReadActivity(); }, Encoding.ASCII.GetBytes(tcpArg)));
        }
        #endregion     
        #region Записать активности измерительных процессов
        public void SetMeasProcActivity(string cmd)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); ReadActivity(); }, Encoding.ASCII.GetBytes(cmd)));
        }
        #endregion

        #region Изменить режим работы последовательного порта
        public void ChangeSerialSelect(ushort value)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); model.SettingsReaded = false; }, Encoding.ASCII.GetBytes($"SETT,serial_select={value}#")));
        }
        #endregion

        #region Изменить скорость последовательного порта
        public void ChangeBaudrate(uint value)
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
            var str = $"CMND,AMV,{groupNum},{value}#";
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

        #region Отправить настройки аналоговых входов
        public void SendAnalogInSwttings(int groupNum, int moduleNum, AnalogInput value)
        {
            var str = $"SETT,am_in_sett={groupNum},{value.Activity.Value}#";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); model.SettingsReaded = false; }, Encoding.ASCII.GetBytes(str)));
        }
        #endregion        

        #region Команда "Произвести стандартизацию"
        /// <summary>
        /// Произвести стандартизацию
        /// </summary>
        /// <param name="index">Номер набора стандартизации</param>
        public void MakeStand(int measProcNum, int standNum)
        {
            var str = $"CMND,ASM,{measProcNum},{standNum}";
            commands.Enqueue(new TcpWriteCommand((buf) =>  SendTlg(buf), Encoding.ASCII.GetBytes(str + "#")));
        }
        #endregion

        #region Команда принудиельного запроса набора стандартизации после стандартизации
        public void GetMeasSettingsExternal(int index)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => GetMeasProcessData(index) , null));
        }
        #endregion

        #region Команда "Записать настройки счечиков"
        public void WriteCounterSettings(CountDiapasone diapasone)
        {
            var str = $"SETT,adc_proc_cntr={diapasone.Num.Value},{diapasone.Start.Value},{diapasone.Finish.Value}";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetSettings2(); }, Encoding.ASCII.GetBytes(str + "#")));
        }
        #endregion        

        #region Команда "Поменять UDP адрес источника"
        public void SetUdpAddr(byte[] addr)
        {
            if (addr.Length != 4) return;
            var str = $"SETT,udp_dst_addr=";
            for (int i = 0; i < 4; i++) str = str + $"{addr[i]},";
            str.Remove(str.Length - 1, 1);

            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetSettings7(); }, Encoding.ASCII.GetBytes(str + "#")));
        }

        #endregion

        #region Команды изменения настроек платы АЦП
        public void SetAdcBoardSettings(AdcBoardSettings settings)
        {
            SwitchAdcBoard(0);            
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"SETT,adc_mode={settings.AdcMode.Value}#")));
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"SETT,adc_proc_mode={settings.AdcProcMode.Value}#")));
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"SETT,adc_sync_mode={settings.AdcSyncMode.Value}#")));
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"SETT,adc_sync_level={settings.AdcSyncLevel.Value}#")));
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"SETT,timer_max={settings.TimerMax.Value}#")));
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"SETT,preamp_gain={settings.PreampGain.Value}#")));
            SwitchAdcBoard(1);
            commands.Enqueue(new TcpWriteCommand((buf) => GetSettings1(), null));
            commands.Enqueue(new TcpWriteCommand((buf) => GetSettings2(), null));
            commands.Enqueue(new TcpWriteCommand((buf) => GetSettings7(), null));
        }
        #endregion

        #region Команда "Запуск-останов платы АЦП"
        public void SwitchAdcBoard(ushort value)
        {
            var str = $"CMND,ADC,{value}#";
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str + "#")));
        }
        #endregion

        #region Команда "Запуск/останов выдачи данных АЦП "
        public void StartStopAdcData(ushort value)
        {
            var str = $"CMND,DAT,{value}#";
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str + "#")));
        }
        #endregion

        #region Команда "Произвести еденичное измеренине"
        public void MakeSingleMeasure(int time, ushort measProcNdx, ushort index)
        {
            var str = $"CMND,AMS,{time*10},{measProcNdx},{index}#";
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str + "#")));
        }
        #endregion
        
        #region Команда "Записать настройки едениц измерерия"
        public void SetMeasUnitsSettings(MeasUnitSettings settings)
        {
            var str = $"SETT,meas_unit={settings.Id.Value},{settings.A.Value.ToString().Replace(",",".")},{settings.B.Value.ToString().Replace(",", ".")}";            
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetSettings4(); }, Encoding.ASCII.GetBytes(str + "#")));
        }

        #endregion

        #region Команда "Переключить реле"
        public void SwitchRelay(ushort value)
        {
            var str = $"CMND,RLS,{value}#";
            commands.Enqueue(new TcpWriteCommand((buf) =>SendTlg(buf), Encoding.ASCII.GetBytes(str)));
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
