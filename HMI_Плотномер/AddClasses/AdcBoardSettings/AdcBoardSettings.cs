using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses.AdcBoardSettings
{
    class AdcBoardSettings: ICloneable
    {
        #region Режим работы АЦП
        public Parameter<ushort> AdcMode { get; set; } = new Parameter<ushort>("AdcMode", "Режим работы АЦП", 0, 1, 111, "hold");
        #endregion

        #region Режим синхронизации
        public Parameter<ushort> AdcSyncMode { get; set; } = new Parameter<ushort>("AdcSyncMode", "Режим синхронизации", 0, 1, 112, "hold");
        #endregion

        #region Уровень синхронизации
        public Parameter<ushort> AdcSyncLevel { get; set; } = new Parameter<ushort>("AdcSyncLevel", "Уровень синхронизации", 0, 65535, 113, "hold");
        #endregion

        #region Режим обработки при регистрировании максимальных амплитуд
        public Parameter<ushort> AdcProcMode { get; set; } = new Parameter<ushort>("AdcProcMode", "Режим обработки при регистрировании максимальных амплитуд", 0, 2, 114, "hold");
        #endregion

        #region Таймер выдачи данных, мс
        public Parameter<ushort> TimerMax { get; set; } = new Parameter<ushort>("AdcTimerMax", "Таймер выдачи данных, мс", 0, 65535, 115, "hold");
        #endregion

        #region Команды
        #region Записать режим измерения АЦП
        RelayCommand _setAdcModeChange;
        public RelayCommand SetAdcModeChangeCommand => _setAdcModeChange ?? (_setAdcModeChange = new RelayCommand(p => {
            var settings = this.Clone() as AdcBoardSettings;
            settings.AdcMode.Value = this.AdcMode.WriteValue;
            SettingsChangedEvent?.Invoke(settings);
        }, p => true));
        #endregion

        #region Записать Режим синхронизации
        RelayCommand _setAdcSyncModeChangeCommand;
        public RelayCommand SetAdcSyncModeChangeCommand => _setAdcSyncModeChangeCommand ?? (_setAdcSyncModeChangeCommand = new RelayCommand(p => {
            var settings = this.Clone() as AdcBoardSettings;
            settings.AdcSyncMode.Value = this.AdcSyncMode.WriteValue;
            SettingsChangedEvent?.Invoke(settings);
        }, p => true));
        #endregion

        #region Записать Уровень синхронизации
        RelayCommand _setAdcSyncLevelChangeCommand;
        public RelayCommand SetAdcSyncLevelChangeCommand => _setAdcSyncLevelChangeCommand ?? (_setAdcSyncLevelChangeCommand = new RelayCommand(p => {
            var settings = this.Clone() as AdcBoardSettings;
            settings.AdcSyncLevel.Value = this.AdcSyncLevel.WriteValue;
            SettingsChangedEvent?.Invoke(settings);
        }, p => true));
        #endregion

        #region Записать Режим обработки при регистрировании максимальных амплитуд
        public RelayCommand _setAdcProcModeChangeCommand;
        RelayCommand SetAdcProcModeChangeCommand => _setAdcProcModeChangeCommand ?? (_setAdcProcModeChangeCommand = new RelayCommand(p => {
            var settings = this.Clone() as AdcBoardSettings;
            settings.AdcProcMode.Value = this.AdcProcMode.WriteValue;
            SettingsChangedEvent?.Invoke(settings);
        }, p => true));
        #endregion

        #region Записать Таймер выдачи данных
        RelayCommand _setTimerMaxChangeCommand;
        public RelayCommand SetTimerMaxChangeCommand => _setTimerMaxChangeCommand ?? (_setTimerMaxChangeCommand = new RelayCommand(p => {
            var settings = this.Clone() as AdcBoardSettings;
            settings.TimerMax.Value = this.TimerMax.WriteValue;
            SettingsChangedEvent?.Invoke(settings);
        }, p => true));
        #endregion

        
        #endregion

        #region Событие изменения настроек АЦП
        public event Action<AdcBoardSettings> SettingsChangedEvent;
        #endregion
        

        public object Clone()
        {
            return new AdcBoardSettings()
            {
                AdcMode = this.AdcMode.Clone() as Parameter<ushort>,
                AdcProcMode = this.AdcProcMode.Clone() as Parameter<ushort>,
                AdcSyncLevel = this.AdcSyncLevel.Clone() as Parameter<ushort>,
                AdcSyncMode = this.AdcSyncMode.Clone() as Parameter<ushort>,
                TimerMax = this.TimerMax.Clone() as Parameter<ushort>
            };
        }
    }
}
