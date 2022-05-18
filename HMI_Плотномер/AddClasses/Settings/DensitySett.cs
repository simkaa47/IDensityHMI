using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses.Settings
{
    /// <summary>
    /// Определяет настройки плотности
    /// </summary>
    class DensitySett:PropertyChangedBase
    {
        #region ЕИ
        MeasUnitSettings _measUnit = new MeasUnitSettings();
        /// <summary>
        /// ЕИ
        /// </summary>
        public MeasUnitSettings MeasUnit
        {
            get => _measUnit;
            set => Set(ref _measUnit, value);
        }
        private RelayCommand _outMeasNumWriteCommand;

        public RelayCommand OutMeasNumWriteCommand => _outMeasNumWriteCommand ?? (_outMeasNumWriteCommand = new RelayCommand(par => OnWriteExecuted(), o => true));
        #endregion
        #region Физическая величина
        /// <summary>
        /// Физическая величина
        /// </summary>
        public Parameter<float> PhysValue { get; } = new Parameter<float>("DensitySettValue", "Физическая величина", float.MinValue, float.MaxValue, 0, "");
        #endregion

        public DensitySett()
        {
            //Подписка на изменение свойств            
            PhysValue.CommandEcecutedEvent += (o) => OnWriteExecuted();
        }
        void OnWriteExecuted()
        {
            NeedWriteEvent?.Invoke($"{MeasUnit.Id.Value},{PhysValue.WriteValue.ToStringPoint()}");
        }
        /// <summary>
        /// Необходимо записать настройки стандартизаций
        /// </summary>
        public event Action<string> NeedWriteEvent;

        public string Copy()
        {
            return $"{MeasUnit.Id.Value},{PhysValue.Value.ToStringPoint()}";
        }
    }
}
