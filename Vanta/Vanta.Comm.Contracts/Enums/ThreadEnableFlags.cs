using System;

namespace Vanta.Comm.Contracts.Enums
{
    [Flags]
    public enum ThreadEnableFlags
    {
        None = 0,
        One = 1,
        Two = 2,
        Double = 3,
        Three = 4,
        Triple = 7,
        Four = 8,
        All = 15,
    }
}
