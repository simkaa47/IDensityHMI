using IDensity.AddClasses;
using IDensity.Models;
using IDensity.Services.ComminicationServices;
using IDensity.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.Services.CheckServices
{
    public class AnalogCheckService : ICheckService
    {
        private readonly CancellationTokenSource _cancellationToken;
        private readonly VM _vM;
        private readonly MainModel _model;
        private readonly CommunicationService _commService;

        public event Action<string> ProcessEvent = delegate{};

        public AnalogCheckService(CancellationTokenSource cancellationToken,
            VM vM)
        {
            _cancellationToken = cancellationToken;
            _vM = vM;
            _model = vM.mainModel;
            _commService = vM.CommService;
        }
        public async Task<List<DeviceCheckResult>> Check()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Проверить отдельно один аналоговый выход
        /// </summary>
        /// <param name="modNum"></param>
        /// <returns></returns>
        async Task<DeviceCheckResult> CheckAnalog(int modNum)
        {
            await Task.Delay(1);
            return new DeviceCheckResult();
        }

        async Task SwitchAnalog(int outNum)
        {
            bool needSetSettings = false;
            ProcessEvent?.Invoke($"Аналоговый выход {outNum} : подготовка к проверке");
            do
            {
                if (!(_model.AnalogGroups[outNum].AO.Clone() is AnalogOutput analog)) return;
                analog.DacType.Value = 0;
                needSetSettings = false;
                if (!_model.AnalogGroups[outNum].AO.CommState.Value)
                    needSetSettings = await CheckAnalogAndExecute(() =>
                    {
                        _commService.SwitchAm(outNum, 0, true);
                        _commService.SwitchAm(outNum, 1, true);
                    });

                if (_model.AnalogGroups[outNum].AO.DacType.Value != 0)
                    needSetSettings = await CheckAnalogAndExecute(() => _commService.ChangeDacAct(outNum, 0, analog));

            } while (needSetSettings && !_cancellationToken.IsCancellationRequested);
        }

        async Task<bool> CheckAnalogAndExecute(Action action)
        {
            if (_cancellationToken.IsCancellationRequested) return false;
            action?.Invoke();
            await Task.Delay(1000, _cancellationToken.Token);
            return true;
        }

        
    }
}
