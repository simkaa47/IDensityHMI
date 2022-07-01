﻿using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Models
{
    /// <summary>
    /// Класс, хранящий в себе настройки связи с платой плотномера
    /// </summary>
    public class CommMode : PropertyChangedBase
    {
        #region RS485
        bool _rsEnable;
        public bool RsEnable { get => _rsEnable; 
            set => Set(ref _rsEnable, value); }
        #endregion

        #region Ethernet
        bool _ethEnable;
        public bool EthEnable { get => _ethEnable; set => Set(ref _ethEnable, value); }
        #endregion

        #region HART
        bool _hartEnable;
        public bool HartEnable { get => _hartEnable; set => Set(ref _hartEnable, value); }
        #endregion
    }
}
