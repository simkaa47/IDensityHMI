using IDensity.Models;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDensity.AddClasses
{
    class CalibrData:PropertyChangedBase
    {
        public CalibrData()
        {
            Type.CommandEcecutedEvent += OnCommandWriteExecuted;            
            foreach (var coeff in Coeffs)
            {
                coeff.CommandEcecutedEvent += OnCommandWriteExecuted;
            }
        }
        void OnCommandWriteExecuted(object par)
        {
            var arg = $"calib_curve={Type.WriteValue},{MeasUnit.Id.Value}";
            foreach (var coeff in Coeffs)
            {
                arg += $",{coeff.WriteValue.ToStringPoint()}";
            }
            NeedWriteEvent?.Invoke(arg);
        }

        
        #region Тип калибровки
        /// <summary>
        /// Тип калибровки
        /// </summary>
        public Parameter<ushort> Type { get; } = new Parameter<ushort>("CalibrType", "Тип калибровки", 0, 10, 0, "hold");
        #endregion

        #region ЕИ
        MeasUnitSettings _measUnit = new MeasUnitSettings();
        /// <summary>
        /// ЕИ
        /// </summary>
        public MeasUnitSettings MeasUnit
        {
            get => _measUnit;
            set => Set(ref _measUnit, value);
        }
        private RelayCommand _outMeasNumWriteCommand;

        public RelayCommand OutMeasNumWriteCommand => _outMeasNumWriteCommand ?? (_outMeasNumWriteCommand = new RelayCommand(par => OnCommandWriteExecuted(null), o => true));
        #endregion
        #region КОэффициенты калибровки
        /// <summary>
        /// КОэффициенты калибровки
        /// </summary>
        public List<Parameter<float>> Coeffs { get; } = Enumerable.Range(0, 6).Select(i => new Parameter<float>("CalibrCoeff" + i, "Коэффициент калибровочной кривой " + i, float.NegativeInfinity, float.PositiveInfinity, 97 + i * 2, "hold")).ToList();

        #endregion
        /// <summary>
        /// Необходимо записать настройки стандартизаций
        /// </summary>
        public event Action<string> NeedWriteEvent;
    }
}
