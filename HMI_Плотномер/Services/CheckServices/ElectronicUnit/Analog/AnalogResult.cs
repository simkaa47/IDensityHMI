using IDensity.AddClasses;
using IDensity.DataAccess;
using System.Collections.Generic;

namespace IDensity.Core.Services.CheckServices.ElectronicUnit.Analog
{
    
    public class AnalogResult:PropertyChangedBase
    {
        public AnalogResult(int num)
        {
            Num = num;
        }

        #region Номер выхода
        /// <summary>
        /// Номер выхода
        /// </summary>
        private int _num;
        /// <summary>
        /// Номер выхода
        /// </summary>
        public int Num
        {
            get => _num;
            set => Set(ref _num, value);
        }
        #endregion

        #region Ошибка
        /// <summary>
        /// Ошибка
        /// </summary>
        private bool _isError;
        /// <summary>
        /// Ошибка
        /// </summary>
        public bool IsError
        {
            get => _isError;
            set => Set(ref _isError, value);
        }
        #endregion

        #region Замеры
        public List<AnalogMeasPoint> MeasPoints { get; } = new List<AnalogMeasPoint>(); 
        #endregion

    }
}
