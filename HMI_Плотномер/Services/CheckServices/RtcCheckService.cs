using IDensity.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.Services.CheckServices
{
    public class RtcCheckService : ICheckService
    {
        private readonly CancellationTokenSource _cancellationToken;
        private readonly VM _vM;

        public event Action<string> ProcessEvent;

        public RtcCheckService(CancellationTokenSource cancellationToken, VM vM)
        {
            _cancellationToken = cancellationToken;
            _vM = vM;
        }

        public async Task<List<DeviceCheckResult>> Check()
        {
            var result = new DeviceCheckResult() { ProcessName = "Проверка коректности работы модуля RTC" };
            var list = new List<DeviceCheckResult>();
            var firstDate = _vM.mainModel.Rtc.Value;            
            ProcessEvent?.Invoke($"Замер данных от модуля RTC контроллера...");
            await Task.Delay(10000);
            var lastDate = _vM.mainModel.Rtc.Value;
            var diff = lastDate - firstDate;
            result.Status += $"За десять секунд состояние модуля RTC изменилось на  " +
                $": {diff.TotalSeconds} секунд\n\r";
            if (diff.TotalSeconds < 8)
            {                
                result.IsError = true;
            }            
            list.Add(result);
            return list;
        }
    }
}
