using System;
using System.Collections.Generic;
using Vanta.Comm.Abstractions.Devices;
using Vanta.Comm.Contracts.Models;
using Vanta.Comm.Device.Mitsubishi.PLC.CCLinkIE.Constants;

namespace Vanta.Comm.Device.Mitsubishi.PLC.CCLinkIE
{
    public sealed class CCLinkIeDeviceDriver : IDeviceDriver
    {
        public string DriverKey => CCLinkIeDriverKeys.CCLinkIe;

        public Task InitializeAsync(DeviceDefinition device, CancellationToken cancellationToken = default)
        {
            _ = device;
            _ = cancellationToken;
            throw CreateNotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            throw CreateNotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            throw CreateNotImplementedException();
        }

        public Task ApplyBlockAsync(BlockDefinition block, CancellationToken cancellationToken = default)
        {
            _ = block;
            _ = cancellationToken;
            throw CreateNotImplementedException();
        }

        public Task RemoveBlockAsync(int blockSequence, CancellationToken cancellationToken = default)
        {
            _ = blockSequence;
            _ = cancellationToken;
            throw CreateNotImplementedException();
        }

        public Task ApplyTagAsync(TagDefinition tag, CancellationToken cancellationToken = default)
        {
            _ = tag;
            _ = cancellationToken;
            throw CreateNotImplementedException();
        }

        public Task RemoveTagAsync(int tagSequence, CancellationToken cancellationToken = default)
        {
            _ = tagSequence;
            _ = cancellationToken;
            throw CreateNotImplementedException();
        }

        public Task<string?> GetDirectTagValueAsync(TagDefinition tag, CancellationToken cancellationToken = default)
        {
            _ = tag;
            _ = cancellationToken;
            throw CreateNotImplementedException();
        }

        public Task<bool> SetDirectTagValueAsync(TagDefinition tag, string value, CancellationToken cancellationToken = default)
        {
            _ = tag;
            _ = value;
            _ = cancellationToken;
            throw CreateNotImplementedException();
        }

        public Task<int[]> ReadBlockMemoryAsync(string memoryHead, int startAddress, int length, CancellationToken cancellationToken = default)
        {
            _ = memoryHead;
            _ = startAddress;
            _ = length;
            _ = cancellationToken;
            throw CreateNotImplementedException();
        }

        public Task<bool> WriteBlockMemoryAsync(string memoryHead, int startAddress, IReadOnlyList<int> values, CancellationToken cancellationToken = default)
        {
            _ = memoryHead;
            _ = startAddress;
            _ = values;
            _ = cancellationToken;
            throw CreateNotImplementedException();
        }

        private static NotSupportedException CreateNotImplementedException()
        {
            return new NotSupportedException("CC-Link IE driver is scaffolded but not implemented yet.");
        }
    }
}
