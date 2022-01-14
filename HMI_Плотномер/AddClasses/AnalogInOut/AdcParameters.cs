using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    class AdcParameters
    {
        #region Связь с АЦП
        public Parameter<bool> CommState { get; } = new Parameter<bool>("AdcCommState", "Состояние связи с платой АЦП", true, false, 0, "")
        {
            OnlyRead = true
        }; 
        #endregion
    }
}
