using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    /// <summary>
    /// Определяет настройку диапазона счетчиков
    /// </summary>
    public class CountDiapasone:ICloneable
    {
        public CountDiapasone()
        {
            Start.CommandEcecutedEvent += (o) =>
              {
                  var diap = Clone() as CountDiapasone;
                  diap.Start.Value = Start.WriteValue;
                  NeedWriteEvent?.Invoke(diap);
              };
            Finish.CommandEcecutedEvent += (o) =>
            {
                var diap = Clone() as CountDiapasone;
                diap.Finish.Value = Finish.WriteValue;
                NeedWriteEvent?.Invoke(diap);
            };
        }
        #region Номер набора
        public Parameter<ushort> Num { get; set; } = new Parameter<ushort>("CountSelectionNum", "Номер набора", 0, 4095, 91, "hold");
        #endregion
        #region Начальный номер счетчика
        public Parameter<ushort> Start { get; set; } = new Parameter<ushort>("CountDiapasoneStart", "Начальный номер счетчика", 0, 4095, 92, "hold");
        #endregion

        #region Конечный номер счетчика
        public Parameter<ushort> Finish { get; set; } = new Parameter<ushort>("CountDiapasoneFinish", "Конечный номер счетчика", 0, 4095, 93, "hold");

        #endregion

        
        public object Clone()
        {
            return new CountDiapasone()
            {
                Num = this.Num.Clone() as Parameter<ushort>,
                Start = this.Start.Clone() as Parameter<ushort>,
                Finish = this.Finish.Clone() as Parameter<ushort>
            };
        }
        /// <summary>
        /// Необходимо записать настройки измерительных процессов
        /// </summary>
        public event Action<CountDiapasone> NeedWriteEvent;
    }
}
