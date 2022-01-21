using IDensity.Models;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDensity.AddClasses
{
    class CalibrData:ICloneable
    {
        #region Номер набора данных калибровки
        /// <summary>
        /// Номер набора данных калибровки
        /// </summary>
        public Parameter<ushort> Num { get; set; } = new Parameter<ushort>("CalibrDataNum", "Номер набора данных калибровки", 0, (ushort)MainModel.CalibCurveNum, 94, "hold");
        #endregion
        #region Номер единицы измерения
        /// <summary>
        /// Номер единицы измерения
        /// </summary>
        public Parameter<ushort> MeasUnitNum { get; set; } = new Parameter<ushort>("CalibrDataMeasUnitNum", "Номер еденицы измерения", 0, ushort.MaxValue, 95, "hold");
        #endregion
        #region КОэффициенты калибровки
        /// <summary>
        /// КОэффициенты калибровки
        /// </summary>
        public List<Parameter<float>> Coeffs { get; set; } = Enumerable.Range(0, 6).Select(i => new Parameter<float>("CalibrCoeff" + i, "Коэффициент калибровочной кривой " + i, float.NegativeInfinity, float.PositiveInfinity, 97 + i * 2, "hold")).ToList();

        #endregion
        #region Команды
        #region Команда "Записать номер еденицы измекрения"
        RelayCommand _setMeasUnitCommand;

        public RelayCommand SetMeasUnitCommand => _setMeasUnitCommand ?? (_setMeasUnitCommand = new RelayCommand(execPar => {
            var calibData = this.Clone() as CalibrData;
            calibData.MeasUnitNum.Value = this.MeasUnitNum.WriteValue;
            SetSettingsCommandEvent?.Invoke(calibData);
        }, 
            CanExecutePar => true));

        #endregion
        #region Команда "Записать коэффициент калибровки"
        RelayCommand _setCoeffCommand;
        public RelayCommand SetCoeffCommand => _setCoeffCommand ?? (_setCoeffCommand = new RelayCommand(execPar => {
            int index = 0;
            if (int.TryParse(execPar.ToString(), out index) && index<MainModel.CountCounters)
            {
                var calibData = this.Clone() as CalibrData;
                calibData.Coeffs[index].Value = this.Coeffs[index].WriteValue;
                SetSettingsCommandEvent?.Invoke(calibData);
            }            
        }, 
            CanExecutePar => true));
        #endregion
        #endregion

        #region Событие "Запись настроек в плату"
        public event Action<CalibrData> SetSettingsCommandEvent;
        #endregion

        public object Clone()
        {
            return new CalibrData()
            {
                Num = this.Num.Clone() as Parameter<ushort>,
                MeasUnitNum = this.MeasUnitNum.Clone() as Parameter<ushort>,
                Coeffs = this.Coeffs.Select(coeff=>coeff.Clone() as Parameter<float>).ToList()
            };
        }


    }
}
