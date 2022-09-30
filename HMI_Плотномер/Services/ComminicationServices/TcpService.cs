using IDensity.AddClasses;
using IDensity.AddClasses.Settings;
using IDensity.Core.AddClasses.Settings;
using IDensity.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IDensity.Services.ComminicationServices
{
    public class TcpService : PropertyChangedBase
    {

        public TcpService(MainModel model)
        {
            this._model = model;
        }

        #region События
        public event Action<string> TcpEvent;
        #endregion        

        #region Поля       

        MainModel _model;

        int indexAm = 0;

        /// <summary>
        /// Количество ошибок соединений
        /// </summary>
        int errCommCount = 0;

        #region Клиент
        public TcpClient Client { get; private set; }
        #endregion

        #region Буффер входящих данных
        /// <summary>
        /// Буффер входящих данных
        /// </summary>
        byte[] inBuf = new byte[10000];
        #endregion

        #region Stream
        public NetworkStream Stream { get; private set; }
        #endregion

        Queue<TcpWriteCommand> commands = new Queue<TcpWriteCommand>();
        #endregion        

        #region Соединение
        void Connect()
        {
            commands?.Clear();
            var ip = _model.TcpConnectData.IP;
            var port = _model.TcpConnectData.PortNum;
            Client = new TcpClient();
            Client.ReceiveTimeout = 2000;
            Client.SendTimeout = 2000;
            TcpEvent?.Invoke($"Выполняется подключение к {ip}:{port}");
            Client.Connect(ip, port);

            TcpEvent?.Invoke($"Произведено подключение к {ip}:{port}");
            Stream = Client.GetStream();

        }
        #endregion

        #region Дисконнект
        void CloseConnection()
        {
            var ip = _model.TcpConnectData.IP;
            var port = _model.TcpConnectData.PortNum;
            if (Client != null)
            {
                TcpEvent?.Invoke($"{ip}:{port}: соединение завершено пользователем");
                Client.Close();
                Client.Dispose();
            }
        }
        #endregion

        public void Disconnect()
        {
            commands.Enqueue(new TcpWriteCommand((buf) => CloseConnection(), null));
        }

        #region основной метод для получения данных из tcp соединения
        public void GetData()
        {
            try
            {                
                if (Client == null || !Client.Connected)
                {
                    Connect();
                    return;
                }
                while (commands.Count > 0)
                {
                    var command = commands.Dequeue();
                    command.Action?.Invoke(command.Parameter);
                    Thread.Sleep(_model.TcpConnectData.Pause);
                }
                if (_model.TcpConnectData.CycicRequest)
                {
                    GetDeviceStatus();
                    GetCurDateTime();
                    GetCurMeas();
                    GetPeriphTelemetry();

                }
                GetSetiings();

                errCommCount = 0;
                _model.Connecting.Value = Client.Connected;
                //if(CycicRequest)CloseConnection();
            }
            catch (Exception ex)
            {
                if (++errCommCount >= 3)
                {
                    commands?.Clear();
                    _model.Connecting.Value = false;
                    CloseConnection();
                    Thread.Sleep(1000);
                    errCommCount = 0;
                }
                else Thread.Sleep(_model.TcpConnectData.Pause);
                TcpEvent?.Invoke(ex.Message);
            }

        }
        #endregion

        #region Отправить телеграмму без требования ответа без ожидания
        void SendTlg(byte[] buffer)
        {
            StreamClear();
            Stream?.Write(buffer, 0, buffer.Length);
            Thread.Sleep(_model.TcpConnectData.Pause);
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
            Stream.Write(buffer, 0, buffer.Length);
            int num = 0;
            int offset = 0;
            do
            {
                num = Stream.Read(inBuf, offset, inBuf.Length - offset);
                Thread.Sleep(_model.TcpConnectData.Pause);
                offset += num;

            } while (Stream.DataAvailable && inBuf.Length - offset > 0);
            _model.Connecting.Value = true;

            return Encoding.ASCII.GetString(inBuf, 0, offset);// Получаем строку из байт;            
        }

        #endregion        

        #region Запрос текущего значения даты времени
        void GetCurDateTime()
        {
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,DTG#"));
            if (str.Length < 23) return;
            str = str.Substring(5, 17);
            var dt = new DateTime();
            if (DateTime.TryParse(str, out dt)) _model.Rtc.Value = dt;
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
                _model.TempTelemetry.TempInternal.Value = nums[0] / 10;
                _model.TelemetryHV.VoltageCurIn.Value = nums[1];
                _model.TelemetryHV.VoltageCurOut.Value = (ushort)nums[2];
                _model.TelemetryHV.HvOn.Value = _model.TelemetryHV.VoltageCurOut.Value > 100;
                for (int i = 0; i < 2; i++)
                {
                    _model.AnalogGroups[i].AO.VoltageDac.Value = (ushort)nums[4 + i * 4];
                    _model.AnalogGroups[i].AO.VoltageTest.Value = (ushort)nums[4 + i * 4 + 1];
                    _model.AnalogGroups[i].AO.AdcValue.Value = (ushort)nums[4 + i * 4 + 2];
                    _model.AnalogGroups[i].AI.AdcValue.Value = (ushort)nums[4 + i * 4 + 3];
                }
            }
        }
        #endregion

        #region Запрос текущего значения измерения
        void GetCurMeas()
        {
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,AMC#"));

            float temp = 0;
            var nums = str.Split(new char[] { ',', '#' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(str => float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out temp)).
                Select(str => temp).
                ToArray();
            if (nums.Length > 0 && nums.Length % 5 == 0)
            {
                _model.CycleMeasStatus.Value = nums[4] > 0 ? true : false;
                for (int i = 0; i < nums.Length; i += 5)
                {
                    var actve = nums[i + 4] > 0;
                    int j = i / 5;
                    _model.MeasResults[j].MeasProcessNum.Value = (ushort)nums[i];
                    _model.MeasResults[j].CounterValue.Value = actve ? nums[i + 1] : 0;
                    _model.MeasResults[j].PhysValueCur.Value = actve ? nums[i + 2] : 0;
                    _model.MeasResults[j].PhysValueAvg.Value = actve ? nums[i + 3] : 0;
                    //  model.SetMeasResultData();
                }


            }
            else
            {
                _model.CycleMeasStatus.Value = false;
                for (int i = 0; i < 2; i++)
                {
                    _model.MeasResults[i].ClearResult();
                }
            }
        }
        #endregion

        #region Запрос статусов устройств
        void GetDeviceStatus()
        {
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,DSR#"));
            ushort temp = 0;
            var strNums = str.Split(new char[] { ',', '#' }, StringSplitOptions.RemoveEmptyEntries).Where(s => ushort.TryParse(s, NumberStyles.HexNumber, null, out temp)).Select(s => temp).ToArray();
            if (strNums.Length != 4) return;
            _model.CommStates.Value = strNums[0];
            _model.AnalogStateGroups[0].Value = strNums[2];
            _model.AnalogStateGroups[1].Value = strNums[3];
            _model.PhysParamsState = strNums[1];
            _model.GetDeviceData();
        }
        #endregion

        #region Получить данные настроек
        void GetSetiings()
        {
            if (!_model.SettingsReaded)
            {
                GetMeasProcessDataAll();// Получить данные процеса измерений
                GetSettings2();
                GetSettings7();
                GetSettings1();
                GetSettings4();
                GetSettings8();
                GetSdLogStatus();
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
        #region Прочитать активности измерительных процессов
        /// <summary>
        /// Прочитать активности измерительных процессов
        /// </summary>
        void ReadActivity()
        {
            _model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes($"*CMND,MPR,65536#"));
            var temp = 0;
            var mask = str.Split(new char[] { ',', '#' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => int.TryParse(s, out temp))
                .Select(s => temp)
                .FirstOrDefault();
            int max = 0;
            for (int i = 0; i < MainModel.MeasProcNum; i++)
            {
                if (i < 2) _model.MeasResults[i].IsActive = false;
                _model.MeasProcSettings[i].IsActive.Value = (mask & (int)Math.Pow(2, i)) > 0;
                if (_model.MeasProcSettings[i].IsActive.Value && max < 2)
                {
                    _model.MeasResults[max].IsActive = true;
                    _model.MeasResults[max].Settings = _model.MeasProcSettings[i];
                    max++;
                }
            }

            _model.SettingsReaded = true;

        }
        #endregion

        #region Прочитать набор измерительных процессов по номеру
        /// <summary>
        /// Прочитать набор измерительных процессов по номеру
        /// </summary>
        /// <param name="index"></param>
        public void GetMeasProcessData(int index)
        {
            _model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes($"*CMND,MPR,{index}#"));
            var arr = GetNumericsFromString(str, new char[] { ',', '=', '#', ':' });
            if (arr == null || arr.Length != 120) throw new Exception($"Сигнатура ответа на запрос настроек измерительных процессов №{index} не соответсвует заданной");
            _model.MeasProcSettings[index].Num = (ushort)arr[0];
            _model.MeasProcSettings[index].MeasProcCounterNum.Value = (ushort)arr[1];
            RecognizeStandDataFromArr(arr, index);
            RecognizeSingleMeasData(arr, index);
            RecognizeCalibrCurveFromArr(arr, index);
            RecognizeDensityFromArr(arr, _model.MeasProcSettings[index].DensityLiqD1, 85);
            RecognizeDensityFromArr(arr, _model.MeasProcSettings[index].DensitySolD2, 87);
            RecognizeTempCompensation(arr, _model.MeasProcSettings[index], 89);
            RecognizeCompensationFromArr(arr, _model.MeasProcSettings[index].SteamCompensation, 101);
            _model.MeasProcSettings[index].MeasType.Value = (ushort)arr[106];
            RecognizeFastChangeSett(arr, index);
            _model.MeasProcSettings[index].MeasDuration.Value = arr[109] / 10;
            _model.MeasProcSettings[index].MeasDeep.Value = (ushort)arr[110];
            _model.MeasProcSettings[index].OutMeasNum = _model.MeasUnitSettings[(ushort)arr[111]];
            _model.MeasProcSettings[index].PipeDiameter.Value = arr[112] / 10;
            _model.MeasProcSettings[index].AttCoeffs[0].Value = arr[113];
            _model.MeasProcSettings[index].AttCoeffs[1].Value = arr[114];
            _model.MeasProcSettings[index].CalculationType.Value = (ushort)arr[115];
            GetVolumeCoeffsFromArr(arr, 116, index);
            _model.SettingsReaded = true;
        }
        #endregion

        #region Функция распознавания данных стандартизации изм. процессов из массива
        /// <summary>
        /// Функция распознавания данных стандартизации изм. процессов из массива
        /// </summary>
        /// <param name="arr">массив чисел</param>
        /// /// <param name="num">НОмер измерительного процесса</param>
        void RecognizeStandDataFromArr(float[] arr, int num)
        {
            for (int i = 0; i < MeasProcSettings.StandCount; i++)
            {
                _model.MeasProcSettings[num].MeasStandSettings[i].Id = i;
                _model.MeasProcSettings[num].MeasStandSettings[i].StandDuration.Value = (ushort)arr[2 + i * 8];
                int day = (ushort)arr[3 + i * 8];
                day = day > 0 && day <= 31 ? day : 1;
                int month = (ushort)arr[4 + i * 8];
                month = month > 0 && month <= 12 ? month : 1;
                int year = (ushort)arr[5 + i * 8] + 2000;
                _model.MeasProcSettings[num].MeasStandSettings[i].LastStandDate.Value = new DateTime(year, month, day);
                _model.MeasProcSettings[num].MeasStandSettings[i].MeasUnit = _model.MeasUnitSettings[(ushort)arr[6 + i * 8]];
                _model.MeasProcSettings[num].MeasStandSettings[i].StandResult.Value = arr[7 + i * 8];
                _model.MeasProcSettings[num].MeasStandSettings[i].StandPhysValue.Value = arr[8 + i * 8];
                _model.MeasProcSettings[num].MeasStandSettings[i].HalfLifeCorr.Value = arr[9 + i * 8];
            }
        }
        #endregion

        /// <summary>
        /// Функция распознавания данных калибр. кривой изм. процессов из массива
        /// </summary>
        /// <param name="arr">Массив чисел</param>
        /// <param name="num">Номер изм. процесса</param>
        void RecognizeCalibrCurveFromArr(float[] arr, int num)
        {
            var offset = 76;
            _model.MeasProcSettings[num].CalibrCurve.Type.Value = (ushort)arr[offset];
            _model.MeasProcSettings[num].CalibrCurve.MeasUnit = _model.MeasUnitSettings[(ushort)arr[offset + 1]];
            for (int i = 0; i < 6; i++)
            {
                _model.MeasProcSettings[num].CalibrCurve.Coeffs[i].Value = arr[offset + 2 + i];
            }
            _model.MeasProcSettings[num].CalibrCurve.Result.Value = (ushort)arr[offset + 8];
        }

        /// <summary>
        /// Метод распознавания данных еденичного измерния из массива чисел
        /// </summary>
        /// <param name="arr">Массив чисел</param>
        /// <param name="num">Номер изм. процесса</param>
        void RecognizeSingleMeasData(float[] arr, int num)
        {
            for (int i = 26; i < MeasProcSettings.SingleMeasResCount * 5 + 26; i += 5)
            {
                var j = (i - 26) / 5;
                int day = (ushort)arr[i];
                day = day > 0 && day <= 31 ? day : 1;
                int month = (ushort)arr[i + 1];
                month = month > 0 && month <= 12 ? month : 1;
                int year = (ushort)arr[i + 2] + 2000;
                _model.MeasProcSettings[num].SingleMeasResults[j].Date.Value = new DateTime(year, month, day);
                _model.MeasProcSettings[num].SingleMeasResults[j].Weak.Value = arr[i + 3];
                _model.MeasProcSettings[num].SingleMeasResults[j].CounterValue.Value = arr[i + 4];
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
            density.MeasUnit = _model.MeasUnitSettings[(ushort)arr[offset]];
            density.PhysValue.Value = arr[offset + 1];
        }

        /// <summary>
        /// Метод распознавания настроек компенсации
        /// </summary>
        /// <param name="arr">Массив чисел</param>
        /// <param name="compensation">Класс определяющий данные компенсации</param>
        /// <param name="offset">Смещение данных компенсации в массиве чисел</param>
        void RecognizeTempCompensation(float[] arr, MeasProcSettings settings, int offset)
        {
            int size = 4;
            for (int i = 0; i < 3; i++)
            {
                settings.TempCompensations[i].Activity.Value = arr[i * size+offset] > 0;
                for (int j = 0; j < 2; j++)
                {
                    settings.TempCompensations[i].Coeffs[j].Value = arr[offset+ i * size+2+j] ;
                }
            }           

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
        }
        /// <summary>
        /// Метод распознаания настроек быстрого измерения
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="num"></param>
        void RecognizeFastChangeSett(float[] arr, int num)
        {
            _model.MeasProcSettings[num].FastChange.Activity.Value = arr[107] > 0;
            _model.MeasProcSettings[num].FastChange.Threshold.Value = (ushort)arr[108];
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

        void GetVolumeCoeffsFromArr(float[] arr, int offset, int measProcNum)
        {
            for (int i = 0; i < _model.MeasProcSettings[measProcNum].VolumeCoeefs.Count; i++)
            {
                _model.MeasProcSettings[measProcNum].VolumeCoeefs[i].Value = arr[offset + i];
            }
        }

        #region Настройки №1
        void GetSettings1()
        {
            _model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FSR,1#"));
            var list = GetNumber("adc_mode", 1, 1);
            if (list == null) return;
            _model.AdcBoardSettings.AdcMode.Value = (ushort)list[0][0];
            list = GetNumber("adc_sync_mode", 1, 1);
            if (list == null) return;
            _model.AdcBoardSettings.AdcSyncMode.Value = (ushort)list[0][0];
            list = GetNumber("adc_sync_level", 1, 1);
            if (list == null) return;
            _model.AdcBoardSettings.AdcSyncLevel.Value = (ushort)list[0][0];
            list = GetNumber("timer_max", 1, 1);
            if (list == null) return;
            _model.AdcBoardSettings.TimerMax.Value = (ushort)list[0][0];
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
            _model.SettingsReaded = true;
        }
        #endregion

        #region Настройки № 2
        void GetSettings2()
        {
            _model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FSR,2#"));
            var list = GetNumber("adc_proc_cntr", 8, MainModel.CountCounters, str);
            
            if (list == null) return;
            for (int i = 0; i < MainModel.CountCounters; i++)
            {
                _model.CountDiapasones[i].Num.Value = (ushort)list[i][0];
                _model.CountDiapasones[i].CounterMode.Value = (ushort)list[i][1];
                _model.CountDiapasones[i].Start.Value = (ushort)list[i][2];
                _model.CountDiapasones[i].Width.Value = (ushort)list[i][3];
                _model.CountDiapasones[i].CountPeakFind.Value = list[i][4];
                _model.CountDiapasones[i].CountPeakSmooth.Value = list[i][5];
                _model.CountDiapasones[i].CountTopPerc.Value = (ushort)list[i][6];
                _model.CountDiapasones[i].CountBotPerc.Value = (ushort)list[i][7];
            }
            list = GetNumber("adc_proc_mode", 1, 1, str);
            if (list == null) return;
            _model.AdcBoardSettings.AdcProcMode.Value = (ushort)list[0][0];
            list = GetNumber("adc_single_meas_time", 1, 1, str);
            if (list == null) return;
            foreach (var mp in _model.MeasProcSettings)
            {
                mp.SingleMeasTime.Value = (ushort)list[0][0];
            }
            _model.SettingsReaded = true;
        }
        #endregion        

        #region Настройки 4
        void GetSettings4()
        {
            _model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FSR,4#"));
            var indexOfEqual = str.LastIndexOf('=');
            if (indexOfEqual < 1) return;
            var strArr = str.Substring(indexOfEqual + 1).
                Split(new char[] { '#', ',', '=' })
                .ToArray();
            float temp = 0;
            if (strArr.Length <= 126) throw new Exception("Сигнатура ответа на запрос CMND,FSR,4# не соответствует ожидаемому!");
            for (int i = 0; i < 126; i += 6)
            {
                var id = i / 6;
                _model.MeasUnitSettings[id].Id.Value = float.TryParse(strArr[i], out temp) ? (ushort)temp : default;
                _model.MeasUnitSettings[id].MeasUnitClassNum.Value = float.TryParse(strArr[i + 1], out temp) ? (ushort)temp : default;
                _model.MeasUnitSettings[id].Type.Value = float.TryParse(strArr[i + 2], out temp) ? (ushort)temp : default;
                _model.MeasUnitSettings[id].A.Value = float.TryParse(strArr[i + 3].Replace(".", ","), out temp) ? temp : default;
                _model.MeasUnitSettings[id].B.Value = float.TryParse(strArr[i + 4].Replace(".", ","), out temp) ? temp : default;
                _model.MeasUnitSettings[id].MeasUnitName.Value = strArr[i + 5];
            }

            _model.SettingsReaded = true;
        }


        #endregion

        #region Настройки № 7
        void GetSettings7()
        {
            _model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FSR,7#"));
            var list = GetNumber("serial_baudrate", 1, 1, str);
            if (list == null) return;
            _model.PortBaudrate.Value = (uint)list[0][0];
            list = GetNumber("hv_target", 1, 1, str);
            if (list == null) return;
            _model.TelemetryHV.VoltageSV.Value = (ushort)(list[0][0] * 0.05);
            list = GetNumber("serial_select", 1, 1, str);
            if (list == null) return;
            _model.PortSelectMode.Value = (ushort)list[0][0];
            list = GetNumber("am_out_sett", 10, 2, str);
            if (list == null) return;
            for (int i = 0; i < 2; i++)
            {
                _model.AnalogGroups[i].AO.Activity.Value = (ushort)list[i][1];
                _model.AnalogGroups[i].AO.DacType.Value = (ushort)list[i][2];
                _model.AnalogGroups[i].AO.MeasUnit = _model.MeasUnitSettings[(ushort)list[i][3]];
                _model.AnalogGroups[i].AO.AnalogMeasProcNdx.Value = (ushort)list[i][4];
                _model.AnalogGroups[i].AO.VarNdx.Value = (ushort)list[i][5];
                _model.AnalogGroups[i].AO.DacLowLimit.Value = list[i][6];
                _model.AnalogGroups[i].AO.DacHighLimit.Value = list[i][7];
                _model.AnalogGroups[i].AO.DacLowLimitMa.Value = list[i][8];
                _model.AnalogGroups[i].AO.DacHighLimitMa.Value = list[i][9];
            }
            list = GetNumber("am_in_sett", 2, 2, str);
            if (list == null) return;
            _model.AnalogGroups[0].AI.Activity.Value = (ushort)list[0][1];
            _model.AnalogGroups[1].AI.Activity.Value = (ushort)list[1][1];
            list = GetNumber("preamp_gain", 1, 1, str);
            if (list == null) return;
            _model.AdcBoardSettings.PreampGain.Value = (ushort)list[0][0];
            list = GetNumber("half_life", 1, 1, str);
            if (list == null) return;
            _model.HalfLife.Value = list[0][0];
            _model.DeviceName.Value = GetStringById("name", str);
            _model.IsotopName.Value = GetStringById("isotope", str);
            float.TryParse(GetStringById("pipe_diameter", str), out float temp);
            _model.SourceInstallDate.Value = GetDate(GetStringById("src_inst_date", str));
            _model.SourceExpirationDate.Value = GetDate(GetStringById("src_exp_date", str));
            _model.SerialNumber.Value = GetStringById("SN", str);
            _model.OrderNumber.Value = GetStringById("ORDER", str);           
            _model.FwVersion.Value = GetStringById("FW_VER", str);
            _model.FwVersion.Value = GetStringById("FW_VER", str);
            _model.CustNumber.Value = GetStringById("CUSTOMER_NUMBER", str); ;
            _model.SettingsReaded = true;

        }
        #endregion

        string GetStringById(string id, string source)
        {
            var indexfirst = source.IndexOf(id);
            if (indexfirst != -1) return source.Substring(indexfirst + id.Length + 1).
                    Split(new char[] { ',', '#' }).
                    FirstOrDefault();
            return string.Empty;
        }

        DateTime GetDate(string strDate)
        {
            int temp = 0;
            var nums = strDate.Split(":", StringSplitOptions.RemoveEmptyEntries).
                Where(s => int.TryParse(s, out temp)).
                Select(s => temp).
                ToList();
            if (nums.Count != 3) return DateTime.MinValue;
            int day = (ushort)nums[0];
            day = day > 0 && day <= 31 ? day : 1;
            int month = (ushort)nums[1];
            month = month > 0 && month <= 12 ? month : 1;
            int year = (ushort)nums[2] + 2000;
            return new DateTime(year, month, day);
        }

        #region Настройки №8
        void GetSettings8()
        {
            _model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FSR,8#"));
            // Номер порта
            var list = GetNumber("udp_sett", 5, 1, str);
            if (list is null) return;
            _model.UdpAddrString = $"{list[0][0]}.{list[0][1]}.{list[0][2]}.{list[0][3]}";
            _model.PortUdp = (int)list[0][4];
            list = GetNumber("tcp_sett", 12, 1, str);
            if (list == null) return;
            _model.IP = $"{list[0][0]}.{list[0][1]}.{list[0][2]}.{list[0][3]}";
            _model.Mask = $"{list[0][4]}.{list[0][5]}.{list[0][6]}.{list[0][7]}";
            _model.GateWay = $"{list[0][8]}.{list[0][9]}.{list[0][10]}.{list[0][11]}";

            //Настройки фильтра калмана
            list = GetNumber("kalman_sett", 3, 2, str);
            if (list == null) return;
            for (int i = 0; i < 2; i++)
            {
                _model.KalmanSettings[i].Speed.Value = list[i][1];
                _model.KalmanSettings[i].Smooth.Value = list[i][2];
            }

            //Настройки темрературы
            list = GetNumber("am_temp_coeffs", 3, 2, str);
            if (list == null) return;
            for (int i = 0; i < 2; i++)
            {
                _model.GetTemperature.Coeffs[i].A.Value = list[i][1];
                _model.GetTemperature.Coeffs[i].B.Value = list[i][2];
            }

            list = GetNumber("temperature_src", 1, 1, str);
            if (list == null) return;
            _model.GetTemperature.Source = (int)list[0][0];

            // Тип устройства
            list = GetNumber("device_type", 1, 1, str);
            if (list == null) return;
            _model.DeviceType.Value = (ushort)list[0][0];
            //Длина уровнемерра
            list = GetNumber("levelmeter_ln", 1, 1, str);
            if (list == null) return;
            _model.LevelLength.Value = list[0][0];
            _model.SettingsReaded = true;
        }
        #endregion

        #region Настройки №8
        void GetSettings9()
        {
            _model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FSR,9#"));
            _model.SettingsReaded = true;
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

        #region Получить статус логирования
        void GetSdLogStatus()
        {
            _model.SettingsReaded = false;
            var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FMS#"));
            var nums = GetNumericsFromString(str, new char[] { ',', '#' });
            if (nums == null || nums.Length != 1) return;
            _model.IsSdWriting = nums[0] > 0 ? true : false;
            _model.SettingsReaded = true;
        }
        #endregion
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
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); _model.SettingsReaded = false; }, Encoding.ASCII.GetBytes($"*SETT,serial_select={value}#")));
        }
        #endregion

        #region Изменить скорость последовательного порта
        public void ChangeBaudrate(uint value)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); _model.SettingsReaded = false; }, Encoding.ASCII.GetBytes($"*SETT,serial_baudrate={value}#")));
        }
        #endregion

        #region Установить RTC
        public void SetRtc(DateTime dt)
        {
            var str = $"*SETT,rtc_set={dt.Day},{dt.Month},{dt.Year % 100},{dt.Hour},{dt.Minute},{dt.Second}#";
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Уставка HV
        public void SetHv(ushort value)
        {
            var str = $"*SETT,hv_target={value * 20}#";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); _model.SettingsReaded = false; }, Encoding.ASCII.GetBytes(str)));
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
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); _model.SettingsReaded = false; }, Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Отправить настройки аналоговых выходов
        public void SendAnalogOutSwttings(int groupNum, int moduleNum, AnalogOutput value)
        {
            var str = $"*SETT,am_out_sett={groupNum},{value.Activity.Value},{value.DacType.Value},{value.MeasUnit.Id.Value},{value.AnalogMeasProcNdx.Value},{value.VarNdx.Value},{value.DacLowLimit.Value.ToStringPoint()},{value.DacHighLimit.Value.ToStringPoint()},{value.DacLowLimitMa.Value.ToStringPoint()},{value.DacHighLimitMa.Value.ToStringPoint()}#";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetSettings7(); }, Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Отправить настройки аналоговых входов
        public void SendAnalogInSwttings(int groupNum, int moduleNum, AnalogInput value)
        {
            var str = $"*SETT,am_in_sett={groupNum},{value.Activity.Value}#";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); _model.SettingsReaded = false; }, Encoding.ASCII.GetBytes(str)));
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
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str + "#")));
        }
        #endregion

        #region Команда принудиельного запроса набора стандартизации после стандартизации
        public void GetMeasSettingsExternal(int index)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => GetMeasProcessData(index), null));
        }
        #endregion

        #region Команда "Записать настройки счечиков"
        public void WriteCounterSettings(CountDiapasone diapasone)
        {
            var str = $"*SETT,adc_proc_cntr={diapasone.Num.Value},{diapasone.CounterMode.Value},{diapasone.Start.Value}," +
                $"{diapasone.Width.Value},{diapasone.CountPeakFind.Value.ToStringPoint()}," +
                $"{diapasone.CountPeakSmooth.Value.ToStringPoint()}," +
                $"{diapasone.CountTopPerc.Value},{diapasone.CountBotPerc.Value}";
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
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Команда "Перезагрузитьь плату"
        public void RstBoard()
        {
            var str = $"*CMND,RST#";
            commands.Enqueue(new TcpWriteCommand((buf) => SendTlg(buf), Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region КОманда "Установить IP адрес платы"
        public void SetIPAddr(string ip, string mask, string gate)
        {
            var str = $"*SETT,tcp_sett={ip.Replace(".", ",")},{mask.Replace(".", ",")},{gate.Replace(".", ",")}#";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetSettings8(); }, Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Команды записи на Sd карту
        #region Запустить запустить логирование на SD
        public void SwithSdCardLog(int param)
        {
            var str = $"*CMND,FMS,{param}#";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetSdLogStatus(); }, Encoding.ASCII.GetBytes(str)));
        }
        #endregion

        #region Запрос записей результатов измерений
        public void GetSdCardWrites(int start, int finish)
        {
            commands.Enqueue(new TcpWriteCommand((buf) =>
            {
                var str = AskResponse(Encoding.ASCII.GetBytes($"*CMND,FMR,meas_results_05052022_103500.txt,1,2#"));
                Thread.Sleep(1000);
            }, null));
        }
        #endregion

        #region Удаление записей результатов измереия
        public void DelSdCardWrites(int start, int finish)
        {
            commands.Enqueue(new TcpWriteCommand((buf) =>
            {
                var str = AskResponse(Encoding.ASCII.GetBytes($"*CMND,FMD,{start},{finish}#"));
                Thread.Sleep(1000);
            }, null));
        }
        #endregion

        public void GetSdFileNames()
        {
            commands.Enqueue(new TcpWriteCommand((buf) =>
            {
                var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,FML#"));

                Thread.Sleep(1000);

            }, null));
        }
        #endregion

        #region Записать настройки FSRD8
        public void SetFsrd8(string arg)
        {
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetSettings8(); }, Encoding.ASCII.GetBytes(arg)));
        }

        #endregion


        public void WriteCommonSettings(string arg)
        {
            var str = $"*SETT,{arg}#";
            commands.Enqueue(new TcpWriteCommand((buf) => { SendTlg(buf); GetSettings7(); }, Encoding.ASCII.GetBytes(str)));
        }


        #endregion
        /// <summary>
        /// Выполнить запрос и выполнить делегат 
        /// </summary>
        /// <param name="cmnd"></param>
        /// <param name="action"></param>
        public void GetResponce(string cmnd, Action<string> action)
        {
            commands.Enqueue(new TcpWriteCommand((buf) =>
            {
                Client.ReceiveTimeout = 10000;
                var str = AskResponse(Encoding.ASCII.GetBytes(cmnd));
                action?.Invoke(str);
                Client.ReceiveTimeout = 2000;

            }, null));
        }

        public void ListenAndExecute(Action<string> action)
        {
            commands.Enqueue(new TcpWriteCommand((buf) =>
            {
                int num = 0;
                int offset = 0;
                do
                {
                    num = Stream.Read(inBuf, offset, inBuf.Length);
                    Thread.Sleep(_model.TcpConnectData.Pause);
                    offset += num;

                } while (Stream.DataAvailable);
                action?.Invoke(Encoding.ASCII.GetString(inBuf, 0, num));

            }, null));
        }

        #region Очистка буфера
        void StreamClear()
        {
            while (Stream.DataAvailable)
            {
                Stream.Read(inBuf, 0, 1);
            }
        }
        #endregion

    }
}
