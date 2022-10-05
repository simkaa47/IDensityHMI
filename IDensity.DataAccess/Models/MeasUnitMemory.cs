using IDensity.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.DataAccess.Models
{
    public class MeasUnitMemory : PropertyChangedBase, IDataBased
    {
        public long Id { get; set ; }

        private string _name;
        /// <summary>
        /// Имя
        /// </summary>
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private long _measUnitId;
        public long MeasUnitId
        {
            get => _measUnitId;
            set => Set(ref _measUnitId, value);

        }
    }
}
