using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

        void Start()
        {
            client = new UdpClient(localPort);
            IPEndPoint remoteIp = null;
            try
            {
                while (true)
                {
                    byte[] data = client.Receive(ref remoteIp);
                    ParseData(data);
                }
            }
            catch(Exception ex)
            {
                UdpErrorEvent?.Invoke($"UdpClient - {ex.Message}");
            }
            finally
            {
                client?.Close();
            }
        }

        void Stop()
        {
            client?.Close();
        }

        void ReInit()
        { 
            
        }

        void ParseData(byte[] data)
        { 
        
        }
        

    }
}
