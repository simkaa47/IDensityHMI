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
        private double _k;
        /// <summary>
        /// К-т
        /// </summary>
        public double K
        {
            get => _k;
            set => Set(ref _k, value);
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
            set => Set(ref _mode, value);
        }
        #endregion

    }
}
