using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.ViewModels
{
    public class MeasProcessVm:PropertyChangedBase
    {
        public MeasProcessVm(VM vM)
        {
            VM = vM;
        }
        public VM VM { get; }


    }
}
