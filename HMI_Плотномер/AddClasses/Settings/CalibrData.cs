using IDensity.Models;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace IDensity.AddClasses
{
    [DataContract]
    public class CalibrData:PropertyChangedBase
    {
        public CalibrData()
        {
            Type.CommandEcecutedEvent += OnCommandWriteExecuted;            
            foreach (var coeff in Coeffs)
            {
                coeff.CommandEcecutedEvent += OnCommandWriteExecuted;
            }
            Result.CommandEcecutedEvent += OnCommandWriteExecuted;
        }
        void OnCommandWriteExecuted(object par)
        {
            var arg = $"calib_curve={Type.WriteValue},0";
            foreach (var coeff in Coeffs)
            {
                arg += $",{coeff.WriteValue.ToStringPoint()}";
            }
            arg += $",{Result.WriteValue}";
            NeedWriteEvent?.Invoke(arg);
        }


        #region Тип калибровки
        /// <summary>
        /// Тип калибровки
        /// </summary>
        [DataMember]
        public Parameter<ushort> Type { get; set; } = new Parameter<ushort>("CalibrType", "Тип калибровки", 0, 10, 0, "hold");
        #endregion

        #region Команда записи
        
        private RelayCommand _outMeasNumWriteCommand;

        public RelayCommand OutMeasNumWriteCommand => _outMeasNumWriteCommand ?? (_outMeasNumWriteCommand = new RelayCommand(par => OnCommandWriteExecuted(null), o => true));
        #endregion
        #region КОэффициенты калибровки
        /// <summary>
        /// КОэффициенты калибровки
        /// </summary>
        [DataMember]
        public List<Parameter<float>> Coeffs { get; set; } = Enumerable.Range(0, 6).Select(i => new Parameter<float>("CalibrCoeff" + i, "Коэффициент калибровочной кривой " + i, float.NegativeInfinity, float.PositiveInfinity, 97 + i * 2, "hold")).ToList();

        #endregion
        #region Резульат калибровки
        [DataMember]
        public Parameter<ushort> Result { get; set; } = new Parameter<ushort>("CalibrResult", "Результат калибровки", 0, 2, 0, ""); 
        #endregion

        /// <summary>
        /// Необходимо записать настройки стандартизаций
        /// </summary>
        public event Action<string> NeedWriteEvent;        
    }
}
