using IDensity.Core.Models.Parameters;
using System.Runtime.Serialization;

namespace IDensity.Core.Models.Analogs
{
    [DataContract]
    public class AnalogOutput : AnalogData
    {
        #region КОнтструтор
        public AnalogOutput(int groupNum) : base(groupNum)
        {
            ModulNum = 0;
            MeasUnitMemoryId = $"StandMeasMemory{groupNum}";
        }
        #endregion

        #region Значение тестового напряжения
        [DataMember]
        public Parameter<ushort> AmTestValue { get; set; } = new Parameter<ushort>("AmTestValue", "Значение тестового напряжения, mV", 0, 3000, 12, "holds");
        #endregion

        #region Индикация напряжения RX
        public Parameter<ushort> VoltageDac { get; } = new Parameter<ushort>("VoltageDac", "Напряжение модуля ЦАП, вольт", 0, 4095, 0, "") { OnlyRead = true };
        #endregion

        #region Индикация напряжения test
        public Parameter<ushort> VoltageTest { get; } = new Parameter<ushort>("VoltageTest", "Напряжение TEST модуля ЦАП", 0, 4095, 0, "") { OnlyRead = true };
        #endregion

        #region активность ЦАП
        [DataMember]
        public Parameter<ushort> Activity { get; set; } = new Parameter<ushort>("DacActivity", "Активность аналогового выходв, вкл./выкл.", 0, 1, 0, "");
        #endregion

        #region Тип ЦАП
        [DataMember]
        public Parameter<ushort> DacType { get; set; } = new Parameter<ushort>("DacType", "Тип источника ЦАП", 0, 1, 0, "");
        #endregion        

        #region Номер измерительного процесса
        [DataMember]
        public Parameter<ushort> AnalogMeasProcNdx { get; set; } = new Parameter<ushort>("AnalogMeasProcNdx", "Номер измерительного процесса", 0, 4, 0, "");
        #endregion

        #region Тип переменной
        [DataMember]
        public Parameter<ushort> VarNdx { get; set; } = new Parameter<ushort>("AnalogVarNdx", "Тип переменной", 0, 1, 1, "");
        #endregion

        #region Нижняя граница ЦАП
        [DataMember]
        public Parameter<float> DacLowLimit { get; set; } = new Parameter<float>("DacLowLimit", "Нижний предел для ЦАП", float.MinValue, float.MaxValue, 0, "");
        #endregion

        #region Верхняя граница ЦАП
        [DataMember]
        public Parameter<float> DacHighLimit { get; set; } = new Parameter<float>("DacHighLimit", "Верхний предел для ЦАП", float.MinValue, float.MaxValue, 0, "");
        #endregion

        #region Нижняя граница ЦАП(mA)
        [DataMember]
        public Parameter<float> DacLowLimitMa { get; set; } = new Parameter<float>("DacLowLimitMa", "Нижний предел для ЦАП(mA)", float.MinValue, float.MaxValue, 0, "");
        #endregion

        #region Верхняя граница ЦАП(mA)
        [DataMember]
        public Parameter<float> DacHighLimitMa { get; set; } = new Parameter<float>("DacHighLimitMa", "Верхний предел для ЦАП(mA)", float.MinValue, float.MaxValue, 0, "");
        #endregion



        public AnalogOutput Clone()
        {
            var output = new AnalogOutput(GroupNum);
            output.Activity.Value = Activity.ValidationOk ? Activity.WriteValue : Activity.Value;
            output.DacType.Value = DacType.ValidationOk ? DacType.WriteValue : DacType.Value;
            output.AnalogMeasProcNdx.Value = AnalogMeasProcNdx.ValidationOk ? AnalogMeasProcNdx.WriteValue : AnalogMeasProcNdx.Value;
            output.VarNdx.Value = VarNdx.ValidationOk ? VarNdx.WriteValue : VarNdx.Value;
            output.DacLowLimit.Value = DacLowLimit.ValidationOk ? DacLowLimit.WriteValue : DacLowLimit.Value;
            output.DacHighLimit.Value = DacHighLimit.ValidationOk ? DacHighLimit.WriteValue : DacHighLimit.Value;
            output.DacLowLimitMa.Value = DacLowLimitMa.ValidationOk ? DacLowLimitMa.WriteValue : DacLowLimitMa.Value;
            output.DacHighLimitMa.Value = DacHighLimitMa.ValidationOk ? DacHighLimitMa.WriteValue : DacHighLimitMa.Value;
            return output;
        }

        #region ID ЕИ
        private string _measUnitMemoryId;
        public string MeasUnitMemoryId
        {
            get => _measUnitMemoryId;
            set => Set(ref _measUnitMemoryId, value);
        }
        #endregion




    }
}
