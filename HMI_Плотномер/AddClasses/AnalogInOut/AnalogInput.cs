using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    public class AnalogInput:AnalogData
    {
        public AnalogInput(int groupNum) : base(groupNum) 
        {
            ModulNum = 1;
        }

        #region активность АЦП
        public Parameter<ushort> Activity { get; } = new Parameter<ushort>("AdcActivity", "Активность аналогового входа, вкл./выкл.", 0, 1, 0, "");
        #endregion

        #region Команда "Записать настройки АЦП"
        RelayCommand _setSettings;
        public RelayCommand SetSettingsCommand => _setSettings ?? (_setSettings = new RelayCommand(o => ChangeSettCallEvent?.Invoke(GroupNum, ModulNum, this), o => true));
        #endregion

        #region Событие вызова команды "Изменить настройки данных"
        public event Action<int, int, AnalogInput> ChangeSettCallEvent;
        #endregion
    }
}
