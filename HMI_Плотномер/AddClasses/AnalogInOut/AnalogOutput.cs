using IDensity.AddClasses;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    class AnalogOutput : AnalogData
    {
        #region КОнтструтор
        public AnalogOutput(int groupNum) : base(groupNum)
        {
            ModulNum = 0;
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

        #region Номер еденицы измерения
        public Parameter<ushort> DacEiNdx { get; } = new Parameter<ushort>("DacEiNdx", "Номер еденицы измерения", 0, 7, 0, "");
        #endregion

        #region Номер переменной для выдачи
        public Parameter<ushort> DacVarNdx { get; } = new Parameter<ushort>("DacVarNdx", "Номер переменной для выдачи", 0, 4, 0, "");
        #endregion

        #region Нижняя граница ЦАП
        public Parameter<float> DacLowLimit { get; } = new Parameter<float>("DacLowLimit", "Нижний предел для ЦАП", float.MinValue, float.MaxValue, 0, "");
        #endregion

        #region Верхняя граница ЦАП
        public Parameter<float> DacHighLimit { get; } = new Parameter<float>("DacHighLimit", "Верхний предел для ЦАП", float.MinValue, float.MaxValue, 0, "");
        #endregion


        #region Команды
        #region Команда отправить тестовый сигнал
        RelayCommand _setTestSignalCommand;
        public RelayCommand SetTestSignalCommand => _setTestSignalCommand ?? (_setTestSignalCommand = new RelayCommand(o => {
            AmTestValue.Value = AmTestValue.WriteValue;
            SetTestValueCallEvent?.Invoke(GroupNum, ModulNum, (ushort)(AmTestValue.WriteValue*10)); 
        } , o => AmTestValue.ValidationOk));
        #endregion

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
            output.DacEiNdx.Value = this.DacEiNdx.ValidationOk ? this.DacEiNdx.WriteValue : this.DacEiNdx.Value;
            output.DacVarNdx.Value = this.DacVarNdx.ValidationOk ? this.DacVarNdx.WriteValue : this.DacVarNdx.Value;
            output.DacLowLimit.Value = this.DacLowLimit.ValidationOk ? this.DacLowLimit.WriteValue : this.DacLowLimit.Value;
            output.DacHighLimit.Value = this.DacHighLimit.ValidationOk ? this.DacHighLimit.WriteValue : this.DacHighLimit.Value;
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
