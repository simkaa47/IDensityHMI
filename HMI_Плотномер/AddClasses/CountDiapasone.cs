using System;
using System.Runtime.Serialization;

namespace IDensity.AddClasses
{
    /// <summary>
    /// Определяет настройку диапазона счетчиков
    /// </summary>
    [DataContract]
    public class CountDiapasone : PropertyChangedBase, ICloneable
    {
        public CountDiapasone()
        {
            Start.PropertyChanged += (o, e) => Finish = (ushort)(Start.Value + Width.Value);
            Width.PropertyChanged += (o, e) => Finish = (ushort)(Start.Value + Width.Value);
        }
        #region Номер набора
        [DataMember]
        public Parameter<ushort> Num { get; set; } = new Parameter<ushort>("CountSelectionNum", "Номер набора", 0, 4095, 91, "hold");
        #endregion

        #region Режим раоты счетчиков
        [DataMember]
        public Parameter<ushort> CounterMode { get; set; } = new Parameter<ushort>("CounterMode", "Режим работы счетчиков", 0, 1, 0, "");
        #endregion

        #region Начальный номер счетчика
        [DataMember]
        public Parameter<ushort> Start { get; set; } = new Parameter<ushort>("CountDiapasoneStart", "Начальный номер счетчика", 0, 4095, 92, "hold");
        #endregion

        #region КОнечный номер счетчика
        private ushort _finish;
        [DataMember]
        public ushort Finish
        {
            get => _finish;
            set => Set(ref _finish, value);

        }
        #endregion

        #region Ширина счетчика
        [DataMember]
        public Parameter<ushort> Width { get; set; } = new Parameter<ushort>("CountDiapasoneWidth", "Ширина счетчика", 0, 4095, 0, "hold");

        #endregion

        #region К-т эксп сглаживания по спектру
        [DataMember]
        public Parameter<float> CountPeakFind { get; set; } = new Parameter<float>("CountPeakFind", "К-т сглаживания по спектру", 0, float.MaxValue, 0, "");
        #endregion

        #region К-т эксп сглаживания по времени
        [DataMember]
        public Parameter<float> CountPeakSmooth { get; set; } = new Parameter<float>("CountPeakSmooth", "К-т сглаживания по времени", 0, float.MaxValue, 0, "");
        #endregion

        #region Отклонение вниз
        [DataMember]
        public Parameter<ushort> CountBotPerc { get; set; } = new Parameter<ushort>("CountBotPerc", "% отклонение вниз", 0, 100, 0, "");
        #endregion

        #region Отклонение вниз
        [DataMember]
        public Parameter<ushort> CountTopPerc { get; set; } = new Parameter<ushort>("CountTopPerc", "% отклонение вверх", 0, 100, 0, "");
        #endregion
        public object Clone()
        {
            return new CountDiapasone()
            {
                Num = this.Num.Clone() as Parameter<ushort>,
                Start = this.Start.Clone() as Parameter<ushort>,
                Width = this.Width.Clone() as Parameter<ushort>,
                CounterMode = this.CounterMode.Clone() as Parameter<ushort>,
                CountPeakFind = this.CountPeakFind.Clone() as Parameter<float>,
                CountPeakSmooth = this.CountPeakSmooth.Clone() as Parameter<float>,
                CountBotPerc = this.CountBotPerc.Clone() as Parameter<ushort>,
                CountTopPerc = this.CountTopPerc.Clone() as Parameter<ushort>
            };
        }

    }
}
