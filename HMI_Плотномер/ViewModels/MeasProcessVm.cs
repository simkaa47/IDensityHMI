using IDensity.AddClasses;
using IDensity.AddClasses.Settings;
using IDensity.ViewModels.Commands;
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
            Describe();
        }
        public VM VM { get; }

        #region Выбраннный процесс
        /// <summary>
        /// Выбраннный процесс
        /// </summary>
        private MeasProcSettings _selectedProcess;
        /// <summary>
        /// Выбраннный процесс
        /// </summary>
        public MeasProcSettings SelectedProcess
        {
            get => _selectedProcess;
            set => Set(ref _selectedProcess, value);
        }
        #endregion

        void Describe()
        {
            foreach (var mp in VM.mainModel.MeasProcSettings)
            {
                mp.NeedWriteEvent += VM.CommService.WriteMeasProcSettings;
                mp.IsActive.CommandEcecutedEvent += (s) => VM.CommService.SetMeasProcActivity();
                mp.NeedMakeStand += VM.CommService.MakeStand;
                mp.StandFinishEvent += (num) => VM.CommService.Tcp.GetMeasSettingsExternal(num);
                mp.NeedMakeSingleMeasEvent += VM.CommService.MakeSingleMeasure;
                mp.SingleMeasEventFinishedEvent += (num) => VM.CommService.Tcp.GetMeasSettingsExternal(num);
            }
        }





    }
}
