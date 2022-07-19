using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses.AdcBoardSettings
{
    public class AdcBoardSettings
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

        #region Коэффициент платы предусиления
        public Parameter<ushort> PreampGain { get; set; } = new Parameter<ushort>("PreampGain", "Коэффицент предусиления", 0, 255, 116, "hold");
        #endregion
       
    }
}
