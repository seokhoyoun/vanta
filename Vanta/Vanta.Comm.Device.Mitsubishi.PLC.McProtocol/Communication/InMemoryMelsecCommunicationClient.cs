using System;
using System.Collections.Generic;
using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Communication
{
    public sealed class InMemoryMelsecCommunicationClient : IMelsecCommunicationClient
    {
        private readonly object _syncRoot = new object();
        private readonly Dictionary<string, int[]> _memoryByKey =
            new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);

        private bool _isOpened;

        public Task OpenAsync(DeviceDefinition device, CancellationToken cancellationToken = default)
        {
            _ = device;

            lock (_syncRoot)
            {
                _isOpened = true;
            }

            return Task.CompletedTask;
        }

        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            lock (_syncRoot)
            {
                _isOpened = false;
            }

            return Task.CompletedTask;
        }

        public Task<int[]> ReadAsync(
            string memoryHead,
            int startAddress,
            int length,
            CancellationToken cancellationToken = default)
        {
            lock (_syncRoot)
            {
                EnsureOpened();

                string key = BuildMemoryKey(memoryHead, startAddress);
                int[]? stored;

                if (_memoryByKey.TryGetValue(key, out stored))
                {
                    int[] copy = new int[length];
                    Array.Copy(stored, copy, Math.Min(stored.Length, length));
                    return Task.FromResult(copy);
                }

                return Task.FromResult(new int[length]);
            }
        }

        public Task<bool> WriteAsync(
            string memoryHead,
            int startAddress,
            IReadOnlyList<int> values,
            CancellationToken cancellationToken = default)
        {
            lock (_syncRoot)
            {
                EnsureOpened();

                int[] copy = new int[values.Count];
                int index;

                for (index = 0; index < values.Count; index++)
                {
                    copy[index] = values[index];
                }

                string key = BuildMemoryKey(memoryHead, startAddress);
                _memoryByKey[key] = copy;
            }

            return Task.FromResult(true);
        }

        private void EnsureOpened()
        {
            if (!_isOpened)
            {
                throw new InvalidOperationException("MELSEC communication client is not opened.");
            }
        }

        private static string BuildMemoryKey(string memoryHead, int startAddress)
        {
            return string.Concat(memoryHead, "::", startAddress.ToString());
        }
    }
}
