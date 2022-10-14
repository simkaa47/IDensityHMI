using IDensity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static IDensity.AddClasses.EventHistory.EventDevice;

namespace IDensity.Core.Services.CheckServices.PrepareChecking
{
    class PrepareCheckService
    {
        private readonly PrepareCheck _prepareCheckResult;
        private readonly VM _vM;

        public PrepareCheckService(PrepareCheck prepareCheckResult, VM vM)
        {
            _prepareCheckResult = prepareCheckResult;
            _vM = vM;
        }

        public void Check()
        {
            var isCommonError = _vM.Events.EventDevices.Any(ed => ed.IsActive && ed.Type == (int)EventTypes.Error);
            _prepareCheckResult.CheckResult = !isCommonError;
            _prepareCheckResult.CheckSysytemErrors.IsError = isCommonError;            
        }
    }
}
