using IDensity.AddClasses;
using IDensity.Models.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Models
{
    class Calibration
    {
        #region Коллукция измерений
        public DataBaseCollection<SingleMeasCell> SingleMeasCells { get; } = new DataBaseCollection<SingleMeasCell>("SingleMeasures", null);
        #endregion

        #region Метод расчета калибровочной кривой
        public void GetCoeffs()
        {
            if (SingleMeasCells.Data.Count < 2)
            {
                CalculateExecEvent?.Invoke("Количество данных в таблице меньше 2!");
            }
        }
        #endregion
        #region Калибровочные коэффициенты
        public List<float> Coeffs { get; } = new List<float>();
        #endregion

        #region Событие расчета коэффициентов
        public event Action<string> CalculateExecEvent; 
        #endregion
    }
}
