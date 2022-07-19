using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.ViewModels
{
    public class CommonSettingsVm:PropertyChangedBase
    {
        public CommonSettingsVm(VM vM)
        {
            VM = vM;
        }

        /// <summary>
        /// Главная VM
        /// </summary>
        public VM VM { get; }
    }
}
