using IDensity.Services.CheckServices;
using IDensity.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.Core.Services.CheckServices.ElectronicUnit
{
    public class ElectronicUnitCheckService
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly VM _vM;

        public ElectronicUnitCheckService(CancellationTokenSource cancellationTokenSource, VM vM)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _vM = vM;
        }

        public async Task<ElectronicUnitCheck> Check()
        {
            var result = new ElectronicUnitCheck();
            var analogs = await new AnalogCheckService(_cancellationTokenSource, _vM).Check();
            result.AnalogCheck0 = analogs[0];
            result.AnalogCheck1 = analogs[1];
            result.HvSourceCheck = CheckHv();
            var soft= await new CheckCheckSumService(_cancellationTokenSource, _vM).Check();
            result.SoftwareCheck = soft[0];
            var rtc = await new RtcCheckService(_cancellationTokenSource, _vM).Check();
            result.RtcCheck = rtc[0];
            result.CheckResult = !(result.AnalogCheck0.IsError
                || result.AnalogCheck1.IsError 
                || result.HvSourceCheck.IsError
                || result.RtcCheck.IsError
                || result.SoftwareCheck.IsError);
            return result;
        }

        DeviceCheckResult CheckHv()
        {
            DeviceCheckResult result = new DeviceCheckResult();
            var deviation = Math.Abs((_vM.mainModel.TelemetryHV.VoltageCurIn.Value - 12) / 12) * 100;
            result.IsError = deviation > 10;
            result.Status = $"Напряжение {_vM.mainModel.TelemetryHV.VoltageCurIn.Value} В при ожидаемых 12 В";
            return result;
        }

        
    }
}
