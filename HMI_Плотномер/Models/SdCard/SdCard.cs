using IDensity.AddClasses;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace IDensity.Models
{
    class SdCard:PropertyChangedBase
    {
        MainModel _model;
        public SdCard(MainModel model)
        {
            _model = model;
            model.Tcp.TcpEvent += (s) =>
              {
                  if (IsReading) GetWritesRequest();
              };
        }
        /// <summary>
        /// Временная коллекция списка файлов
        /// </summary>
        List<SdFileInfo> fileNames;

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
            _model.Tcp.GetResponce("*CMND,FML#", (str) =>
            {
                var fileInfos = str.Split(new char[] { '#', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (fileInfos.Count % 2 != 0) return;
                fileNames = new List<SdFileInfo>();
                for (int i = 2; i < fileInfos.Count; i += 2)
                {
                    fileNames.Add(new SdFileInfo()
                    {
                        Name = fileInfos[i],
                        WriteNumber = (int.TryParse(fileInfos[i + 1], out int temp)) ? temp : 0
                    });
                }
                if(str.Contains("111111111"))_model.SdCard.FileNames = fileNames;
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
            var parameter = IsWriting ? 0 : 1;
            if (_model.CommMode.EthEnable)
            {
                _model.Tcp.SwithSdCardLog((ushort)parameter);
            }

        }, o => _model.Connecting.Value));
        #endregion        

        #endregion

        #region Флаг активности записи на карту
        /// <summary>
        /// Флаг активности записи на карту
        /// </summary>
        private bool _isWriting;
        /// <summary>
        /// Флаг активности записи на карту
        /// </summary>
        public bool IsWriting
        {
            get => _isWriting;
            set => Set(ref _isWriting, value);
        }
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


        public void GetWrites()
        {
            if (_model.CommMode.EthEnable && !IsReading)
            {
                if (SelectedFileInfo != null)
                {
                    IsReading = true;
                    SdCardMeasDatas.Clear();
                    readFile = SelectedFileInfo;
                    GetWritesRequest();
                }
            }
            else IsReading = false;
        }

        void GetWritesRequest()
        {
            _model.Tcp.GetResponce($"*CMND,FMR,{readFile.Name},{readFile.Start},{readFile.finish}#", (str) =>
            {
                if (str == "")
                {
                    if(IsReading)GetWritesRequest();
                    return;
                } 
                Application.Current.Dispatcher.Invoke(() => SdCardMeasDatas.Add(new SdCardMeasData() { Temp = str }));
                if (readFile.Start >= readFile.finish)
                {
                    IsReading = false;
                }
                if (IsReading)
                {
                    readFile.Start++;
                    GetWritesRequest();
                }

            });
        }
    }
    
}
