using System.Collections.Generic;
using Vanta.Comm.Abstractions.Simulation;
using Vanta.Comm.Simulation.Scenarios;

namespace Vanta.Comm.Simulation.Profiles
{
    public sealed class DeviceSimulationProfile
    {
        public IReadOnlyList<DeviceSimulationMemoryPreset> Presets { get; set; } =
            Array.Empty<DeviceSimulationMemoryPreset>();

        public IDeviceSimulationScenario Scenario { get; set; } = new NoOpDeviceSimulationScenario();
    }
}
