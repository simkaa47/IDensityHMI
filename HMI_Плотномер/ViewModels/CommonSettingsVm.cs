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
            Init();
        }

        /// <summary>
        /// Главная VM
        /// </summary>
        public VM VM { get; }

        void Init()
        {
            VM.mainModel.HalfLife.CommandEcecutedEvent += (o) => 
                VM.CommService.WriteCommonSettings($"half_life={VM.mainModel.HalfLife.WriteValue.ToStringPoint()}");
            VM.mainModel.DeviceName.CommandEcecutedEvent += (o) =>
                VM.CommService.WriteCommonSettings($"name={VM.mainModel.DeviceName.WriteValue.Substring(0,Math.Min(VM.mainModel.DeviceName.WriteValue.Length,10))}");
            VM.mainModel.IsotopName.CommandEcecutedEvent += (o) => 
                VM.CommService.WriteCommonSettings($"isotope={VM.mainModel.IsotopName.WriteValue.Substring(0, Math.Min(VM.mainModel.IsotopName.WriteValue.Length, 10))}");
            VM.mainModel.SourceInstallDate.CommandEcecutedEvent += (o) => 
                VM.CommService.WriteCommonSettings($"src_inst_date={VM.mainModel.SourceInstallDate.WriteValue.ToString("dd:MM:yy")}");
            VM.mainModel.SourceExpirationDate.CommandEcecutedEvent += (o) => VM.CommService.WriteCommonSettings($"src_exp_date={VM.mainModel.SourceExpirationDate.WriteValue.ToString("dd:MM:yy")}");}
        }
    }
