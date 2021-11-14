using HMI_Плотномер.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMI_Плотномер.Views.AddClasses
{
    class ParameterWithCommand
    {
        public string Description { get; set; } = "Описание параметра";
        public object WriteValue { get; set; }
        public object Command { get; set; }
        public object ReadValue { get; set; }

    }
}
