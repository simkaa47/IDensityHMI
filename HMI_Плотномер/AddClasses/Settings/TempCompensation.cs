using IDensity.AddClasses;
using System.Collections.Generic;
using System.Linq;

namespace IDensity.Core.AddClasses.Settings
{
    public class TempCompensation:PropertyChangedBase
    {
        private int _index;
        public int Index
        {
            get=> _index;
            set => Set(ref _index, value);
        }


        #region Активность
        public Parameter<bool> Activity { get; } = new Parameter<bool>("TempCompensationActivity", "Актвность", false, true, 0, "");
        #endregion

        #region К-ты компннсации
        public List<Parameter<float>> Coeffs { get; } = Enumerable.Range(0, 2).Select(i => new Parameter<float>($"TempCompensationCOeff{i}", $"К-т компннсации {i}", float.MinValue, float.MaxValue, 0, "")).ToList();
        #endregion

        public string Copy()
        {
            var arg = $"{Index},{(Activity.Value?1:0)},0";
            foreach (var coeff in Coeffs)
            {
                arg += "," + coeff.Value.ToStringPoint();
            }
            return arg;
        }

    }
}
