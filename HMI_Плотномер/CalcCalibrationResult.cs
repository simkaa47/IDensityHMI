using IDensity.DataAccess;

namespace IDensity
{
    class CalcCalibrationResult : PropertyChangedBase
    {
        #region Степень полинома
        private int _deegry;

        public int Deegry
        {
            get { return _deegry; }
            set { Set(ref _deegry, value); }
        }
        #endregion

        #region К-т
        private double _coeff;

        public double Coeff
        {
            get { return _coeff; }
            set { Set(ref _coeff, value); }
        }

        #endregion

        public CalcCalibrationResult(int deg, double coeff)
        {
            this.Deegry = deg;
            this.Coeff = coeff;
        }

    }
}
