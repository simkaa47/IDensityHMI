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
            list.Add(new ProcessParameter
            {
                Description="Температура прибора",
                MeasUnit= "℃",
                Value=_vM.mainModel.TempTelemetry.TempInternal.Value,
                MaxValue=80,
                MinValue=-40
            });
            return list;
        }
    }
}
