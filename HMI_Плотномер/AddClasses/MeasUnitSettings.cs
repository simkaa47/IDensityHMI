using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    /// <summary>
    /// Класс настроек едениц измерения
    /// </summary>
    class MeasUnitSettings
    {
        #region Номер еденицы измерения
        public Parameter<ushort> Id { get; set; } = new Parameter<ushort>("MeasUnitSettNum", "Номер еденицы измерения", 0, 10, 0, "");
        #endregion

        #region Номер еденицы измерения по классификации
        public Parameter<ushort> MeasUnitClassNum { get; set; } = new Parameter<ushort>("MeasUnitClassNum", "Номер еденицы измерения по классификации", 0, 10, 0, "");
        #endregion

        #region Ти ЕИ
        public Parameter<ushort> Type { get; set; } = new Parameter<ushort>("MeasUnitType", "Тип единицы измерения", 0, 10, 0, "");
        #endregion

        #region Коэффициент А
        /// <summary>
        /// Коэффициент А
        /// </summary>
        public Parameter<float> A { get; set; } = new Parameter<float>("MeasUnitA", "Коэффициент A", float.NegativeInfinity, float.PositiveInfinity, 121, "hold");
        #endregion
        #region Коэффициент B
        /// <summary>
        /// Коэффициент B
        /// </summary>
        public Parameter<float> B { get; set; } = new Parameter<float>("MeasUnitB", "Коэффициент B", float.NegativeInfinity, float.PositiveInfinity, 123, "hold");
        #endregion

        #region Название ЕИ
        public Parameter<string> MeasUnitName { get; set; } = new Parameter<string>("MeasUnitName", "Название", "", "zzzzzzzzzzz", 0, "");
        #endregion
        
        #region Событие
        public event Action<MeasUnitSettings> Writing;
        #endregion

        #region Команды
        RelayCommand _setMeasUnitCommand;
        public RelayCommand SetMeasUnitCommand => _setMeasUnitCommand ?? (_setMeasUnitCommand = new RelayCommand(execPar => Writing?.Invoke(this), canExecPar => true));

        
        #endregion

    }
}
