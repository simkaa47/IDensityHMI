using IDensity.DataAccess.Repositories;

namespace IDensity.DataAccess.Models
{
    public class MeasUnit : PropertyChangedBase, IDataBased
    {
        public long Id { get ; set ; }

        #region Имя
        /// <summary>
        /// Имя
        /// </summary>
        private string _name;
        /// <summary>
        /// Имя
        /// </summary>
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        #endregion

        #region К-т
        /// <summary>
        /// К-т
        /// </summary>
        private float _k = 1;
        /// <summary>
        /// К-т
        /// </summary>
        public float K
        {
            get => _k;
            set => Set(ref _k, value);
        }
        #endregion

        #region Смещение
        private float _offset;
        public float Offset
        {
            get => _offset;
            set => Set(ref _offset, value);
        }

        #endregion

        #region Тип прибора
        private int _deviceType;
        public int DeviceType
        {
            get => _deviceType;
            set
            {
                if (value >= 0 && value < 2) Set(ref _deviceType, value);
            }

        }
        #endregion

        #region Режим измререния
        /// <summary>
        /// Режим измререния
        /// </summary>
        private int _mode;
        /// <summary>
        /// Режим измререния
        /// </summary>
        public int Mode
        {
            get => _mode;
            set 
            {
                if(value>=0) Set(ref _mode, value);
            } 
        }
        #endregion

        public static bool CompareMeasUnits(MeasUnit first, MeasUnit second)
        {
            if (first is null || second is null) return false;
            return first.K == second.K && first.Offset == second.Offset;
        }

    }
}
