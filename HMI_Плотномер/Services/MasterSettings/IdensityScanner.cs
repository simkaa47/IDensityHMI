using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.Services.MasterSettings
{
    public class IdensityScanner : IDisposable
    {
        private readonly string adress;
        private readonly int portNum;
        public TcpClient Client { get; private set; }
        NetworkStream stream;
        byte[] inBuffer;

        public IdensityScanner(IPAddress adress, int portNum)
        {
            this.adress = adress.ToString();
            this.portNum = portNum;
            Client = new TcpClient();
            Client.ReceiveTimeout = 5000;
        }

        public event Action<string> FoundedDeviceEvent = delegate { };
        public event Action<string> FaultSearchDeviceEvent = delegate { };

        public async Task Scan(List<string> adresses, CancellationToken cancellation)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (cancellation.IsCancellationRequested) return;
                    FaultSearchDeviceEvent?.Invoke(adress);
                    Connect();
                    if (Client.Connected)
                    {
                        stream = Client.GetStream();
                        inBuffer = new byte[100];
                        var str = AskResponse(Encoding.ASCII.GetBytes("*CMND,DTG#"));
                        if (str.Length == 23)
                        {                            
                            adresses.Add(adress);
                            FoundedDeviceEvent?.Invoke(adress);
                        }

                    }
                }
                catch (Exception ex)
                {
                    //FaultSearchDeviceEvent?.Invoke($"{DateTime.Now.ToString("G")} : {adress}:{ex.Message}");
                }
                finally
                {
                    if (Client != null) Client.Close();
                }
            });
        }

        void Connect()
        {
            Console.WriteLine($"{DateTime.Now.ToString("G")} : {adress}:Подключение");
            CancellationToken ct = new CancellationToken();
            if (Client.ConnectAsync(adress, portNum).Wait(2500, ct))
            {
                ct.ThrowIfCancellationRequested();
            }            
        }

        public void Dispose()
        {
            
        }

        string AskResponse(byte[] buffer)
        {

            stream.Write(buffer, 0, buffer.Length);
            int num = 0;
            int offset = 0;
            do
            {
                num = stream.Read(inBuffer, offset, inBuffer.Length - offset);
                Thread.Sleep(10);
                offset += num;

            } while (stream.DataAvailable && (inBuffer.Length - offset) > 0);

            return Encoding.ASCII.GetString(inBuffer, 0, offset);// Получаем строку из байт;            
        }
    }
}
