using System;
using System.Collections.Generic;
using System.Text;

namespace HMI_Плотномер.AddClasses
{
    /// <summary>
    /// Хранит информацию о том или ином параметре: адрес Modbus, описание
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class Parameter<T>: PropertyChangedBase
    {
        public Parameter(string description, string regType, int regNum)
        {
            this.Description = description;
            this.RegType = regType;
            this.RegNum = regNum;
        }
        #region Описание
        string _description = "";
        public string Description { get => _description; set => Set(ref _description, value); }
        #endregion

        #region Величина
        T _value;
        public T Value { get => _value; 
            set => Set(ref _value, value); }
        #endregion

        #region Тип регистра
        public string RegType { get; } = "holding";
        #endregion

        #region Адрес регистра
       public int RegNum { get; }
        #endregion

    }
}
