using System.Collections.Concurrent;
using Vanta.Comm.Abstractions.Services;

namespace Vanta.Comm.Infrastructure.Adapter.Memory
{
    public sealed class InMemorySharedMemoryStore : ISharedMemoryStore
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, string>> _stringMemory =
            new ConcurrentDictionary<string, ConcurrentDictionary<int, string>>(StringComparer.OrdinalIgnoreCase);

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, int>> _intMemory =
            new ConcurrentDictionary<string, ConcurrentDictionary<int, int>>(StringComparer.OrdinalIgnoreCase);

        public bool TryGetString(string memoryName, int index, out string? value)
        {
            value = null;

            ConcurrentDictionary<int, string>? memory;
            string? stored;

            if (_stringMemory.TryGetValue(memoryName, out memory) &&
                memory.TryGetValue(index, out stored))
            {
                value = stored;
                return true;
            }

            return false;
        }

        public void SetString(string memoryName, int index, string value)
        {
            ConcurrentDictionary<int, string> memory = GetOrCreateStringMemory(memoryName);
            memory[index] = value;
        }

        public bool TryGetInt32(string memoryName, int index, out int value)
        {
            value = default(int);

            ConcurrentDictionary<int, int>? memory;
            if (_intMemory.TryGetValue(memoryName, out memory))
            {
                return memory.TryGetValue(index, out value);
            }

            return false;
        }

        public void SetInt32(string memoryName, int index, int value)
        {
            ConcurrentDictionary<int, int> memory = GetOrCreateIntMemory(memoryName);
            memory[index] = value;
        }

        private ConcurrentDictionary<int, string> GetOrCreateStringMemory(string memoryName)
        {
            ConcurrentDictionary<int, string>? memory;

            if (_stringMemory.TryGetValue(memoryName, out memory))
            {
                return memory;
            }

            memory = new ConcurrentDictionary<int, string>();
            _stringMemory[memoryName] = memory;

            return memory;
        }

        private ConcurrentDictionary<int, int> GetOrCreateIntMemory(string memoryName)
        {
            ConcurrentDictionary<int, int>? memory;

            if (_intMemory.TryGetValue(memoryName, out memory))
            {
                return memory;
            }

            memory = new ConcurrentDictionary<int, int>();
            _intMemory[memoryName] = memory;

            return memory;
        }
    }
}
