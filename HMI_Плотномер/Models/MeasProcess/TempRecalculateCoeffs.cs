using IDensity.AddClasses;
using IDensity.Core.Models.Parameters;
using IDensity.DataAccess;
using System.Runtime.Serialization;

namespace IDensity.Core.Models.MeasProcess
{
    [DataContract]
    public class TempRecalculateCoeffs : PropertyChangedBase
    {

        public TempRecalculateCoeffs(int modNum)
        {
            ModNum = modNum;
        }
        #region Номер модлуя
        private int _modNum;
        [DataMember]
        public int ModNum
        {
            get => _modNum;
            set => Set(ref _modNum, value);
        }
        #endregion
        [DataMember]
        public Parameter<float> A { get; set; } = new Parameter<float>("TempRecalculateA", "К-т A", float.MinValue, float.MaxValue, 0, "");
        [DataMember]
        public Parameter<float> B { get; set; } = new Parameter<float>("TempRecalculateB", "К-т B", float.MinValue, float.MaxValue, 0, "");
    }
}
