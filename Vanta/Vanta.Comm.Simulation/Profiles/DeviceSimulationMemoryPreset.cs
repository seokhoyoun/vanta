using System.Collections.Generic;

namespace Vanta.Comm.Simulation.Profiles
{
    public sealed class DeviceSimulationMemoryPreset
    {
        public string MemoryHead { get; set; } = string.Empty;

        public int StartAddress { get; set; }

        public IReadOnlyList<int> Values { get; set; } = Array.Empty<int>();
    }
}
