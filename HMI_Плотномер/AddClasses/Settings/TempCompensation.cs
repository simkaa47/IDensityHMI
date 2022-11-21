using IDensity.AddClasses;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace IDensity.Core.AddClasses.Settings
{
    [DataContract]
    public class TempCompensation:PropertyChangedBase
    {
        private int _index;
        [DataMember]
        public int Index
        {
            get=> _index;
            set => Set(ref _index, value);
        }


        #region Активность
        [DataMember]
        public Parameter<bool> Activity { get; set; } = new Parameter<bool>("TempCompensationActivity", "Актвность", false, true, 0, "");
        #endregion

        #region К-ты компннсации
        [DataMember]
        public List<Parameter<float>> Coeffs { get; set; } = Enumerable.Range(0, 2).Select(i => new Parameter<float>($"TempCompensationCOeff{i}", $"К-т компннсации {i}", float.MinValue, float.MaxValue, 0, "")).ToList();
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
