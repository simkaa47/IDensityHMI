using IDensity.AddClasses;
using IDensity.Core.Models.Parameters;
using IDensity.DataAccess;

namespace IDensity.Core.Models.Telemetry
{
    public class TempBoardTelemetry : PropertyChangedBase
    {
        #region Температура с внешнего датчика, С
        public Parameter<float> TempInternal { get; } = new Parameter<float>("TempInternal", "Температура с внешнего датчика, С", 0, float.PositiveInfinity, 8, "read")
        {
            OnlyRead = true
        };
        #endregion

        #region Температура с внутреннего датчика, С
        public Parameter<float> TempExternal { get; } = new Parameter<float>("TempExternal", "Температура с внутреннего датчика, С", 0, float.PositiveInfinity, 9, "read")
        {
            OnlyRead = true
        };
        #endregion

        #region Статус связи с платой питания
        public Parameter<bool> TempBoardCommState { get; } = new Parameter<bool>("TempBoardCommState", "Статус связи с платой питания", false, true, 0, "");
        #endregion

        #region OverTemp
        /// <summary>
        /// OverTemp
        /// </summary>
        private bool _overTemp;
        /// <summary>
        /// OverTemp
        /// </summary>
        public bool OverTemp
        {
            get => _overTemp;
            set => Set(ref _overTemp, value);
        }
        #endregion

        public TempBoardTelemetry()
        {
            TempInternal.PropertyChanged += (obj, args) =>
            {
                OverTemp = TempInternal.Value >= 80 || TempInternal.Value <= -40;
            };
        }

    }
}
