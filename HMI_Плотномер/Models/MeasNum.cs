using IDensity.AddClasses;
using IDensity.Models.SQL;

namespace IDensity.Models
{
    public class MeasNum : PropertyChangedBase, IDataBased
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
        private double _coeff;
        /// <summary>
        /// К-т
        /// </summary>
        public double Coeff
        {
            get => _coeff;
            set => Set(ref _coeff, value);
        }
        #endregion

        #region Режим  работы
        /// <summary>
        /// Режим  работы
        /// </summary>
        private int _mode;
        /// <summary>
        /// Режим  работы
        /// </summary>
        public int Mode
        {
            get => _mode;
            set => Set(ref _mode, value);
        }
        #endregion


    }
}
