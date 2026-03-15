using System.Collections.Generic;
using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.Infrastructure.Adapter.Configuration
{
    public sealed class AdapterConfigurationSnapshot
    {
        private readonly Dictionary<int, DeviceDefinition> _devicesById;
        private readonly Dictionary<int, BlockDefinition> _blocksBySequence;
        private readonly Dictionary<int, TagDefinition> _tagsBySequence;
        private readonly Dictionary<int, DefineTagDefinition> _defineTagsBySequence;
        private readonly Dictionary<int, CompositeTypeDefinition> _compositeTypesById;
        private readonly Dictionary<string, CompositeTypeDefinition> _compositeTypesByName;
        private readonly Dictionary<int, ProcessDefinition> _processesById;
        private readonly Dictionary<int, SequenceDefinition> _sequencesById;

        public AdapterConfigurationSnapshot(
            IEnumerable<DeviceDefinition>? devices = null,
            IEnumerable<BlockDefinition>? blocks = null,
            IEnumerable<TagDefinition>? tags = null,
            IEnumerable<DefineTagDefinition>? defineTags = null,
            IEnumerable<CompositeTypeDefinition>? compositeTypes = null,
            IEnumerable<ProcessDefinition>? processes = null,
            IEnumerable<SequenceDefinition>? sequences = null,
            IEnumerable<SignalTowerDefinition>? signalTowers = null,
            IEnumerable<EventKeyDefinition>? eventKeys = null,
            IEnumerable<TrendItemDefinition>? trendItems = null)
        {
            Devices = CreateList(devices);
            Blocks = CreateList(blocks);
            Tags = CreateList(tags);
            DefineTags = CreateList(defineTags);
            CompositeTypes = CreateList(compositeTypes);
            Processes = CreateList(processes);
            Sequences = CreateList(sequences);
            SignalTowers = CreateList(signalTowers);
            EventKeys = CreateList(eventKeys);
            TrendItems = CreateList(trendItems);

            _devicesById = new Dictionary<int, DeviceDefinition>();
            _blocksBySequence = new Dictionary<int, BlockDefinition>();
            _tagsBySequence = new Dictionary<int, TagDefinition>();
            _defineTagsBySequence = new Dictionary<int, DefineTagDefinition>();
            _compositeTypesById = new Dictionary<int, CompositeTypeDefinition>();
            _compositeTypesByName = new Dictionary<string, CompositeTypeDefinition>(StringComparer.OrdinalIgnoreCase);
            _processesById = new Dictionary<int, ProcessDefinition>();
            _sequencesById = new Dictionary<int, SequenceDefinition>();

            BuildDeviceIndex();
            BuildBlockIndex();
            BuildTagIndex();
            BuildDefineTagIndex();
            BuildCompositeTypeIndex();
            BuildProcessIndex();
            BuildSequenceIndex();
        }

        public IReadOnlyList<DeviceDefinition> Devices { get; }

        public IReadOnlyList<BlockDefinition> Blocks { get; }

        public IReadOnlyList<TagDefinition> Tags { get; }

        public IReadOnlyList<DefineTagDefinition> DefineTags { get; }

        public IReadOnlyList<CompositeTypeDefinition> CompositeTypes { get; }

        public IReadOnlyList<ProcessDefinition> Processes { get; }

        public IReadOnlyList<SequenceDefinition> Sequences { get; }

        public IReadOnlyList<SignalTowerDefinition> SignalTowers { get; }

        public IReadOnlyList<EventKeyDefinition> EventKeys { get; }

        public IReadOnlyList<TrendItemDefinition> TrendItems { get; }

        public DeviceDefinition? FindDevice(int deviceId)
        {
            DeviceDefinition? item;

            if (_devicesById.TryGetValue(deviceId, out item))
            {
                return item;
            }

            return null;
        }

        public BlockDefinition? FindBlock(int blockSequence)
        {
            BlockDefinition? item;

            if (_blocksBySequence.TryGetValue(blockSequence, out item))
            {
                return item;
            }

            return null;
        }

        public TagDefinition? FindTag(int tagSequence)
        {
            TagDefinition? item;

            if (_tagsBySequence.TryGetValue(tagSequence, out item))
            {
                return item;
            }

            return null;
        }

        public DefineTagDefinition? FindDefineTag(int defineTagSequence)
        {
            DefineTagDefinition? item;

            if (_defineTagsBySequence.TryGetValue(defineTagSequence, out item))
            {
                return item;
            }

            return null;
        }

        public CompositeTypeDefinition? FindCompositeType(int compositeTypeId)
        {
            CompositeTypeDefinition? item;

            if (_compositeTypesById.TryGetValue(compositeTypeId, out item))
            {
                return item;
            }

            return null;
        }

        public CompositeTypeDefinition? FindCompositeType(string compositeTypeName)
        {
            if (string.IsNullOrWhiteSpace(compositeTypeName))
            {
                return null;
            }

            CompositeTypeDefinition? item;

            if (_compositeTypesByName.TryGetValue(compositeTypeName, out item))
            {
                return item;
            }

            return null;
        }

        public ProcessDefinition? FindProcess(int processId)
        {
            ProcessDefinition? item;

            if (_processesById.TryGetValue(processId, out item))
            {
                return item;
            }

            return null;
        }

        public SequenceDefinition? FindSequence(int sequenceId)
        {
            SequenceDefinition? item;

            if (_sequencesById.TryGetValue(sequenceId, out item))
            {
                return item;
            }

            return null;
        }

        private static IReadOnlyList<T> CreateList<T>(IEnumerable<T>? items)
        {
            List<T> list = new List<T>();

            if (items == null)
            {
                return list;
            }

            foreach (T item in items)
            {
                list.Add(item);
            }

            return list;
        }

        private void BuildDeviceIndex()
        {
            foreach (DeviceDefinition item in Devices)
            {
                _devicesById[item.DeviceId] = item;
            }
        }

        private void BuildBlockIndex()
        {
            foreach (BlockDefinition item in Blocks)
            {
                _blocksBySequence[item.BlockSequence] = item;
            }
        }

        private void BuildTagIndex()
        {
            foreach (TagDefinition item in Tags)
            {
                _tagsBySequence[item.TagSequence] = item;
            }
        }

        private void BuildDefineTagIndex()
        {
            foreach (DefineTagDefinition item in DefineTags)
            {
                _defineTagsBySequence[item.DefineTagSequence] = item;
            }
        }

        private void BuildCompositeTypeIndex()
        {
            foreach (CompositeTypeDefinition item in CompositeTypes)
            {
                _compositeTypesById[item.CompositeTypeId] = item;

                if (!string.IsNullOrWhiteSpace(item.CompositeTypeName))
                {
                    _compositeTypesByName[item.CompositeTypeName] = item;
                }
            }
        }

        private void BuildProcessIndex()
        {
            foreach (ProcessDefinition item in Processes)
            {
                _processesById[item.ProcessId] = item;
            }
        }

        private void BuildSequenceIndex()
        {
            foreach (SequenceDefinition item in Sequences)
            {
                _sequencesById[item.SequenceId] = item;
            }
        }
    }
}
