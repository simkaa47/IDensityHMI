using IDensity.AddClasses;
using IDensity.Models.SQL;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace IDensity.Models
{
    /// <summary>
    /// Набор статических методов для расчета к-тов полинома методои наименьших квадратов
    /// </summary>
    static class Calibration
    {        
        #region Степень полинома
        private static  int polDegree = 1;

        public static  int PolDegree
        {
            get { return polDegree; }
            set 
            {
                var temp = polDegree;
                if (value >= 1 && value <= 5)
                {
                    temp = value;
                }                
            }
        }
        #endregion

        #region Конструктор
        static Calibration()
        {
            CalculateExecEvent += (mes) => MessageBox.Show(mes);
        }
        #endregion
        

        #region Метод расчета калибровочной кривой
        public static List<double> GetCoeffs(List<Point> data)
        {           
            if (data == null || data.Count < 2)
            {
                CalculateExecEvent?.Invoke("Количество данных в таблице меньше 2!");
                return new List<double>();
            }
            var xyTable = new double[2, data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                xyTable[0, i] = data[i].X;
                xyTable[1, i] = data[i].Y;
            }
            var matrix = MakeSystem(xyTable, polDegree+1);// составляем СЛУ
            var resArr = GaussMethod(matrix);
            return new List<double>(resArr);
        }
        private static double[,] MakeSystem(double[,] xyTable, int basis)
        {
            double[,] matrix = new double[basis, basis + 1];
            for (int i = 0; i < basis; i++)
            {
                for (int j = 0; j < basis; j++)
                {
                    matrix[i, j] = 0;
                }
            }
            for (int i = 0; i < basis; i++)
            {
                for (int j = 0; j < basis; j++)
                {
                    double sumA = 0, sumB = 0;
                    for (int k = 0; k < xyTable.Length / 2; k++)
                    {
                        sumA += Math.Pow(xyTable[0, k], i) * Math.Pow(xyTable[0, k], j);
                        sumB += xyTable[1, k] * Math.Pow(xyTable[0, k], i);
                    }
                    matrix[i, j] = sumA;
                    matrix[i, basis] = sumB;
                }
            }
            return matrix;
        }
        static double[]  GaussMethod(double[,] matrix)
        {
            double s,d = 0;
            var sizeX = matrix.GetLength(0);
            var sizeY = matrix.GetLength(1);
            if ((sizeX < 1) || ((sizeY - sizeX) != 1)) return null;            
            double[] x = new double[sizeX];
            double[,] a = new double[sizeX, sizeX];
            double[] b = new double[sizeX];
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeX; j++)
                {
                    a[i, j] = matrix[i, j];
                    b[i] = matrix[i,sizeX];
                }

            }
            int n = sizeX;
            for (int k = 0; k < n; k++)
            {
                for (int j = k + 1; j < n; j++)
                {
                    d = a[j, k] / a[k, k];
                    for (int i = k; i < n; i++)
                    {
                        a[j, i] = a[j, i] - d * a[k, i];
                    }
                    b[j] = b[j] - d *b[k];
                }
            }

            for (int k = n - 1; k >= 0; k--)
            {
                d = 0;
                for (int j = k; j < n; j++)
                {
                    s = a[k, j] * x[j];
                    d += s;
                }
                x[k] = (b[k] - d) / a[k, k];
            }

            return x;
        }
        
        #endregion       

        #region Событие расчета коэффициентов
        public static event Action<string> CalculateExecEvent; 
        #endregion
    }
}
