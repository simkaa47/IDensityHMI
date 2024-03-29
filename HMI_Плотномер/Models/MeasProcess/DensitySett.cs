﻿using IDensity.AddClasses;
using IDensity.Core.Models.Parameters;
using IDensity.DataAccess;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IDensity.Core.Models.MeasProcess
{
    /// <summary>
    /// Определяет настройки плотности
    /// </summary>
    [DataContract]
    public class DensitySett : PropertyChangedBase
    {
        #region Физическая величина
        /// <summary>
        /// Физическая величина
        /// </summary>
        [DataMember]
        public Parameter<float> PhysValue { get; set; } = new Parameter<float>("DensitySettValue", "Физическая величина", float.MinValue, float.MaxValue, 0, "");
        #endregion

    }
}
