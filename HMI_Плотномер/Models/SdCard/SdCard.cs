using IDensity.AddClasses;
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
                  if (IsReading) GetRequest();
              };
        }
        
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


        #region Кол-во записей на карте
        /// <summary>
        /// Кол-во записей на карте
        /// </summary>
        private int _countWrites;
        /// <summary>
        /// Кол-во записей на карте
        /// </summary>
        public int CountWrites
        {
            get => _countWrites;
            set => Set(ref _countWrites, value);
        }
        #endregion       


        #region Стартовый номер записи для удаления
        /// <summary>
        /// Стартовый номер записи для удаления
        /// </summary>
        private int _startDelNum;
        /// <summary>
        /// Стартовый номер записи для удаления
        /// </summary>
        public int StartDelNum
        {
            get => _startDelNum;
            set => Set(ref _startDelNum, value);
        }
        #endregion


        #region Финишный номер для удаления
        /// <summary>
        /// Финишный номер для удаления
        /// </summary>
        private int _finishDelNum;
        /// <summary>
        /// Финишный номер для удаления
        /// </summary>
        public int FinishDelNum
        {
            get => _finishDelNum;
            set => Set(ref _finishDelNum, value);
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
                    GetRequest();
                }
            }
            else IsReading = false;
        }

        void GetRequest()
        {
            _model.Tcp.GetResponce($"*CMND,FMR,{readFile.Name},{readFile.Start},{readFile.Start}#", (str) =>
            {
                if (str == "")
                {
                    if(IsReading)GetRequest();
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
                    GetRequest();
                }

            });
        }
    }
    
}
