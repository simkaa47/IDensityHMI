using System;

namespace IDensity.Core.Models.Tcp
{
    class TcpWriteCommand
    {
        public TcpWriteCommand(Action<byte[]> action, byte[] parameter)
        {
            Action = action;
            Parameter = parameter;
        }
        public Action<byte[]> Action;
        public byte[] Parameter;
    }
}
