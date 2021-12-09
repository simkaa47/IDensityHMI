using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    class TempBoardTelemetry
    {
        #region Температура с внешнего датчика, С
        public Parameter<float> TempInternal { get; } = new Parameter<float>("TempInternal", "Температура с внешнего датчика, С", 0, float.PositiveInfinity, 8, "read")
        {           
            OnlyRead = true
        };
        #endregion

        #region Температура с внутреннего датчика, С
        public Parameter<float> TempExternal { get; } = new Parameter<float>("TempExternal", "Температура с внутреннего датчика, С", 0, float.PositiveInfinity, 9, "read")
        {           
            OnlyRead = true
        };
        #endregion

        #region Статус связи с платой питания
        public Parameter<bool> TempBoardCommState { get; } = new Parameter<bool>("TempBoardCommState", "Статус связи с платой питания", false, true, 0, "");
        #endregion
    }
}
