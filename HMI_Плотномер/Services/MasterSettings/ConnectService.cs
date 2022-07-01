using IDensity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.Services.MasterSettings
{
    public class ConnectService
    {
        public  List<NetworkAdapterData> GetAdapterData()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
                .Where(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Where(ni => ni.GetIPProperties().UnicastAddresses.Any(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork))
                .Select(ni => new NetworkAdapterData
                {
                    Name = ni.Name,
                    InterfaceType = ni.NetworkInterfaceType.ToString(),
                    Description = ni.Description,
                    Ip = ni.GetIPProperties().UnicastAddresses
                    .Where(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Select(addr => addr.Address).First()
                    .ToString()

                }).ToList();
            
        }
        public List<string> FoundedDevicesIP;

        public double processCurrent;

        public event Action<string> TryScanEvent;
        public event Action<string> FoundDeviceEvent;

        public async Task Scan(IEnumerable<NetworkAdapterData> adapters, CancellationToken cancellation)
        {
            processCurrent = 0;
            var count = adapters.Count();
            double price = 100.0/255.0 / count;
            FoundedDevicesIP = new List<string>();

            foreach (var adapter in adapters)
            {
                
                byte temp = 0;
                var addr = adapter.Ip.Split(".")
                    .Where(s => byte.TryParse(s, out temp))
                    .Select(s => temp)
                    .ToArray();
                if (addr.Length != 4) 
                {
                    processCurrent += 255 * price;
                    continue;
                } 
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < 255; i++)
                {
                    if (cancellation.IsCancellationRequested) return;
                    var ip = new IPAddress(new byte[] { addr[0], addr[1], addr[2], (byte)i });
                    var scaner = new IdensityScanner(ip, 49050);
                    scaner.FoundedDeviceEvent += (s) => FoundDeviceEvent?.Invoke(s);
                    scaner.FaultSearchDeviceEvent += (s) =>
                    {
                        TryScanEvent?.Invoke(s);
                        processCurrent += price;
                    };
                    tasks.Add(scaner.Scan(FoundedDevicesIP,cancellation));
                                       
                }
                await Task.WhenAll(tasks);
            }
            
        }

    }
}
