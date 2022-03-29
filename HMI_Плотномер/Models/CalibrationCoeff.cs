using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Models
{
    internal class CalibrationCoeff 
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
