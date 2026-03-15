using System.Collections.Generic;
using Vanta.Comm.Abstractions.Simulation;
using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.Simulation.Scenarios
{
    public sealed class NoOpDeviceSimulationScenario : IDeviceSimulationScenario
    {
        public void OnConnected(DeviceDefinition device, IDeviceMemoryMap memoryMap)
        {
            _ = device;
            _ = memoryMap;
        }

        public void OnDisconnected(DeviceDefinition device, IDeviceMemoryMap memoryMap)
        {
            _ = device;
            _ = memoryMap;
        }

        public void OnBeforeRead(
            DeviceDefinition device,
            string memoryHead,
            int startAddress,
            int length,
            IDeviceMemoryMap memoryMap)
        {
            _ = device;
            _ = memoryHead;
            _ = startAddress;
            _ = length;
            _ = memoryMap;
        }

        public void OnAfterWrite(
            DeviceDefinition device,
            string memoryHead,
            int startAddress,
            IReadOnlyList<int> values,
            IDeviceMemoryMap memoryMap)
        {
            _ = device;
            _ = memoryHead;
            _ = startAddress;
            _ = values;
            _ = memoryMap;
        }
    }
}
