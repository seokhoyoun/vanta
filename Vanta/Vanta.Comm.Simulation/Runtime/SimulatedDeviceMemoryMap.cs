using System;
using System.Collections.Generic;
using Vanta.Comm.Abstractions.Simulation;
using Vanta.Comm.Contracts.Models;
using Vanta.Comm.Simulation.Profiles;
using Vanta.Comm.Simulation.Scenarios;

namespace Vanta.Comm.Simulation.Runtime
{
    public sealed class SimulatedDeviceMemoryMap : IDeviceMemoryMap
    {
        private readonly object _syncRoot = new object();
        private readonly Dictionary<string, Dictionary<int, int>> _wordMemoryByHead =
            new Dictionary<string, Dictionary<int, int>>(StringComparer.OrdinalIgnoreCase);

        private readonly IDeviceSimulationScenario _scenario;

        private bool _isConnected;

        public SimulatedDeviceMemoryMap()
            : this(new DeviceSimulationProfile())
        {
        }

        public SimulatedDeviceMemoryMap(DeviceSimulationProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (profile.Scenario == null)
            {
                _scenario = new NoOpDeviceSimulationScenario();
            }
            else
            {
                _scenario = profile.Scenario;
            }

            LoadPresets(profile.Presets);
        }

        public bool IsConnected
        {
            get
            {
                lock (_syncRoot)
                {
                    return _isConnected;
                }
            }
        }

        public void Connect()
        {
            lock (_syncRoot)
            {
                _isConnected = true;
            }
        }

        public void Disconnect()
        {
            lock (_syncRoot)
            {
                _isConnected = false;
            }
        }

        public int[] ReadWords(string memoryHead, int startAddress, int length)
        {
            EnsureConnected();

            int[] values = new int[length];
            Dictionary<int, int> headMemory = GetOrCreateHeadMemory(memoryHead);
            int index;

            lock (_syncRoot)
            {
                for (index = 0; index < length; index++)
                {
                    int address = startAddress + index;
                    int value;

                    if (headMemory.TryGetValue(address, out value))
                    {
                        values[index] = value;
                    }
                    else
                    {
                        values[index] = 0;
                    }
                }
            }

            return values;
        }

        public void WriteWords(string memoryHead, int startAddress, IReadOnlyList<int> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            EnsureConnected();

            Dictionary<int, int> headMemory = GetOrCreateHeadMemory(memoryHead);
            int index;

            lock (_syncRoot)
            {
                for (index = 0; index < values.Count; index++)
                {
                    int address = startAddress + index;
                    headMemory[address] = values[index];
                }
            }
        }

        public int ReadBit(string memoryHead, int startAddress, int bitOffset)
        {
            int[] words = ReadWords(memoryHead, startAddress, 1);
            int wordValue = 0;

            if (words.Length > 0)
            {
                wordValue = words[0];
            }

            if (bitOffset < 0)
            {
                return 0;
            }

            return (wordValue >> bitOffset) & 0x01;
        }

        public void WriteBit(string memoryHead, int startAddress, int bitOffset, int value)
        {
            int[] words = ReadWords(memoryHead, startAddress, 1);
            int wordValue = 0;

            if (words.Length > 0)
            {
                wordValue = words[0];
            }

            if (bitOffset >= 0)
            {
                if (value == 0)
                {
                    wordValue = wordValue & ~(1 << bitOffset);
                }
                else
                {
                    wordValue = wordValue | (1 << bitOffset);
                }
            }

            int[] nextValues = new int[1];
            nextValues[0] = wordValue;
            WriteWords(memoryHead, startAddress, nextValues);
        }

        public void LoadPreset(string memoryHead, int startAddress, IReadOnlyList<int> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            Dictionary<int, int> headMemory = GetOrCreateHeadMemory(memoryHead);
            int index;

            lock (_syncRoot)
            {
                for (index = 0; index < values.Count; index++)
                {
                    int address = startAddress + index;
                    headMemory[address] = values[index];
                }
            }
        }

        public void NotifyConnected(DeviceDefinition device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            _scenario.OnConnected(device, this);
        }

        public void NotifyDisconnected(DeviceDefinition device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            _scenario.OnDisconnected(device, this);
        }

        public void NotifyBeforeRead(DeviceDefinition device, string memoryHead, int startAddress, int length)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            _scenario.OnBeforeRead(device, memoryHead, startAddress, length, this);
        }

        public void NotifyAfterWrite(DeviceDefinition device, string memoryHead, int startAddress, IReadOnlyList<int> values)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            _scenario.OnAfterWrite(device, memoryHead, startAddress, values, this);
        }

        private void LoadPresets(IReadOnlyList<DeviceSimulationMemoryPreset> presets)
        {
            if (presets == null)
            {
                return;
            }

            int index;
            for (index = 0; index < presets.Count; index++)
            {
                DeviceSimulationMemoryPreset preset = presets[index];

                if (preset == null)
                {
                    continue;
                }

                LoadPreset(preset.MemoryHead, preset.StartAddress, preset.Values);
            }
        }

        private Dictionary<int, int> GetOrCreateHeadMemory(string memoryHead)
        {
            if (string.IsNullOrWhiteSpace(memoryHead))
            {
                throw new InvalidOperationException("Simulation memory head is required.");
            }

            lock (_syncRoot)
            {
                Dictionary<int, int>? headMemory;

                if (_wordMemoryByHead.TryGetValue(memoryHead, out headMemory))
                {
                    return headMemory;
                }

                headMemory = new Dictionary<int, int>();
                _wordMemoryByHead[memoryHead] = headMemory;
                return headMemory;
            }
        }

        private void EnsureConnected()
        {
            lock (_syncRoot)
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Simulation memory map is not connected.");
                }
            }
        }
    }
}
