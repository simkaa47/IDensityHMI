using IDensity.AddClasses;
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

        #region Пауза между запросами
        private int _pause;

        public int Pause
        {
            get { return _pause; }
            set 
            {
                if (value >= 100) Set(ref _pause, value);
            }
        }

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
        void CloseConnection()
        {
            if (client != null)
            {               
                TcpEvent?.Invoke($"{IP}:{PortNum}: соединение завершено пользователем");
                client.Close();
                client.Dispose();                
            }
        }
        #endregion

        public void Disconnect()
        {
            commands.Enqueue(new TcpWriteCommand((buf) => CloseConnection(), null));
        }

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
                CloseConnection();
            }
            catch (Exception ex)
            {
                if (++errCommCount >= 3)
                {                    
                    commands?.Clear();
                    model.Connecting.Value = false;
                    CloseConnection();
                    Thread.Sleep(1000);
                    errCommCount = 0;
                }
                else Thread.Sleep(Pause);
                TcpEvent?.Invoke(ex.Message);
            }

        }
        #endregion

        #region Отправить телеграмму без требования ответа без ожидания
        void SendTlg(byte[] buffer)
        {
            StreamClear();
            stream?.Write(buffer, 0, buffer.Length);
            Thread.Sleep(Pause);
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
                Thread.Sleep(Pause);
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
            Thread.Sleep(Pause);
            model.Connecting.Value = true;
            return num;
        }
        #endregion

        #region Запрос текущего значения даты времени
        void GetCurDateTime()
        {
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,DTG#"));
            if (str.Length < 23) return;
            str = str.Substring(5, 17);
            var dt = new DateTime();
            if (DateTime.TryParse(str, out dt)) model.Rtc.Value = dt;
        }
        #endregion

        #region Получить данные периферийных модулей
        void GetPeriphTelemetry()
        {
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,PPG#"));
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
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,AMC#"));
            
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
                    var actve = nums[i + 4] > 0;
                    int j = i/5;
                    model.MeasResults[j].MeasProcessNum.Value = (ushort)nums[i];
                    model.MeasResults[j].CounterValue.Value = actve ? nums[i + 1] : 0;
                    model.MeasResults[j].PhysValueCur.Value = actve ? nums[i + 2] : 0;
                    model.MeasResults[j].PhysValueAvg.Value = actve ? nums[i + 3] : 0;
                  //  model.SetMeasResultData();
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
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,DSR#"));
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
                GetSettings8();
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
            var str = AskResponse(Encoding.ASCII.GetBytes($"*CMND,MPR,65536#"));
            var temp = 0;
            var mask = str.Split(new char[] { ',', '#' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => int.TryParse(s, out temp))
                .Select(s => temp)
                .FirstOrDefault();
            int max = 0;
            for (int i = 0; i < MainModel.MeasProcNum; i++)
            {
                if (i < 2) model.MeasResults[i].IsActive = false;
                model.MeasProcSettings[i].IsActive.Value = (mask & (int)Math.Pow(2,i)) > 0;
                if (model.MeasProcSettings[i].IsActive.Value && max <= 2)
                {
                    model.MeasResults[max].IsActive = true;
                    model.MeasResults[max].Settings = model.MeasProcSettings[i];
                    max++;
                }
            }
            
            model.SettingsReaded = true;

        }
        /// <summary>
        /// Прочитать набор измерительных процессов по номеру
        /// </summary>
        /// <param name="index"></param>
        public void GetMeasProcessData(int index)
        {
            model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes($"*CMND,MPR,{index}#"));
            var arr = GetNumericsFromString(str, new char[] { ',', '=', '#',':' });
            if (arr == null || arr.Length != 108) throw new Exception($"Сигнатура ответа на запрос настроек измерительных процессов №{index} не соответсвует заданной");
            model.MeasProcSettings[index].Num = (ushort)arr[0];
            model.MeasProcSettings[index].MeasProcCounterNum.Value = (ushort)arr[1];
            RecognizeStandDataFromArr(arr, index);
            RecognizeSingleMeasData(arr, index);
            RecognizeCalibrCurveFromArr(arr, index);
            RecognizeDensityFromArr(arr, model.MeasProcSettings[index].DensityLiq, 84);
            RecognizeDensityFromArr(arr, model.MeasProcSettings[index].DensitySol, 86);
            RecognizeCompensationFromArr(arr, model.MeasProcSettings[index].TempCompensation, 88);
            RecognizeCompensationFromArr(arr, model.MeasProcSettings[index].SteamCompensation, 95);
            model.MeasProcSettings[index].MeasType.Value = (ushort)arr[100];
            RecognizeFastChangeSett(arr, index);
            model.MeasProcSettings[index].MeasDuration.Value = (ushort)arr[105];
            model.MeasProcSettings[index].MeasDeep.Value = (ushort)arr[106];
            model.MeasProcSettings[index].OutMeasNum = model.MeasUnitSettings[(ushort)arr[107]];
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
                model.MeasProcSettings[num].MeasStandSettings[i].MeasUnit = model.MeasUnitSettings[(ushort)arr[6 + i * 8]];
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
            model.MeasProcSettings[num].CalibrCurve.MeasUnit = model.MeasUnitSettings[(ushort)arr[offset+1]];
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
            density.MeasUnit = model.MeasUnitSettings[(ushort)arr[offset]];
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
            compensation.D.Value = arr[offset + 6];
        }
        /// <summary>
        /// Метод распознаания настроек быстрого измерения
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="num"></param>
        void RecognizeFastChangeSett(float[] arr, int num)
        {
            model.MeasProcSettings[num].FastChange.Activity.Value = arr[103] > 0;
            model.MeasProcSettings[num].FastChange.Threshold.Value = (ushort)arr[104];
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
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FSR,1#"));
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
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FSR,2#"));
            var list = GetNumber("adc_proc_cntr", 3, MainModel.CountCounters, str);
            if (list == null) return;
            for (int i = 0; i < MainModel.CountCounters; i++)
            {
                model.CountDiapasones[i].Num.Value = (ushort)list[i][0];
                model.CountDiapasones[i].Start.Value = (ushort)list[i][1];
                model.CountDiapasones[i].Finish.Value = (ushort)list[i][2];
            }
            list = GetNumber("adc_proc_mode", 1, 1, str);
            if (list == null) return;
            model.AdcBoardSettings.AdcProcMode.Value = (ushort)list[0][0];
            list = GetNumber("adc_single_meas_time", 1, 1,str);
            if (list == null) return;
            foreach (var mp in model.MeasProcSettings)
            {
                mp.SingleMeasTime.Value = (ushort)list[0][0];
            }           
            model.SettingsReaded = true;
        }
        #endregion        

        #region Настройки 4
        void GetSettings4()
        {
            model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FSR,4#"));
            var indexOfEqual = str.LastIndexOf('=');
            if (indexOfEqual < 1) return;            
            var strArr = str.Substring(indexOfEqual+1).
                Split(new char[] { '#', ',', '=' })
                .ToArray();
            float temp = 0;
            if (strArr.Length <= 126) throw new Exception("Сигнатура ответа на запрос CMND,FSR,4# не соответствует ожидаемому!");
            for (int i = 0; i < 126; i+=6)
            {
                var id = i / 6;
                model.MeasUnitSettings[id].Id.Value = float.TryParse(strArr[i], out temp) ? (ushort)temp : default(ushort);
                model.MeasUnitSettings[id].MeasUnitClassNum.Value = float.TryParse(strArr[i+1], out temp) ? (ushort)temp : default(ushort);
                model.MeasUnitSettings[id].Type.Value = float.TryParse(strArr[i + 2], out temp) ? (ushort)temp : default(ushort);
                model.MeasUnitSettings[id].A.Value = float.TryParse(strArr[i+3].Replace(".",","), out temp) ? temp : default(float);
                model.MeasUnitSettings[id].B.Value = float.TryParse(strArr[i + 4].Replace(".", ","), out temp) ? temp : default(float);
                model.MeasUnitSettings[id].MeasUnitName.Value = strArr[i + 5];
            }

            model.SettingsReaded = true;
        }
        

        #endregion

        #region Настройки № 7
        void GetSettings7()
        {
            model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FSR,7#"));
            var list = GetNumber("serial_baudrate", 1,1, str);
            if (list==null) return;
            model.PortBaudrate.Value = (uint)list[0][0];
            list = GetNumber("hv_target", 1, 1, str);
            if (list == null) return;
            model.TelemetryHV.VoltageSV.Value = (ushort)(list[0][0]*0.05);
            list = GetNumber("serial_select", 1,1,str);
            if (list==null) return;
            model.PortSelectMode.Value = (ushort)list[0][0];
            list = GetNumber("am_out_sett", 10, 2, str);
            if (list == null) return;
            for (int i = 0; i < 2; i++)
            {
                model.AnalogGroups[i].AO.Activity.Value = (ushort)list[i][1];
                model.AnalogGroups[i].AO.DacType.Value = (ushort)list[i][2];
                model.AnalogGroups[i].AO.MeasUnit = model.MeasUnitSettings[(ushort)list[i][3]];
                model.AnalogGroups[i].AO.AnalogMeasProcNdx.Value = (ushort)list[i][4];
                model.AnalogGroups[i].AO.VarNdx.Value = (ushort)list[i][5];
                model.AnalogGroups[i].AO.DacLowLimit.Value = list[i][6];
                model.AnalogGroups[i].AO.DacHighLimit.Value = list[i][7];
                model.AnalogGroups[i].AO.DacLowLimitMa.Value = list[i][8];
                model.AnalogGroups[i].AO.DacHighLimitMa.Value = list[i][9];
            }
            list = GetNumber("am_in_sett", 2, 2, str);
            if (list == null) return;
            model.AnalogGroups[0].AI.Activity.Value = (ushort)list[0][1];
            model.AnalogGroups[1].AI.Activity.Value = (ushort)list[1][1]; 
            list = GetNumber("preamp_gain", 1, 1, str);
            if (list == null) return;
            model.AdcBoardSettings.PreampGain.Value = (ushort)list[0][0];           
            model.SettingsReaded = true;
        }
        #endregion

        #region Настройки №8
        void GetSettings8()
        {
            model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FSR,8#"));            
            // Номер порта
            var list = GetNumber("udp_sett", 5, 1, str);
            model.UdpAddrString = $"{list[0][0]}.{list[0][1]}.{list[0][2]}.{list[0][3]}";
            model.PortUdp = (int)list[0][4];
            model.SettingsReaded = true;
        }
        #endregion

        #region Метод вычленения числовых данных по тестовому индикатору в пакетах FSRD
        /// <summary>
        /// Метод вычленения числовых данных по тестовому индикатору в пакетах FSRD
        /// </summary>
        /// <param name="id">Индентификатор данных</param>
        /// <param name="parNum">количество данных в одном наборе настроек</param>
        /// <param name="count">количество наборов</param>
        /// <param name="str">источник данных</param>
        /// <returns></returns>
        List<List<float>> GetNumber(string id, int parNum, int count, string str)
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
        #endregion

        #region Команды


        #region Включить-выключить HV
        public void SwitchHv(int value)
        {
            commands.Enqueue(new TcpWriteCommand(SendTlg, Encoding.ASCII.GetBytes($"*CMND,HVO,{value.ToString()}#")));
        }
        #endregion

        #region Запуск-останов циклических измерений
        public void SwitchMeas(int value)
        {
            commands.Enqueue(new TcpWriteCommand(SendTlg, Encoding.ASCII.GetBytes($"*CMND,AMM,{value.ToString()}#")));
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
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); model.SettingsReaded = false; }, Encoding.ASCII.GetBytes($"*SETT,serial_select={value}#")));
        }
        #endregion

        #region Изменить скорость последовательного порта
        public void ChangeBaudrate(uint value)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); model.SettingsReaded = false; }, Encoding.ASCII.GetBytes($"*SETT,serial_baudrate={value}#")));
        }
        #endregion

        #region Установить RTC
        public void SetRtc(DateTime dt)
        {
            var str = $"*SETT,rtc_set={dt.Day},{dt.Month},{dt.Year%100},{dt.Hour},{dt.Minute},{dt.Second}#";
            commands.Enqueue(new TcpWriteCommand((buf) =>  SendTlg(buf), Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Уставка HV
        public void SetHv(ushort value)
        {
            var str = $"*SETT,hv_target={value*20}#";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf);model.SettingsReaded = false; }, Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Управление питанием аналоговых модулей
        public void SwitchAm(int groupNum, int moduleNum, bool value)
        {
            var str = $"*CMND,AMP,{groupNum},{moduleNum},{(value ? 1 : 0)}#";
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Отправить значение тестовой величины
        public void SetTestValueAm(int groupNum, int moduleNum, ushort value)
        {
            var str = $"*CMND,AMV,{groupNum},{value}#";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); model.SettingsReaded = false;}, Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Отправить настройки аналоговых выходов
        public void SendAnalogOutSwttings(int groupNum, int moduleNum, AnalogOutput value)
        {
            var str = $"*SETT,am_out_sett={groupNum},{value.Activity.Value},{value.DacType.Value},{value.MeasUnit.Id.Value},{value.AnalogMeasProcNdx.Value},{value.VarNdx.Value},{value.DacLowLimit.Value.ToStringPoint()},{value.DacHighLimit.Value.ToStringPoint()},{value.DacLowLimitMa.Value.ToStringPoint()},{value.DacHighLimitMa.Value.ToStringPoint()}#";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetSettings7(); } , Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Отправить настройки аналоговых входов
        public void SendAnalogInSwttings(int groupNum, int moduleNum, AnalogInput value)
        {
            var str = $"*SETT,am_in_sett={groupNum},{value.Activity.Value}#";
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
            var str = $"*CMND,ASM,{measProcNum},{standNum}";
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
            var str = $"*SETT,adc_proc_cntr={diapasone.Num.Value},{diapasone.Start.Value},{diapasone.Finish.Value}";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetSettings2(); }, Encoding.ASCII.GetBytes(str + "#")));
        }
        #endregion        

        #region Команда "Поменять UDP адрес источника"
        public void SetUdpAddr(byte[] addr, int portNum)
        {
            if (addr.Length != 4) return;
            var str = $"*SETT,udp_sett=";
            for (int i = 0; i < 4; i++) str = str + $"{addr[i]},";
            str += portNum.ToString();
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetSettings8(); }, Encoding.ASCII.GetBytes(str + "#")));
        }

        #endregion

        #region Команды изменения настроек платы АЦП
        public void SetAdcMode(ushort value)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"*SETT,adc_mode={value}#")));
            commands.Enqueue(new TcpWriteCommand((buf) => GetSettings1(), null));
        }
        public void SetAdcProcMode(ushort value)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"*SETT,adc_proc_mode={value}#")));
            commands.Enqueue(new TcpWriteCommand((buf) => GetSettings2(), null));
        }
        public void SetAdcSyncMode(ushort value)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"*SETT,adc_sync_mode={value}#")));
            commands.Enqueue(new TcpWriteCommand((buf) => GetSettings2(), null));
        }
        public void SetAdcSyncLevel(ushort value)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"*SETT,adc_sync_level={value}#")));
            commands.Enqueue(new TcpWriteCommand((buf) => GetSettings1(), null));
        }
        public void SetAdcTimerMax(ushort value)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"*SETT,timer_max={value}#")));
            commands.Enqueue(new TcpWriteCommand((buf) => GetSettings1(), null));
        }
        public void SetPreampGain(ushort value)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"*SETT,preamp_gain={value}#")));
            commands.Enqueue(new TcpWriteCommand((buf) => GetSettings7(), null));
        }

        #endregion

        #region Очистить спектр
        public void ClearSpectr()
        {
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes($"*CMND,CRS#")));
        }
        #endregion

        #region Команда "Запуск-останов платы АЦП"
        public void SwitchAdcBoard(ushort value)
        {
            var str = $"*CMND,ADC,{value}#";
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Команда "Запуск/останов выдачи данных АЦП "
        public void StartStopAdcData(ushort value)
        {
            var str = $"*CMND,DAT,{value}";
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str + "#")));
        }
        #endregion

        #region Команда "Произвести еденичное измеренине"
        public void MakeSingleMeasure(int time, ushort measProcNdx, ushort index)
        {
            var str = $"*CMND,AMS,{time},{measProcNdx},{index}#";
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str)));
        }
        #endregion
        
        #region Команда "Записать настройки едениц измерерия"
        public void SetMeasUnitsSettings(MeasUnitSettings settings)
        {
            var str = $"*SETT,meas_unit={settings.Id.WriteValue}" +
                $",{settings.MeasUnitClassNum.WriteValue}," +
                $"{settings.Type.WriteValue}," +
                $"{settings.A.WriteValue.ToStringPoint()}," +
                $"{settings.B.WriteValue.ToStringPoint()}," +
                settings.MeasUnitName.WriteValue.Replace(",", "").Substring(0, Math.Min(8, settings.MeasUnitName.WriteValue.Length));
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetSettings4(); }, Encoding.ASCII.GetBytes(str + "#")));
        }

        #endregion

        #region Команда "Переключить реле"
        public void SwitchRelay(ushort value)
        {
            var str = $"*CMND,RLS,{value}#";
            commands.Enqueue(new TcpWriteCommand((buf) =>SendTlg(buf), Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Команда "Перезагрузитьь плату"
        public void RstBoard()
        {
            var str = $"*CMND,RST#";
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str)));
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

        #endregion




    }
}
