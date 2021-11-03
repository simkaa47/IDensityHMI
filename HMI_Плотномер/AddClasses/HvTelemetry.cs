using System;
using System.Collections.Generic;
using System.Text;

namespace HMI_Плотномер.AddClasses
{
    class HvTelemetry
    {
        public Parameter<ushort> VoltageSV { get; } = new Parameter<ushort>("Уставка напряжения, 8000-40000, в еденицах по 50mV", "read", 4);
        public Parameter<float> VoltageCurIn { get; } = new Parameter<float>("Входное напряжение, 0-65535 mV", "read", 5);
        public Parameter<ushort> VoltageCurOut { get; } = new Parameter<ushort>("Выходное напряжение, 8000-40000, в еденицах по 50mV", "read", 6);
        public Parameter<ushort> Current { get; } = new Parameter<ushort>("Ток, 0-65535 mA", "read", 7);
        public Parameter<bool> HvOn { get; } = new Parameter<bool>("Статус высокого напряжения", "read", 7);
    }
}
