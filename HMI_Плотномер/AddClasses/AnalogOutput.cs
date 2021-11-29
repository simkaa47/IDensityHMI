using HMI_Плотномер.AddClasses;
using HMI_Плотномер.ViewModels.Commands;
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
        public Parameter<ushort> AmTestValue = new Parameter<ushort>("AmTestValue", "Значение тестового напряжения, mV", 0, 3000, 0, "");
        #endregion

        #region Источник данных для ЦАП
        public Parameter<ushort> Source = new Parameter<ushort>("DacDataSource", "Источник данных для ЦАП", 0, 2, 0, "");
        #endregion

        #region Команда отправить тестовый сигнал
        RelayCommand _setTestSignalCommand;
        public RelayCommand SetTestSignalCommand => _setTestSignalCommand ?? (_setTestSignalCommand = new RelayCommand(o => SetTestValueCallEvent?.Invoke(GroupNum, ModulNum, AmTestValue.Value), o => true));
        #endregion

        #region Команда "Изменить источник данных для ЦАП
        RelayCommand _changeDacSourceCommand;
        public RelayCommand ChangeDacSourceCommand => _changeDacSourceCommand ?? (_changeDacSourceCommand = new RelayCommand(o =>ChangeSourceCallEvent(GroupNum, ModulNum, Source.Value), o => true));
        #endregion

        #region Событие вызова команды установки тестового напряжения
        public event Action<int, int, ushort> SetTestValueCallEvent;
        #endregion

        #region Событие вызова команды "Изменить источник данных"
        public event Action<int, int, ushort> ChangeSourceCallEvent;
        #endregion

    }
}
