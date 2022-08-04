using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Models
{
    public class CheckSum:PropertyChangedBase
    {
        #region КОнтрольная сумма
        /// <summary>
        /// КОнтрольная сумма
        /// </summary>
        private uint _value;
        /// <summary>
        /// КОнтрольная сумма
        /// </summary>
        public uint Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
        #endregion

    }
}
