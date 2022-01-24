using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IDensity.Models
{
    /// <summary>
    /// В этом классе происходит парсинг пакетов, приходящих по UDP
    /// </summary>
    class UDP
    {
        #region UDP клиент
        UdpClient client;
        int localPort = 49051;
        #endregion

        #region Событие
        public event Action<string> UdpErrorEvent;
        #endregion

        Queue<List<ushort>> OscillModeTrendQueue { get; set; } = new Queue<List<ushort>>();
        List<ushort> CurOscillList = new List<ushort>();

        #region Конструктор
        public UDP()
        {
            Start();
        }
        #endregion

        public async void Start()
        {
            if (client != null) return;
            client = new UdpClient(localPort);
            IPEndPoint remoteIp = null;
            await Task.Run(() => 
            {
                try
                {
                    while (true)
                    {
                        byte[] data = client.Receive(ref remoteIp);
                        ParseData(data);
                    }
                }
                catch (Exception ex)
                {
                    UdpErrorEvent?.Invoke($"UdpClient - {ex.Message}");
                }
                finally
                {
                    client?.Close();
                }
            });
            
        }

        public void Stop()
        {
            client?.Close();
        }

        

        void ParseData(byte[] data)
        {
            if (data.Length < 10 || data[data.Length-1]!=35) return;
            string header = Encoding.ASCII.GetString(data, 0, 5);
            switch (header)
            {
                case "*AOLV":
                    byte num = 0;
                    var list = Encoding.ASCII.GetString(data, 0, 10).Split(",", StringSplitOptions.RemoveEmptyEntries).Where(s => byte.TryParse(s, out num)).Select(s => num).ToList();
                    if (list.Count != 2) return;
                    if (list[0] == 0) CurOscillList = new List<ushort>();
                    for (int i = 11; i < data.Length; i+=2)
                    {
                        CurOscillList.Add((ushort)(data[i] * 256 + data[i - 1]));
                    }
                    break;
                default: break;
            }

        }
        

    }
}
