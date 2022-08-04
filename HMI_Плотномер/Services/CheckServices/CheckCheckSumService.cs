using IDensity.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.Services.CheckServices
{
    public class CheckCheckSumService : ICheckService
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly VM _vM;

        public event Action<string> ProcessEvent;
        public CheckCheckSumService(CancellationTokenSource cancellationTokenSource,VM vM)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _vM = vM;
        }

        public async Task<List<DeviceCheckResult>> Check()
        {
            var result = new DeviceCheckResult() { ProcessName = "Проверка контрольной суммы ПО прибора" };
            ProcessEvent?.Invoke("Запрос контрольной суммы ПО прибора..");
            _vM.CommService.GetCheckSum();
            await Task.Delay(5000, _cancellationTokenSource.Token);
            if (_vM.mainModel.CurCheckSum != _vM.mainModel.CheckSum.Value)
            {
                result.IsError = true;
                result.Status = $"Контрольная сумма ПО прибора не совпадает с желаемой ({_vM.mainModel.CheckSum.Value})";
            }
            else
                result.Status = $"Проверка контрольной суммы проведена успешно";
            var list = new List<DeviceCheckResult>();
            list.Add(result);
            return list;
        }
    }
}
