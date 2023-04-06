using IDensity.Core.Models.Parameters;
using System.Runtime.Serialization;

namespace IDensity.AddClasses.AdcBoardSettings
{
    [DataContract]
    public class AdcBoardSettings
    {
        #region Режим работы АЦП
        [DataMember]
        public Parameter<ushort> AdcMode { get; set; } = new Parameter<ushort>("AdcMode", "Режим работы АЦП", 0, 1, 111, "hold");
        #endregion

        #region Режим синхронизации
        [DataMember]
        public Parameter<ushort> AdcSyncMode { get; set; } = new Parameter<ushort>("AdcSyncMode", "Режим синхронизации", 0, 1, 112, "hold");
        #endregion

        #region Уровень синхронизации
        [DataMember]
        public Parameter<ushort> AdcSyncLevel { get; set; } = new Parameter<ushort>("AdcSyncLevel", "Уровень синхронизации", 0, 65535, 113, "hold");
        #endregion

        #region Режим обработки при регистрировании максимальных амплитуд
        [DataMember]
        public Parameter<ushort> AdcProcMode { get; set; } = new Parameter<ushort>("AdcProcMode", "Режим обработки при регистрировании максимальных амплитуд", 0, 2, 114, "hold");
        #endregion

        #region Таймер выдачи данных, мс
        [DataMember]
        public Parameter<ushort> TimerMax { get; set; } = new Parameter<ushort>("AdcTimerMax", "Таймер выдачи данных, мс", 0, 65535, 115, "hold");
        #endregion

        #region Коэффициент платы предусиления
        [DataMember]
        public Parameter<ushort> PreampGain { get; set; } = new Parameter<ushort>("PreampGain", "Коэффицент предусиления", 0, 255, 116, "hold");
        #endregion

        #region Связь с АЦП
        public Parameter<bool> CommState { get; } = new Parameter<bool>("AdcCommState", "Состояние связи с платой АЦП", true, false, 0, "")
        {
            OnlyRead = true
        };
        #endregion
    }
}
