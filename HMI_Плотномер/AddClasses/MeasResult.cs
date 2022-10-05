using IDensity.AddClasses.Settings;
using IDensity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    public class MeasResult:PropertyChangedBase
    {
        public MeasResult(string measMemoryId)
        {
            MeasUnitMemoryId = measMemoryId;
        }
        #region Значение счетчика, импульсов/с
        /// <summary>
        /// Значение счетчика, импульсов/с
        /// </summary>
        public Parameter<float> CounterValue { get; } = new Parameter<float>("MeasResultCounter", "Значение счетчика, импульсов/с.", 0, float.MaxValue, 0, "read");
        #endregion
        #region Мгновенное значение ФВ
        /// <summary>
        /// Мгновенное значение ФВ
        /// </summary>
        public Parameter<float> PhysValueCur { get; } = new Parameter<float>("MeasResultPhysValueCur", "Мгновенное значение.", 0, float.MaxValue, 0, "read");
        #endregion
        #region Усредненное значение ФВ
        /// <summary>
        /// Усредненное значение ФВ
        /// </summary>
        public Parameter<float> PhysValueAvg { get; } = new Parameter<float>("MeasResultPhysValueAvg", "Усредненное значение.", 0, float.MaxValue, 0, "read");
        #endregion

        #region Номер измерительного процесса
        public Parameter<ushort> MeasProcessNum { get; } = new Parameter<ushort>("MeasResultProcNum", "Номер измерительного процесса", 0, MainModel.MeasProcNum, 0, "");
        #endregion

        #region Настройки соответствующего измерительногоь процесса
        private MeasProcSettings _settings;
        /// <summary>
        /// Настройки соответствующего измерительногоь процесса
        /// </summary>
        public MeasProcSettings Settings
        {
            get { return _settings; }
            set { Set(ref _settings,value); }
        }

        #endregion

        #region Активность
        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set { Set(ref _isActive, value); }
        }
        #endregion

        private string _measUnitMemoryId;
        public string MeasUnitMemoryId
        {
            get => _measUnitMemoryId;
            set => Set(ref _measUnitMemoryId, value);
        }


        public void ClearResult()
        {
            PhysValueCur.Value = 0;
            PhysValueAvg.Value = 0;
        }

        
    }
}
