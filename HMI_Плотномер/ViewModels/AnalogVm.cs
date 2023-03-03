using IDensity.AddClasses;
using IDensity.Models;
using System.Collections.Generic;
using System.Linq;

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

        #region Аналоговые входы
        public List<AnalogInput> AnalogInputs => VM.mainModel.AnalogGroups.Select(g => g.AI).ToList();
        #endregion

        #region Аналоговые выходы
        public List<AnalogOutput> AnalogOutputs => VM.mainModel.AnalogGroups.Select(g => g.AO).ToList();
        #endregion
    }
}
