using System;

namespace Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Communication
{
    public sealed class MelsecMcProtocolException : InvalidOperationException
    {
        public MelsecMcProtocolException(string message)
            : base(message)
        {
        }
    }
}
