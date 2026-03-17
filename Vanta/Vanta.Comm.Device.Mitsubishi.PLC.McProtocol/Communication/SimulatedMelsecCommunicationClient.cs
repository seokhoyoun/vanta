using System;
using Vanta.Comm.Contracts.Models;
using Vanta.Comm.Simulation.Profiles;
using Vanta.Comm.Simulation.Runtime;

namespace Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Communication
{
    public sealed class SimulatedMelsecCommunicationClient : IMelsecCommunicationClient
    {
        private readonly SimulatedDeviceMemoryMap _memoryMap;
        private DeviceDefinition? _device;

        public SimulatedMelsecCommunicationClient()
            : this(new DeviceSimulationProfile())
        {
        }

        public SimulatedMelsecCommunicationClient(DeviceSimulationProfile profile)
            : this(new SimulatedDeviceMemoryMap(profile))
        {
        }

        public SimulatedMelsecCommunicationClient(SimulatedDeviceMemoryMap memoryMap)
        {
            if (memoryMap == null)
            {
                throw new ArgumentNullException(nameof(memoryMap));
            }

            _memoryMap = memoryMap;
        }

        public Task OpenAsync(DeviceDefinition device, CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            _device = device;
            _memoryMap.Connect();
            _memoryMap.NotifyConnected(device);

            return Task.CompletedTask;
        }

        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            DeviceDefinition? device = _device;

            if (device != null && _memoryMap.IsConnected)
            {
                _memoryMap.NotifyDisconnected(device);
            }

            _memoryMap.Disconnect();
            _device = null;

            return Task.CompletedTask;
        }

        public Task<int[]> ReadAsync(
            string memoryHead,
            int startAddress,
            int length,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            DeviceDefinition device = GetRequiredDevice();
            _memoryMap.NotifyBeforeRead(device, memoryHead, startAddress, length);

            int[] values = _memoryMap.ReadWords(memoryHead, startAddress, length);
            return Task.FromResult(values);
        }

        public Task<bool> WriteAsync(
            string memoryHead,
            int startAddress,
            IReadOnlyList<int> values,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            DeviceDefinition device = GetRequiredDevice();
            _memoryMap.WriteWords(memoryHead, startAddress, values);
            _memoryMap.NotifyAfterWrite(device, memoryHead, startAddress, values);

            return Task.FromResult(true);
        }

        public SimulatedDeviceMemoryMap GetMemoryMap()
        {
            return _memoryMap;
        }

        private DeviceDefinition GetRequiredDevice()
        {
            if (_device == null)
            {
                throw new InvalidOperationException("Simulated MELSEC client is not opened.");
            }

            return _device;
        }
    }
}
