using System;
using System.Collections.Generic;
using System.Text;

namespace HMI_Плотномер.AddClasses
{
    class TempBoardTelemetry
    {
        public Parameter<float> TempInternal { get; } = new Parameter<float>("Температура с внешнего датчика, С", "read", 8);
        public Parameter<float> TempExternal { get; } = new Parameter<float>("Температура с внутреннего датчика, С", "read", 9);
    }
}
