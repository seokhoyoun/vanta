using System.Threading;
using Vanta.Comm.Abstractions.Repositories;
using Vanta.Comm.Contracts.Models;
using Vanta.Comm.Infrastructure.Adapter.Configuration;

namespace Vanta.Comm.Infrastructure.Adapter.Repositories
{
    public sealed class AdapterConfigurationRepository : IConfigurationRepository
    {
        private readonly IAdapterConfigurationSource _configurationSource;
        private readonly SemaphoreSlim _sync = new SemaphoreSlim(1, 1);
        private AdapterConfigurationSnapshot? _snapshot;

        public AdapterConfigurationRepository(IAdapterConfigurationSource configurationSource)
        {
            _configurationSource = configurationSource;
        }

        public async Task<IReadOnlyList<DeviceDefinition>> GetDevicesAsync(CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.Devices;
        }

        public async Task<DeviceDefinition?> GetDeviceAsync(int deviceId, CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.FindDevice(deviceId);
        }

        public async Task<IReadOnlyList<BlockDefinition>> GetBlocksAsync(CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.Blocks;
        }

        public async Task<BlockDefinition?> GetBlockAsync(int blockSequence, CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.FindBlock(blockSequence);
        }

        public async Task<IReadOnlyList<TagDefinition>> GetTagsAsync(CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.Tags;
        }

        public async Task<TagDefinition?> GetTagAsync(int tagSequence, CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.FindTag(tagSequence);
        }

        public async Task<IReadOnlyList<DefineTagDefinition>> GetDefineTagsAsync(CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.DefineTags;
        }

        public async Task<DefineTagDefinition?> GetDefineTagAsync(int defineTagSequence, CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.FindDefineTag(defineTagSequence);
        }

        public async Task<IReadOnlyList<CompositeTypeDefinition>> GetCompositeTypesAsync(CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.CompositeTypes;
        }

        public async Task<CompositeTypeDefinition?> GetCompositeTypeAsync(int compositeTypeId, CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.FindCompositeType(compositeTypeId);
        }

        public async Task<CompositeTypeDefinition?> GetCompositeTypeAsync(string compositeTypeName, CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.FindCompositeType(compositeTypeName);
        }

        public async Task<IReadOnlyList<ProcessDefinition>> GetProcessesAsync(CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.Processes;
        }

        public async Task<ProcessDefinition?> GetProcessAsync(int processId, CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.FindProcess(processId);
        }

        public async Task<IReadOnlyList<SequenceDefinition>> GetSequencesAsync(CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.Sequences;
        }

        public async Task<SequenceDefinition?> GetSequenceAsync(int sequenceId, CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.FindSequence(sequenceId);
        }

        public async Task<IReadOnlyList<SignalTowerDefinition>> GetSignalTowersAsync(CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.SignalTowers;
        }

        public async Task<IReadOnlyList<EventKeyDefinition>> GetEventKeysAsync(CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.EventKeys;
        }

        public async Task<IReadOnlyList<TrendItemDefinition>> GetTrendItemsAsync(CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = await GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
            return snapshot.TrendItems;
        }

        public async Task RefreshAsync(CancellationToken cancellationToken = default)
        {
            await _sync.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                _snapshot = await _configurationSource.LoadAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _sync.Release();
            }
        }

        private async Task<AdapterConfigurationSnapshot> GetSnapshotAsync(CancellationToken cancellationToken)
        {
            if (_snapshot != null)
            {
                return _snapshot;
            }

            await RefreshAsync(cancellationToken).ConfigureAwait(false);

            if (_snapshot != null)
            {
                return _snapshot;
            }

            return new AdapterConfigurationSnapshot();
        }
    }
}
