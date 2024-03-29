﻿using IDensity.AddClasses;
using IDensity.Core.Models.Adc;
using IDensity.DataAccess;
using IDensity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IDensity.Services.ComminicationServices
{

    /// <summary>
    /// В этом классе происходит парсинг пакетов, приходящих по UDP
    /// </summary>
    public class UdpService : PropertyChangedBase
    {
        // 0 - Осциллограмма, 1 - неполный спектр, 2 - полный спектр, 3 - счетчики
        private int _mode;

        public int Mode
        {
            get { return _mode; }
            set { Set(ref _mode, value); }
        }


        #region UDP клиент
        public UdpClient Client { get; private set; }
        int localPort = 49051;
        IPEndPoint remoteIp = null;
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
        public UdpService(AdcAvgSettings settings, VM vM)
        {
            _settings = settings;
            _vM = vM;
        }
        #endregion

        public async void Start()
        {
            if (Client != null) return;
            
            await Task.Run(() =>
            {
                SetClient(_vM.mainModel.PortUdp);
                while (true)
                {

                    try
                    {
                        while (true)
                        {
                            byte[] data = Client.Receive(ref remoteIp);
                            ParseData(data); 
                        }
                    }
                    catch (Exception ex)
                    {
                        UdpErrorEvent?.Invoke($"UdpClient - {ex.Message}");
                    }
                    finally
                    {
                        Client?.Close();
                        SetClient(_vM.mainModel.PortUdp);
                    } 
                }
            });

        }

        void SetClient(int port)
        {
            Client = new UdpClient(port);
            Client.Client.ReceiveTimeout = 5000;
            remoteIp = null;
        }

        public void Stop()
        {
            Client?.Close();
        }

        int nextPacketNum = 1;
        private readonly AdcAvgSettings _settings;
        private readonly VM _vM;

        void ParseData(byte[] data)
        {
            if (data.Length < 10 || data[data.Length - 1] != 35) return;
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
            nextPacketNum = nextPacketNum + 1 > count ? 1 : nextPacketNum + 1;
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
                    Mode = header == "*AML1" ? 1 : 2;
                    AddToCollection(2, Math.Min(1034, data.Length));
                    if (packetNum >= count)
                    {
                        if (Mode == 2) UpdateOscillEvent?.Invoke(FilterSpectr(CurOscillList));
                        else UpdateOscillEvent?.Invoke(CurOscillList);
                    }
                    break;
                case "*AML3":
                    Mode = 3;
                    AddToCollection(4, data.Length);
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
                for (int i = 10 + byteInBum - 1; i < bytesAvialable; i += byteInBum)
                {
                    int temp = 0;
                    for (int j = i; j > i - byteInBum; j--)
                    {
                        temp += data[j] << (j - i + byteInBum - 1) * 8;
                    }
                    CurOscillList.Add(new Point(index, temp));
                    index++;
                }
            }

            List<Point> FilterSpectr(List<Point> unFilter)
            {
                List<Point> filtered = new List<Point>();
                double sum = 0; ;
                for (int i = 0; i < unFilter.Count; i++)
                {
                    sum += unFilter[i].Y;
                    if (i < _settings.SpectrFilterDeep)
                    {
                        filtered.Add(new Point(unFilter[i].X, sum / (i + 1)));
                    }
                    else
                    {
                        sum -= unFilter[i - _settings.SpectrFilterDeep].Y;
                        filtered.Add(new Point(unFilter[i].X, sum / _settings.SpectrFilterDeep));
                    }
                }
                return filtered;
            }

        }


    }
}
