using IDensity.Services.CheckServices;
using IDensity.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using IDensity.AddClasses;
using IDensity.DataAccess;

namespace IDensity.Core.Services.CheckServices.Sensor
{
    class SensorCheckService:PropertyChangedBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly VM _vM;
        public SensorCheckService(CancellationTokenSource cancellationTokenSource, VM vM)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _vM = vM;
        }

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
        public async Task<SensorCheck> Check()
        {
            ProcessPercent = 0;
            var result = new SensorCheck();            
            var pulses = await new PulseCheckService(_cancellationTokenSource, _vM).Check();
            result.PulseCheck = pulses.FirstOrDefault();
            ProcessPercent = 50;
            result.HvCheck = CheckHv();
            result.CheckResult = !(result.HvCheck.IsError || result.PulseCheck.IsError);
            ProcessPercent = 100;
            return result;
        }

        DeviceCheckResult CheckHv()
        {
            var result = new DeviceCheckResult();
            result.IsError = Math.Abs(_vM.mainModel.TelemetryHV.VoltageSV.Value - _vM.mainModel.TelemetryHV.VoltageCurOut.Value) > 10;
            result.Status = $"Измеренное значение высокого напряжения {_vM.mainModel.TelemetryHV.VoltageCurOut.Value} В\n\r";
            result.Status += $"Уставка {_vM.mainModel.TelemetryHV.VoltageSV.Value} В\n\r";
            result.Status += $"Условие проверки - отклонение высокого напряжения от уставки не должно быть более 10 В\n\r";
            result.Status += $"Отклонение от уставки {_vM.mainModel.TelemetryHV.VoltageSV.Value - _vM.mainModel.TelemetryHV.VoltageCurOut.Value} В,";
            if(result.IsError) result.Status += $" проверка не пройдена";
            else result.Status += $" проверка выполнена успешно";
            return result;
        }
    }
}
