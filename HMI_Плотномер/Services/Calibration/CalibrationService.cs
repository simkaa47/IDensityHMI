using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace IDensity.Services.Calibration
{
    public  class CalibrationService
    {
        #region Степень полинома
        private static int polDegree = 1;

        public static int PolDegree
        {
            get { return polDegree; }
            set
            {
                var temp = polDegree;
                if (value >= 1 && value <= 5)
                {
                    polDegree = value;
                }
            }
        }
        #endregion

        static CalibrationService()
        {
            CalculateExecEvent += (s) => MessageBox.Show(s);
        }

        #region Событие расчета коэффициентов
        public static event Action<string> CalculateExecEvent;
        #endregion

        public static List<double> GetCoeffs(List<Point> data)
        {
            if (data == null || data.Count < 2)
            {
                CalculateExecEvent?.Invoke("Количество данных в таблице меньше 2!");
                return new List<double>();
            }
            var className = "Calibration";
            var asmName = "VeryImportantAlgortim.dll";
            Assembly asm = Assembly.LoadFrom(asmName);
            var points = data.Select(p=>(p.X,p.Y)).ToList();

            Type calibration = asm.GetTypes().Where(t => t.Name == className).FirstOrDefault();
            if (calibration != null)
            {

            }
            else throw new Exception($"Не удалось найти или некорректен файл {asmName}.dll");
            MethodInfo getCoeffs = calibration.GetMethod("GetCoeffs", BindingFlags.Public | BindingFlags.Static);
            object result = getCoeffs?.Invoke(null, new object[] { points, PolDegree });
            if (result is List<double> list) return list;
            else throw new Exception($"Не удалось найти или некорректен файл {asmName}.dll");
        }
    }
}
