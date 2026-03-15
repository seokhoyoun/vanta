using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Vanta.Comm.Infrastructure.Adapter.Memory
{
    public sealed class InMemoryAdapterTagStore : IAdapterTagStore
    {
        private readonly ConcurrentDictionary<string, string> _tagValues =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private readonly ConcurrentDictionary<string, int[]> _blockValues =
            new ConcurrentDictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);

        public Task<string?> GetTagValueAsync(string tagName, bool direct = false, CancellationToken cancellationToken = default)
        {
            string key = BuildTagKey(tagName, direct);
            string? value;

            _tagValues.TryGetValue(key, out value);

            return Task.FromResult<string?>(value);
        }

        public Task<bool> SetTagValueAsync(string tagName, string tagValue, bool direct = true, CancellationToken cancellationToken = default)
        {
            string key = BuildTagKey(tagName, direct);
            _tagValues[key] = tagValue;

            return Task.FromResult(true);
        }

        public Task<int[]> GetBlockMemoryAsync(
            string commandKey,
            string memoryHead,
            int startAddress,
            int length,
            CancellationToken cancellationToken = default)
        {
            string key = BuildBlockKey(commandKey, memoryHead, startAddress);
            int[]? existing;

            if (_blockValues.TryGetValue(key, out existing))
            {
                int[] buffer = new int[length];
                Array.Copy(existing, buffer, Math.Min(existing.Length, length));
                return Task.FromResult(buffer);
            }

            return Task.FromResult(new int[length]);
        }

        public Task<bool> SetBlockMemoryAsync(
            string commandKey,
            string memoryHead,
            int startAddress,
            IReadOnlyList<int> values,
            CancellationToken cancellationToken = default)
        {
            int[] buffer = new int[values.Count];
            int index;

            for (index = 0; index < values.Count; index++)
            {
                buffer[index] = values[index];
            }

            string key = BuildBlockKey(commandKey, memoryHead, startAddress);
            _blockValues[key] = buffer;

            return Task.FromResult(true);
        }

        private static string BuildTagKey(string tagName, bool direct)
        {
            string prefix = direct ? "direct" : "linked";
            return string.Concat(prefix, "::", tagName);
        }

        private static string BuildBlockKey(string commandKey, string memoryHead, int startAddress)
        {
            return string.Concat(commandKey, "::", memoryHead, "::", startAddress.ToString());
        }
    }
}
