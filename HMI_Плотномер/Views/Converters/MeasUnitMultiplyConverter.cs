using IDensity.DataAccess.Models;
using IDensity.DataAccess.Repositories;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IDensity.Core.Views.Converters
{
    [ValueConversion(typeof(object[]), typeof(int))]
    class MeasUnitMultiplyConverter : IMultiValueConverter
    {
        public MeasUnit CurMeasUnit { get; set; }
        private readonly EfRepository<MeasUnitMemory> _memoryRepository;

        public MeasUnitMultiplyConverter()
        {
            _memoryRepository = new EfRepository<MeasUnitMemory>();
        }
        public object Convert(object[] v, Type t, object p, CultureInfo c)
        {

            double k = 1;
            double b = 0;
            bool needToWriteMemory = false;
            if (v.Length < 3) return 0;
            if (!(v[0] is float result)) return 0;
            if (v[1] is MeasUnit unit)
            {
                needToWriteMemory = (CurMeasUnit != null) && (CurMeasUnit.Id != unit.Id) ? true : false;
                CurMeasUnit = unit;
                k = unit.K;
                b = unit.Offset;
            }
            double y = k * result + b;
            if (v[2] is string memoryId && needToWriteMemory)
            {
                SaveMemoryMeasUnit(memoryId, CurMeasUnit.Id);
            }
            return y.ToString("f3");

        }

        public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c)
        {
            var k = CurMeasUnit is null ? 1 : CurMeasUnit.K;
            var b = CurMeasUnit is null ? 0 : CurMeasUnit.Offset;
            float value = 0;
            if (!(float.TryParse(v.ToString(), out value))) value = 0;
            return new object[] { (value - b) / k, CurMeasUnit };
        }

        void SaveMemoryMeasUnit(string memoryId, long newId)
        {
            var memory = _memoryRepository.GetAll().Where(m => m.Name == memoryId).FirstOrDefault();
            if (!(memory is null))
            {
                memory.MeasUnitId = newId;
                _memoryRepository.Update(memory);
            }
        }


    }
}
