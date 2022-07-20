using IDensity.AddClasses;

namespace IDensity.ViewModels
{
    public class MeasUnitVm:PropertyChangedBase
    {
        public MeasUnitVm(VM vM)
        {
            VM = vM;
            Init();
        }

        public VM VM { get; }

        void Init()
        {
            foreach (var sett in VM.mainModel.MeasUnitSettings)
            {
                sett.Writing += VM.CommService.SetMeasUnitsSettings;
            }
        }
    }
}
