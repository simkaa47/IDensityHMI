using IDensity.AddClasses;
using IDensity.Models;
using IDensity.ViewModels;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace IDensity.ViewModels.SdCard
{
    public class SdCardVm : PropertyChangedBase
    {
        public SdCardVm(VM vM)
        {
            VM = vM;
            VM.CommService.Tcp.TcpEvent += (s) =>
              {
                  if (IsReading) GetWritesRequest();
              };            

        }
        /// <summary>
        /// Временная коллекция списка файлов
        /// </summary>
        List<SdFileInfo> fileNames;


        #region Путь к файлу записи
        /// <summary>
        /// Путь к файлу записи
        /// </summary>
        private string _writeToFilePath;
        /// <summary>
        /// Путь к файлу записи
        /// </summary>
        public string WriteToFilePath
        {
            get => _writeToFilePath;
            set => Set(ref _writeToFilePath, value);
        }
        #endregion


        #region Команды
        #region Очистить список файлов
        /// <summary>
        /// Очистить список файлов
        /// </summary>
        RelayCommand _clearSdFileListCommand;
        /// <summary>
        /// Очистить список файлов
        /// </summary>
        public RelayCommand ClearSdFileListCommand => _clearSdFileListCommand ?? (_clearSdFileListCommand = new RelayCommand(execPar => FileNames = new List<SdFileInfo>(), canExecPar => true));
        #endregion

        #region Команда запроса имен файлов на SD карте
        /// <summary>
        /// Команда запроса имен файлов на SD карте
        /// </summary>
        RelayCommand _getFilesSdCommand;
        /// <summary>
        /// Команда запроса имен файлов на SD карте
        /// </summary>
        public RelayCommand GetFilesSdCommand => _getFilesSdCommand ?? (_getFilesSdCommand = new RelayCommand(execPar =>
        {
            VM.CommService.Tcp.GetResponce("*CMND,FML#", (str) =>
            {
                fileNames = new List<SdFileInfo>();
                ParseFileNames(str);
            });
        }, canExecPar => true));
        #endregion
         
        #region Запрос записей на SD карте
        RelayCommand _getSdCardWritesCommand;
        public RelayCommand GetSdCardWritesCommand => _getSdCardWritesCommand ?? (_getSdCardWritesCommand = new RelayCommand(par =>
        {
            GetWrites();
        }, o => true));

        #endregion

        #region Запуск - останов записи на карту
        RelayCommand _switchSdCardLogCommand;
        public RelayCommand SwitchSdCardLogCommand => _switchSdCardLogCommand ?? (_switchSdCardLogCommand = new RelayCommand(par =>
        {
            var parameter = VM.mainModel.IsSdWriting ? 0 : 1;
            VM.CommService.Tcp.SwithSdCardLog((ushort)parameter);

        }, o => VM.mainModel.Connecting.Value));
        #endregion        

        #endregion                    

        #region Коллекция имен файлов
        /// <summary>
        /// Коллекция имен файлов
        /// </summary>
        private List<SdFileInfo> _FileNames;
        /// <summary>
        /// Коллекция имен файлов
        /// </summary>
        public List<SdFileInfo> FileNames
        {
            get => _FileNames;
            set => Set(ref _FileNames, value);
        }
        #endregion

        #region Выбранный файл
        /// <summary>
        /// Выбранный файл
        /// </summary>
        private SdFileInfo _selectedFileInfo;
        /// <summary>
        /// Выбранный файл
        /// </summary>
        public SdFileInfo SelectedFileInfo
        {
            get => _selectedFileInfo;
            set => Set(ref _selectedFileInfo, value);
        }
        #endregion

        SdFileInfo readFile;

        public ObservableCollection<SdCardMeasData> SdCardMeasDatas { get; } = new ObservableCollection<SdCardMeasData>();

        #region Флаг чтения
        /// <summary>
        /// Флаг чтения
        /// </summary>
        private bool _isReading;
        /// <summary>
        /// Флаг чтения
        /// </summary>
        public bool IsReading
        {
            get => _isReading;
            set => Set(ref _isReading, value);
        }
        #endregion        

        #region Флаг записи в файл
        /// <summary>
        /// Флаг записи в файл
        /// </summary>
        private bool _isWritingToFile;
        /// <summary>
        /// Флаг записи в файл
        /// </summary>
        public bool IsWritingToFile
        {
            get => _isWritingToFile;
            set => Set(ref _isWritingToFile, WriteToFilePath != null ? value : false);
        }
        public VM VM { get; }
        #endregion

        public void GetWrites()
        {
            if (SelectedFileInfo != null)
            {
                IsReading = true;
                SdCardMeasDatas.Clear();
                readFile = SelectedFileInfo;
                GetWritesRequest();
            }
            else IsReading = false;
        }

        void GetWritesRequest()
        {
            VM.CommService.Tcp.GetResponce($"*CMND,FMR,{readFile.Name},{readFile.Start},{readFile.Finish}#", (str) =>
            {
                if (str == "")
                {
                    if (IsReading) GetWritesRequest();
                    return;
                }
                Write(str);
                if (readFile.Start >= readFile.Finish)
                {
                    IsReading = false;
                }
                if (IsReading)
                {
                    readFile.Start += readFile.Finish - readFile.Start > 1 ? 2 : 1;
                    GetWritesRequest();
                }

            });

        }
        void GetNextFileListTelegrams()
        {
            VM.CommService.Tcp.ListenAndExecute((str) =>
            {
                if (str.Length > 8)
                {
                    ParseFileNames(str);
                }
            });
        }

        void ParseFileNames(string str)
        {
            var fileInfos = str.Split(new char[] { '#', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (fileInfos.Count % 2 != 0) return;
            for (int i = 2; i < fileInfos.Count; i += 2)
            {
                if (!fileInfos[i].Contains(".txt")) continue;
                fileNames.Add(new SdFileInfo()
                {
                    Id = fileNames.Count + 1,
                    Name = fileInfos[i],
                    WriteNumber = int.TryParse(fileInfos[i + 1], out int temp) ? temp : 0
                });
            }
            if (str.Contains("111111111")) FileNames = fileNames;
            else GetNextFileListTelegrams();
        }

        async void WriteToFile(string str)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(WriteToFilePath, true))
                {
                    await writer.WriteLineAsync(str);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                IsWritingToFile = false;
            }
        }

        void Write(string answer)
        {
            var index = answer.IndexOf("txt,");
            if (index >= 0)
            {
                string clearString = answer.Substring(index + 4);
                var strings = clearString.Split(new char[] { ',', '#' }, StringSplitOptions.RemoveEmptyEntries);
                if (strings.Length % 26 == 0)
                {
                    for (int i = 0; i < strings.Length / 26; i++)
                    {
                        StringBuilder builder = new StringBuilder(strings[i * 26]);
                        for (int j = i * 26 + 1; j < i * 26 + 26; j++)
                        {
                            builder.Append("," + strings[j]);
                        }
                        var result = builder.ToString();
                        if (IsWritingToFile) WriteToFile(result);
                        Application.Current.Dispatcher.Invoke(() => SdCardMeasDatas.Add(ParseResults(strings, i * 26, i * 26 + 25)));
                    }
                }
            }
            else
            {
                var jshdkj = "";
            }
        }

        SdCardMeasData ParseResults(string[] strs, int start, int finish)
        {
            if (strs.Length <= finish) return null;
            if (finish - start != 25) return null;
            SdCardMeasData result = new SdCardMeasData();
            ushort tempUshort = 0;
            DateTime tempDateTime = new DateTime();
            result.Time = DateTime.TryParse($"{strs[start]} {strs[start + 1]}", out tempDateTime) ? tempDateTime : default;
            for (int i = 0; i < 2; i++)
            {
                result.MeasResults[i].ProcNum = (int)strs[start + i * 5 + 2].StringToFloat();
                result.MeasResults[i].CounterValue = (int)strs[start + i * 5 + 3].StringToFloat();
                result.MeasResults[i].PhysValueCur = strs[start + i * 5 + 4].StringToFloat();
                result.MeasResults[i].PhusValueAvg = strs[start + i * 5 + 5].StringToFloat();
                result.MeasResults[i].IsActive = strs[start + i * 5 + 6].StringToFloat() != 0;
                result.AnalogData[i].Dac = (int)strs[start + i * 4 + 16].StringToFloat();
                result.AnalogData[i].Rx = (int)strs[start + i * 4 + 17].StringToFloat();
                result.AnalogData[i].test = (int)strs[start + i * 4 + 18].StringToFloat();
                result.AnalogData[i].Adc = (int)strs[start + i * 4 + 19].StringToFloat();
            }
            result.TempInt = strs[start + 12].StringToFloat();
            result.HvInput = strs[start + 13].StringToFloat();
            result.HvOutU = strs[start + 14].StringToFloat();
            result.HvCurrent = strs[start + 15].StringToFloat();
            result.CommState = (ushort)(ushort.TryParse(strs[start + 24], NumberStyles.HexNumber, null, out tempUshort) ? tempUshort : 0);
            result.PhysParamState = (ushort)(ushort.TryParse(strs[start + 25], NumberStyles.HexNumber, null, out tempUshort) ? tempUshort : 0);
            return result;

        }
    }

}
