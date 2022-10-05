using IDensity.DataAccess.Models;
using IDensity.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace IDensity.Views.Converters
{
    public class GetMeasNumConverter : IMultiValueConverter
    {
        private readonly EfRepository<MeasUnitMemory> _measUnitMemoryRepository;
        
        public GetMeasNumConverter()
        {
            _measUnitMemoryRepository = new EfRepository<MeasUnitMemory>();
        }

        public MeasUnit CurrentMeasUnit { get; set; }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return null;
            if(!(values[0] is IEnumerable<MeasUnit> measUnits)) return null;
            if (!(values[1] is int mode)) return null;
            if (values.Length == 2) return measUnits.Where(mu => mu.Mode == mode).ToList();
            if (!(values[2] is string idString)) return null;
            MeasUnit selected = null;
            if (!(IsMemoryExist(idString)))
            {
                long measUnitId = 0;
                selected = measUnits.Where(mu => mu.Mode == mode).FirstOrDefault();
                if (selected != null) measUnitId = selected.Id;
                _measUnitMemoryRepository.Add(new MeasUnitMemory
                {
                    Name = idString,
                    MeasUnitId = measUnitId
                });
            }
            long index = GetId(idString);
            selected = measUnits.Where(m => m.Id == index).FirstOrDefault();
            if (selected is null || selected.Mode!=mode)selected = measUnits.Where(mu => mu.Mode == mode).FirstOrDefault();
            if (selected is null) return null;
            SetId(idString, selected.Id);
            return selected;


        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var ret = new object[2];
            return ret;
        }

        long GetId(string id)
        {
            var memorie = _measUnitMemoryRepository.GetAll().Where(m => m.Name == id).FirstOrDefault();
            return memorie is null ? 0 : memorie.MeasUnitId;
        }

        bool IsMemoryExist(string id)
        {
            return _measUnitMemoryRepository.GetAll().Any(m => m.Name == id);
        }

        void SetId(string memoryId,long measUnitId)
        {
            var memory = _measUnitMemoryRepository.GetAll().Where(m => m.Name == memoryId).FirstOrDefault();
            if(!(memory is null))
            {
                memory.MeasUnitId = measUnitId;
                _measUnitMemoryRepository.Update(memory);
            }
        }

        

    }
}
