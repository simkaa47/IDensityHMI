using IDensity.AddClasses;

namespace IDensity.Core.AddClasses.Settings
{
    public class TempRecalculateCoeffs : PropertyChangedBase
    {

        public TempRecalculateCoeffs(int modNum)
        {
            ModNum = modNum;
        }
        #region Номер модлуя
        private int _modNum;
        public int ModNum
        {
            get => _modNum;
            set => Set(ref _modNum, value);
        }
        #endregion

        public Parameter<float> A { get; } = new Parameter<float>("TempRecalculateA", "К-т A", float.MinValue, float.MaxValue, 0, "");
        public Parameter<float> B { get; } = new Parameter<float>("TempRecalculateB", "К-т B", float.MinValue, float.MaxValue, 0, "");
    }
}
