using IDensity.AddClasses;

namespace IDensity.ViewModels
{
    public class AnalogVm:PropertyChangedBase
    {
        public AnalogVm(VM vM)
        {
            VM = vM;
            Init();

        }

        public VM VM { get; }

        void Init()
        {
            var commService = VM.CommService;
            foreach (var gr in VM.mainModel.AnalogGroups)
            {
                gr.AI.SwitchPwrEvent += commService.SwitchAm;
                gr.AI.ChangeSettCallEvent += commService.ChangeAdcAct;
                gr.AO.SwitchPwrEvent += commService.SwitchAm;
                gr.AO.SetTestValueCallEvent += commService.SetTestValueAm;
                gr.AO.ChangeSettCallEvent += commService.ChangeDacAct;
            }
        }
    }
}
