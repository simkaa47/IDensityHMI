using IDensity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.Services.CheckServices
{
    public class PulseCheckService : ICheckService
    {
        private readonly CancellationTokenSource _cancellationToken;
        private readonly VM _vM;

        public event Action<string> ProcessEvent;

        public async Task<List<DeviceCheckResult>> Check()
        {
            var result = new DeviceCheckResult() { ProcessName = "Проверка коректности импульсов от платы АЦП" };
            if (await CheckSetAddr(result))
            {
                SwitchOscillMode();
                await WritePulse(result);
            }
            var list = new List<DeviceCheckResult>();
            list.Add(result);
            return list;
        }

        public PulseCheckService(CancellationTokenSource cancellationToken,VM vM)
        {
            _cancellationToken = cancellationToken;
            _vM = vM;
        }

        /// <summary>
        /// Проверка адреса
        /// </summary>
        async Task<bool> CheckSetAddr(DeviceCheckResult result)
        { 
            var localInterfases =  NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
                .Where(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Where(ni => ni.GetIPProperties().UnicastAddresses.Any(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork))
                .Select(ni => ni.GetIPProperties().UnicastAddresses
                    .Where(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Select(addr => addr.Address).First()
                    .ToString()

                ).ToList();
            //Проверка запущен ли TCP
            var tcpLocal = "";

            if (_vM.CommService.Tcp.Stream != null)
            {
                tcpLocal = _vM.CommService.Tcp.Client.Client.LocalEndPoint
                    .ToString()
                    .Split(new char[] { ':', '[', ']' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToArray()[1];
            }
            if (localInterfases.Count == 0)
            {
                result.IsError = true;
                result.Status = "На локальном компьютере не найдено доступных интерфейсов";
                return false;
            }
            while (!localInterfases.Contains(_vM.mainModel.UdpAddrString))
            {
                string ip = localInterfases.Contains(tcpLocal) ? tcpLocal: localInterfases[0];
                ProcessEvent?.Invoke($"Установка корректного IP UDP сервера ({ip})");
                byte temp = 0;
                var bytes = ip.Split(".", StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => byte.TryParse(s, out temp))
                    .Select(s => temp)
                    .ToArray();
                _vM.CommService.SetUdpAddr(bytes, 49051);
                await Task.Delay(1000, _cancellationToken.Token);
            }
            return true;
        }
        /// <summary>
        /// Включить режим осциллографа и обнулить спектр
        /// </summary>
        /// <returns></returns>
        void SwitchOscillMode()
        {
            _vM.CommService.StartStopAdcData(0);
            _vM.CommService.SwitchAdcBoard(0);
            _vM.CommService.SetAdcMode(0);
            _vM.CommService.StartStopAdcData(1);
            _vM.CommService.SwitchAdcBoard(1);
            _vM.AdcViewModel.UdpService.Start();
            _vM.AdcViewModel.CheckPulseService.Clear();
        }


        async Task WritePulse(DeviceCheckResult result)
        {
            ProcessEvent?.Invoke($"Обработка импульсов с платы АЦП...");
            await Task.Delay(10000, _cancellationToken.Token);
            var pulses = _vM.AdcViewModel.CheckPulseService.CommontCount;
            var noises = _vM.AdcViewModel.CheckPulseService.NoisePulses;
            var over = _vM.AdcViewModel.CheckPulseService.OverValuePulses;
            var percent = _vM.AdcViewModel.CheckPulseService.SuccessDeviation;
            result.Status += $"Проверено импульсов от платы АЦП : {pulses}\n\r";
            result.Status += $"Количество импульсов с пересветом : {over}\n\r";
            result.Status += $"Количество импульсов с шумами : {noises}\n\r";
            result.Status += $"Процент корректных импульсов : {percent.ToString("f3")}\n\r";
            result.Status += $"Условие проверки - процент корректных импульсов должен быть более 90%, за 10 секунд от платы АЦП должно быть получено не менее 100 импульсов \n\r";
            if (pulses <= 100)
            {
                result.Status += $"Проверка импульсов не пройдена, поскольку получено слишком мало импульсов от платы АЦП";
                result.IsError = true;
            }
            else if (percent < 90)
            {
                result.Status += $"Процент корректных импульсов : {percent.ToString("f3")}, проверка формы импульсов не выполнена";
                result.IsError = true;
            }
            else
                result.Status += $"Процент корректных импульсов : {percent.ToString("f3")}, проверка формы импульсов пройдена успешно";

        }
    }
}
