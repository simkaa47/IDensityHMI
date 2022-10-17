using IDensity.Services.CheckServices;
using IDensity.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace IDensity.Core.Services.CheckServices.Sensor
{
    class SensorCheckService
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly VM _vM;
        public SensorCheckService(CancellationTokenSource cancellationTokenSource, VM vM)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _vM = vM;
        }
        public async Task<SensorCheck> Check()
        {
            var result = new SensorCheck();
            var pulses = await new PulseCheckService(_cancellationTokenSource, _vM).Check();
            result.PulseCheck = pulses.FirstOrDefault();
            result.HvCheck = CheckHv();
            result.CheckResult = !(result.HvCheck.IsError || result.PulseCheck.IsError);
            return result;
        }

        DeviceCheckResult CheckHv()
        {
            var result = new DeviceCheckResult();
            result.IsError = Math.Abs(_vM.mainModel.TelemetryHV.VoltageSV.Value - _vM.mainModel.TelemetryHV.VoltageCurOut.Value) > 10;
            result.Status = $"Отклонение от уставки {_vM.mainModel.TelemetryHV.VoltageSV.Value - _vM.mainModel.TelemetryHV.VoltageCurOut.Value} В";
            return result;
        }
    }
}
