using IDensity.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Core.Services.CheckServices.Process
{
    public class GetProcesesService
    {
        private readonly VM _vM;

        public GetProcesesService(VM vM)
        {
            _vM = vM;
        }

        public List<ProcessParameter> GetProcessParameters()
        {
            var list = new List<ProcessParameter>();
            var param = new ProcessParameter
            {
                Description = "Температура датчика на плате прибора",
                MeasUnit = "℃",
                Value = _vM.mainModel.TempTelemetry.TempInternal.Value,
                MaxValue = 80,
                MinValue = -40
            };
            if(param.Value>-40 && param.Value<=80)
            {
                param.Result = "Измеренное значение температуры находится в пределах допустимых значений температуры, проверка температуры пройдена успешно";
            }
            else param.Result = "Измеренное значение температуры не находится в пределах допустимых значений температуры, проверка температуры не пройдена";

            list.Add(param);
            return list;
        }
    }
}
