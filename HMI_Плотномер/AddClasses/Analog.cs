using IDensity.AddClasses;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    class AnalogData
    {
        public AnalogData(int groupNum)
        {
            this.GroupNum = groupNum;
        }
        #region Тип протокола
        public enum Protocols
        {
            None, Hart, Profibus, Fieldbus
        }
        public Parameter<Protocols> Protocol { get; } = new Parameter<Protocols>("ProtocolType", "Тип протокола", Protocols.None, Protocols.Fieldbus, 0, "");
        #endregion
        #region Номер группы
        public int GroupNum { get; set; }
        #endregion
        #region Номер модуля
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
        #region Команды
        #region Команда подать питание
        RelayCommand _switchPwrAmCommand;
        public RelayCommand SwitchPwrAmCommand => _switchPwrAmCommand ?? (_switchPwrAmCommand = new RelayCommand(o => SwitchPwrEvent?.Invoke(GroupNum, ModulNum, !PwrState.Value), o => true));
        #endregion
        #endregion
        #region Событие вкл-выкл модуля
        /// <summary>
        /// Событие вкл-выкл модуля
        /// </summary>
        public event Action<int, int, bool> SwitchPwrEvent; 
        #endregion
    }
}
