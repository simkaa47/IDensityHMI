using IDensity.DataAccess;
using IDensity.Services.CheckServices;
using IDensity.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.Core.Services.CheckServices.ElectronicUnit
{
    public class ElectronicUnitCheckService : PropertyChangedBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly VM _vM;

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

        public ElectronicUnitCheckService(CancellationTokenSource cancellationTokenSource, VM vM)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _vM = vM;
        }



        public async Task<ElectronicUnitCheck> Check()
        {
            var result = new ElectronicUnitCheck();
            ProcessPercent = 0;
            var analogService = new AnalogCheckService(_cancellationTokenSource, _vM);
            analogService.PropertyChanged += (o, e) =>
            {

                if (e.PropertyName == nameof(analogService.ProcessPercent))
                {
                    ProcessPercent = 80.0 / 100.0 * analogService.ProcessPercent;
                }
            };
            var analogs = await analogService.Check();

            result.AnalogCheck0 = analogs[0];
            result.AnalogCheck1 = analogs[1];
            result.HvSourceCheck = CheckHv();

            var soft = await new CheckCheckSumService(_cancellationTokenSource, _vM).Check();
            ProcessPercent = 90;
            result.SoftwareCheck = soft[0];
            var rtc = await new RtcCheckService(_cancellationTokenSource, _vM).Check();
            ProcessPercent = 100;
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
            var deviation = Math.Abs(_vM.mainModel.TelemetryHV.VoltageCurIn.Value - 12);
            result.IsError = deviation > 0.5;
            result.Status = $"Напряжение {_vM.mainModel.TelemetryHV.VoltageCurIn.Value.ToString("f3")} В при ожидаемых 12\n\r" +
                $"Отклонение напряжения должно быть не более 12 ± 0.5 В\n\r";
            if (result.IsError)
            {
                result.Status += $"Отклонение {deviation.ToString("f3")} В, проверка не выполнена";
            }
            else result.Status += $"Отклонение {deviation.ToString("f3")} В, проверка выполнена успешно";
            return result;
        }




    }
}
