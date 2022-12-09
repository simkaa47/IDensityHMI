using IDensity.AddClasses;
using IDensity.Models;
using IDensity.Services.ComminicationServices;
using IDensity.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.Services.CheckServices
{
    public class AnalogCheckService :PropertyChangedBase, ICheckService
    {
        private readonly CancellationTokenSource _cancellationToken;
        private readonly VM _vM;
        private readonly MainModel _model;
        private readonly CommunicationService _commService;

        public event Action<string> ProcessEvent = delegate { };

        #region Процент выполнения
        /// <summary>
        /// Процент выполнения
        /// </summary>
        private double _processPercent;
        /// <summary>
        /// Процент выполнения
        /// </summary>
        public double ProcessPercent
        {
            get => _processPercent;
            set => Set(ref _processPercent, value);
        }
        #endregion

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
            List<DeviceCheckResult> result = new List<DeviceCheckResult>();
            for (int i = 0; i < 2; i++)
            {
                result.Add(await CheckAnalog(i));
            }
            return result;
        }
        /// <summary>
        /// Проверить отдельно один аналоговый выход
        /// </summary>
        /// <param name="modNum"></param>
        /// <returns></returns>
        async Task<DeviceCheckResult> CheckAnalog(int modNum)
        {
            await SwitchAnalog(modNum);
            var result = new DeviceCheckResult() { ProcessName = $"Проверка работы аналогового выхода {modNum}" };
            for (int i = 4; i <= 20; i += 8)
            {
                if (!await SetCurrent(i, modNum, result))
                {
                    ProcessPercent += 50;
                    break;
                }
                ProcessPercent += 50.0 / 3.0;
            }
            return result;
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

        async Task<bool> SetCurrent(int setValue, int moduleNum, DeviceCheckResult result)
        {
            ProcessEvent?.Invoke($"Аналоговый выход {moduleNum} : Выставляем ток {setValue} mA");
            _model.AnalogGroups[moduleNum].AO.AmTestValue.Value = (ushort)(setValue * 1000);
            _commService.SetTestValueAm(moduleNum, 0, (ushort)(setValue * 1000));
            await Task.Delay(5000, _cancellationToken.Token);
            var currentLevel = (float)_model.AnalogGroups[moduleNum].AO.AdcValue.Value / 69;
            if (_model.AnalogGroups[moduleNum].AO.VoltageTest.Value < 200)
            {
                var msg = $"Аналоговый выход {moduleNum} : Обрыв цепи аналогового выхода";
                ProcessEvent?.Invoke(msg);
                result.Status = msg;
                result.IsError = true;
                return false;
            }
            else
            {
                var deviation = Math.Abs(currentLevel - setValue) / 16 * 100;
                result.IsError = deviation > 0.1 ? true : false;
                var msg = ($"Аналоговый выход {moduleNum} :   " +
                    $"Результат замера {currentLevel} mA при ожидаемых {setValue} mA " +
                    $"(Погрешность {deviation}%)");
                ProcessEvent?.Invoke(msg);
                result.Status += msg + "\n\r";
            }
            return true;
        }


    }
}
