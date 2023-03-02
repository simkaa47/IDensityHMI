using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IDensity.AddClasses.Settings
{
    /// <summary>
    /// Определяет настройки плотности
    /// </summary>
    [DataContract]
    public class DensitySett:PropertyChangedBase
    {
        #region Команда записи
        
        private RelayCommand _outMeasNumWriteCommand;

        public RelayCommand OutMeasNumWriteCommand => _outMeasNumWriteCommand ?? (_outMeasNumWriteCommand = new RelayCommand(par => OnWriteExecuted(), o => true));
        #endregion
        #region Физическая величина
        /// <summary>
        /// Физическая величина
        /// </summary>
        [DataMember]
        public Parameter<float> PhysValue { get; set; } = new Parameter<float>("DensitySettValue", "Физическая величина", float.MinValue, float.MaxValue, 0, "");
        #endregion        
        
    }
}
