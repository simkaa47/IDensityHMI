using HMI_Плотномер.AddClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HMI_Плотномер.Models
{
    class MainModel
    {
        ObservableCollection<TimePoint> _plotCollection;
        public ObservableCollection<TimePoint> PlotCollection
        {
            get => _plotCollection ?? (_plotCollection = new ObservableCollection<TimePoint>());
        }
        
        public async void ModelProcess()
        {
            await Task.Run(() => AddCollection());
        }

        void AddCollection()
        {
            int sdfsd = 14;
            sdfsd++;
            while (true)
            {
                
                App.Current?.Dispatcher?.Invoke(Add) ;
                Thread.Sleep(1000);
            }
        }
        void Add()
        {
            PlotCollection.Add(new TimePoint { time = DateTime.Now, y = new Random().Next(18, 32) });
        }
    }
}
