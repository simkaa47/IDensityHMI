namespace IDensity.Core.Models.MeasProcess
{
    public class CalibrationCoeff
    {
        public CalibrationCoeff(int deg, double coeff)
        {
            Deg = deg;
            Coeff = coeff;
        }
        /// <summary>
        /// Степень к-та
        /// </summary>
        public int Deg { get; set; }
        /// <summary>
        /// Коэффициент
        /// </summary>
        public double Coeff { get; set; }
    }
}
