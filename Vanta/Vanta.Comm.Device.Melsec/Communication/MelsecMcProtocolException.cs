using System;

namespace Vanta.Comm.Device.Melsec.Communication
{
    public sealed class MelsecMcProtocolException : InvalidOperationException
    {
        public MelsecMcProtocolException(string message)
            : base(message)
        {
        }
    }
}
