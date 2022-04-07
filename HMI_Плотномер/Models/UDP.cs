using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IDensity.Models
{
    
    /// <summary>
    /// В этом классе происходит парсинг пакетов, приходящих по UDP
    /// </summary>
    class UDP:PropertyChangedBase
    {
        // 0 - Осциллограмма, 1 - неполный спектр, 2 - полный спектр, 3 - счетчики
        private int _mode;

        public int Mode
        {
            get { return _mode; }
            set { Set(ref _mode, value); }
        }


        #region UDP клиент
        UdpClient client;
        int localPort = 49051;
        #endregion

        #region Событие ошибки UDP
        public event Action<string> UdpErrorEvent;
        #endregion

        #region Событие "Готовы данные для тренда в режиме осциллографа"
        public event Action<List<Point>> UpdateOscillEvent;
        #endregion
        #region Событие "Готовы данные в режиме макс амплитуд"
        public event Action<List<Point>> UpdateAmplitudesEvent;
        #endregion
        int index = 0;
        List<Point> CurOscillList = new List<Point>();

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

        int nextPacketNum = 1;

        void ParseData(byte[] data) 
        {
            if (data.Length < 10 || data[data.Length-1]!=35) return;
            byte num = 0;
            var list = Encoding.ASCII.GetString(data, 0, 10).Split(",", StringSplitOptions.RemoveEmptyEntries).Where(s => byte.TryParse(s, out num)).Select(s => num).ToList();
            if (list.Count != 2) return;
            int count = list[0];// общее количество пакетов
            int packetNum = list[1];// номер пакета в серии
            if (packetNum != nextPacketNum)
            {
                nextPacketNum = 1;
                return;
            }
            nextPacketNum = (nextPacketNum + 1) > count ? 1 : nextPacketNum+1;
            string header = Encoding.ASCII.GetString(data, 0, 5);
            switch (header)
            {
                case "*AOLV":
                    Mode = 0;
                    AddToCollection(2, data.Length);
                    // если посдледний пакет, то можно соообщить о том что он собран
                    if (packetNum >= count) UpdateOscillEvent?.Invoke(CurOscillList);
                    break;
                case "*AML1":
                case "*AML2":
                    Mode = header=="*AML1"?1:2;
                    AddToCollection(2, packetNum == count ? data.Length : data.Length - 10);
                    if (packetNum >= count) UpdateOscillEvent?.Invoke(CurOscillList);
                    break;
                case "*AML3":
                    Mode = 3;
                    AddToCollection(4, data.Length );
                    if (packetNum >= count) UpdateAmplitudesEvent?.Invoke(CurOscillList);
                    break;
                default: break;
            }
            // локальная функция прохода по массиву
            void AddToCollection(int byteInBum, int bytesAvialable)
            {
                if (packetNum == 1) 
                {
                    CurOscillList = new List<Point>();
                    index = 0;
                }                
                for (int i = 10 + byteInBum-1; i < bytesAvialable; i += byteInBum)
                {
                    int temp = 0;
                    for (int j = i; j > i-byteInBum; j--)
                    {
                        temp += (data[j] << ((j-i+byteInBum-1) * 8));
                    }
                    CurOscillList.Add(new Point(index, temp));
                    index++;
                }                
            }

        }
        

    }
}
