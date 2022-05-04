using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Models.SdCard
{
    class SdCard:PropertyChangedBase
    {
        public SdCard(MainModel _model)
        {
            model = _model;
        }
        MainModel model;
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


        #region Стартовый номер записи для чтения
        /// <summary>
        /// Стартовый номер записи для чтения
        /// </summary>
        private int _startReadNum;
        /// <summary>
        /// Стартовый номер записи для чтения
        /// </summary>
        public int StartReadNum
        {
            get => _startReadNum;
            set => Set(ref _startReadNum, value);
        }
        #endregion


        #region Финишный номер записи для чтения
        /// <summary>
        /// Финишный номер записи для чтения
        /// </summary>
        private int _finishReadNum;
        /// <summary>
        /// Финишный номер записи для чтения
        /// </summary>
        public int FinishReadNum
        {
            get => _finishReadNum;
            set => Set(ref _finishReadNum, value);
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


       





    }
    
}
