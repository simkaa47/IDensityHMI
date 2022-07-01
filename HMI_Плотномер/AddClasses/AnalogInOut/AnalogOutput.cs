using IDensity.AddClasses;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    public class AnalogOutput : AnalogData
    {
        #region КОнтструтор
        public AnalogOutput(int groupNum) : base(groupNum)
        {
            ModulNum = 0;
            AmTestValue.CommandEcecutedEvent += (par) => 
            {
                AmTestValue.Value = AmTestValue.WriteValue;
                SetTestValueCallEvent?.Invoke(GroupNum, ModulNum, (ushort)(AmTestValue.WriteValue));
            };
        }
        #endregion

        #region Значение тестового напряжения
        public Parameter<ushort> AmTestValue { get; } = new Parameter<ushort>("AmTestValue", "Значение тестового напряжения, mV", 0, 3000, 12, "holds");
        #endregion

        #region Индикация напряжения RX
        public Parameter<ushort> VoltageDac{ get; } = new Parameter<ushort>("VoltageDac", "Напряжение модуля ЦАП, вольт", 0, 4095, 0, "") { OnlyRead = true };
        #endregion

        #region Индикация напряжения test
        public Parameter<ushort> VoltageTest { get; } = new Parameter<ushort>("VoltageTest", "Напряжение TEST модуля ЦАП", 0, 4095, 0, "") { OnlyRead = true };
        #endregion

        #region активность ЦАП
        public Parameter<ushort> Activity { get; } = new Parameter<ushort>("DacActivity", "Активность аналогового выходв, вкл./выкл.", 0, 1, 0, "");
        #endregion

        #region Тип ЦАП
        public Parameter<ushort> DacType { get; } = new Parameter<ushort>("DacType", "Тип источника ЦАП", 0, 1, 0, "");
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
        #endregion

        #region Номер измерительного процесса
        public Parameter<ushort> AnalogMeasProcNdx { get; } = new Parameter<ushort>("AnalogMeasProcNdx", "Номер измерительного процесса", 0, 4, 0, "");
        #endregion

        #region Тип переменной
        public Parameter<ushort> VarNdx { get; } = new Parameter<ushort>("AnalogVarNdx", "Тип переменной", 0, 1, 1, "");
        #endregion

        #region Нижняя граница ЦАП
        public Parameter<float> DacLowLimit { get; } = new Parameter<float>("DacLowLimit", "Нижний предел для ЦАП", float.MinValue, float.MaxValue, 0, "");
        #endregion

        #region Верхняя граница ЦАП
        public Parameter<float> DacHighLimit { get; } = new Parameter<float>("DacHighLimit", "Верхний предел для ЦАП", float.MinValue, float.MaxValue, 0, "");
        #endregion

        #region Нижняя граница ЦАП(mA)
        public Parameter<float> DacLowLimitMa { get; } = new Parameter<float>("DacLowLimitMa", "Нижний предел для ЦАП(mA)", float.MinValue, float.MaxValue, 0, "");
        #endregion

        #region Верхняя граница ЦАП(mA)
        public Parameter<float> DacHighLimitMa { get; } = new Parameter<float>("DacHighLimitMa", "Верхний предел для ЦАП(mA)", float.MinValue, float.MaxValue, 0, "");
        #endregion

        #region Команды      

        #region Команда "Записать настройки ЦАП"
        RelayCommand _setSettings;
        public RelayCommand SetSettingsCommand => _setSettings ?? (_setSettings = new RelayCommand(o => ChangeSettCallEvent?.Invoke(GroupNum, 0, Clone()), o => true));
        #endregion
        #endregion

        AnalogOutput Clone()
        {
            var output = new AnalogOutput(GroupNum);
            output.Activity.Value = this.Activity.ValidationOk ? this.Activity.WriteValue : this.Activity.Value;
            output.DacType.Value = this.DacType.ValidationOk ? this.DacType.WriteValue : this.DacType.Value;
            output.MeasUnit = this.MeasUnit;
            output.AnalogMeasProcNdx.Value = this.AnalogMeasProcNdx.ValidationOk ? this.AnalogMeasProcNdx.WriteValue : this.AnalogMeasProcNdx.Value;
            output.VarNdx.Value = this.VarNdx.ValidationOk ? this.VarNdx.WriteValue : this.VarNdx.Value;
            output.DacLowLimit.Value = this.DacLowLimit.ValidationOk ? this.DacLowLimit.WriteValue : this.DacLowLimit.Value;
            output.DacHighLimit.Value = this.DacHighLimit.ValidationOk ? this.DacHighLimit.WriteValue : this.DacHighLimit.Value;
            output.DacLowLimitMa.Value = this.DacLowLimitMa.ValidationOk ? this.DacLowLimitMa.WriteValue : this.DacLowLimitMa.Value;
            output.DacHighLimitMa.Value = this.DacHighLimitMa.ValidationOk ? this.DacHighLimitMa.WriteValue : this.DacHighLimitMa.Value;
            return output;
        }

        #region Событие вызова команды установки тестового напряжения
        public event Action<int, int, ushort> SetTestValueCallEvent;
        #endregion

        #region Событие вызова команды "Изменить настройки данных"
        public event Action<int, int, AnalogOutput> ChangeSettCallEvent;
        #endregion

    }
}
