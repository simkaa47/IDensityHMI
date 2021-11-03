using System;
using System.Collections.Generic;
using System.Text;

namespace HMI_Плотномер.AddClasses
{
    class TcpWriteCommand
    {
        public TcpWriteCommand(Action<byte[]> action, byte[] parameter)
        {
            this.Action = action;
            this.Parameter = parameter;
        }
        public Action<byte[]> Action;
        public byte[] Parameter;
    }
}
