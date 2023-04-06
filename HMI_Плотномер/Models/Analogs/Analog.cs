using IDensity.AddClasses;
using IDensity.Core.Models.Parameters;
using IDensity.DataAccess;
using System.Runtime.Serialization;

namespace IDensity.Core.Models.Analogs
{
    [DataContract]
    public abstract class AnalogData : PropertyChangedBase
    {
        public AnalogData(int groupNum)
        {
            GroupNum = groupNum;
        }
        #region Тип протокола
        public enum Protocols
        {
            None, Hart, Profibus, Fieldbus
        }
        public Parameter<Protocols> Protocol { get; } = new Parameter<Protocols>("ProtocolType", "Тип протокола", Protocols.None, Protocols.Fieldbus, 0, "");
        #endregion
        #region Номер группы
        [DataMember]
        public int GroupNum { get; set; }
        #endregion
        #region Номер модуля
        [DataMember]
        public int ModulNum { get; set; }
        #endregion
        #region Индикаторы
        #region Связь
        public Parameter<bool> CommState { get; } = new Parameter<bool>("CommState", "Состояние связи с аналоговым модулем", false, true, 0, "");
        #endregion
        #region Питание
        public Parameter<bool> PwrState { get; } = new Parameter<bool>("PwrState", "Питание аналогового модуля", false, true, 0, "");
        #endregion

        #endregion
        #region Значение АЦП
        public Parameter<ushort> AdcValue { get; } = new Parameter<ushort>("AdcValue", "Значение АЦП аналогового модуля", 0, 4095, 0, "");
        #endregion

    }
}
