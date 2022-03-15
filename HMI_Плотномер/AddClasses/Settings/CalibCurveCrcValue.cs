using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses.Settings
{
    /// <summary>
    /// Данные, из которых получена калибровочная кривая
    /// </summary>
    class CalibCurveCrcValue:PropertyChangedBase
    {
        const string TcpArg = "date,weak,value";
        public CalibCurveCrcValue(int id)
        {
            this.Id = id;
            Date.CommandEcecutedEvent += o => CallWriteEvent("date", Date.WriteValue.ToString("dd:MM:yy"));
            Weak.CommandEcecutedEvent += o => CallWriteEvent("weak", Weak.WriteValue);
            CounterValue.CommandEcecutedEvent += o => CallWriteEvent("value", CounterValue.WriteValue);
        }
        #region Id
        private int _id;

        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        } 
        #endregion
        #region Дата 
        /// <summary>
        /// Дата измерения
        /// </summary>
        public Parameter<DateTime> Date { get; } = new Parameter<DateTime>("CalibCurveCrcDate", "Дата измерения", DateTime.MinValue, DateTime.MaxValue, 0, "");
        #endregion
        #region Ослабление
        /// <summary>
        /// Ослабление
        /// </summary>
        public Parameter<float> Weak { get; } = new Parameter<float>("CalibCurveCrcWeak", "Ослабление", float.MinValue, float.MaxValue, 0, "");
        #endregion
        #region Значение счетчика
        /// <summary>
        /// Значение счетчика
        /// </summary>
        public Parameter<float> CounterValue { get; } = new Parameter<float>("CalibCurveCrcValue", "Значение счетчика", 0, float.MaxValue, 0, "");
        #endregion

        void CallWriteEvent<T>(string parName, T value)
        {
            var arg = TcpArg.Replace(parName, value.ToString().Replace(",", "."));
            var parameters = arg.Split(new char[] { ',', '=' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var par in parameters)
            {
                switch (par)
                {
                    case "date":
                        arg = arg.Replace(par, Date.Value.ToString("dd:MM:yy"));
                        break;
                    case "weak":
                        arg = arg.Replace(par, Weak.Value.ToString().Replace(",", "."));
                        break;
                    case "value":
                        arg = arg.Replace(par, CounterValue.Value.ToString().Replace(",", "."));
                        break;
                    default:
                        break;
                }
            }
            NeedWriteEvent?.Invoke(arg, Id);

        }

        /// <summary>
        /// Необходимо записать данные еденичных измерений
        /// </summary>
        public event Action<string, int> NeedWriteEvent;
    }
}
