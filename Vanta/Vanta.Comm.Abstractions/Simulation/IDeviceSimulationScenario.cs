using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.Abstractions.Simulation
{
    public interface IDeviceSimulationScenario
    {
        void OnConnected(DeviceDefinition device, IDeviceMemoryMap memoryMap);

        void OnDisconnected(DeviceDefinition device, IDeviceMemoryMap memoryMap);

        void OnBeforeRead(
            DeviceDefinition device,
            string memoryHead,
            int startAddress,
            int length,
            IDeviceMemoryMap memoryMap);

        void OnAfterWrite(
            DeviceDefinition device,
            string memoryHead,
            int startAddress,
            IReadOnlyList<int> values,
            IDeviceMemoryMap memoryMap);
    }
}
