﻿using IDensity.Models.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.AddClasses
{
    /// <summary>
    /// Универсальный класс перечислений
    /// </summary>
    public class EnumCustom : PropertyChangedBase, IDataBased
    {
        #region Id
        long _id;
        public long Id { get => _id; set => Set(ref _id, value); }
        #endregion

        #region Имя
        string _name = "";
         public string Name { get => _name; set => Set(ref _name, value); }

        #endregion
    }
}
