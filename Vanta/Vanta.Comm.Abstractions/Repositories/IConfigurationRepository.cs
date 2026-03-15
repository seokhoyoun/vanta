using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.Abstractions.Repositories
{
    public interface IConfigurationRepository
    {
        Task<IReadOnlyList<DeviceDefinition>> GetDevicesAsync(CancellationToken cancellationToken = default);

        Task<DeviceDefinition?> GetDeviceAsync(int deviceId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<BlockDefinition>> GetBlocksAsync(CancellationToken cancellationToken = default);

        Task<BlockDefinition?> GetBlockAsync(int blockSequence, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TagDefinition>> GetTagsAsync(CancellationToken cancellationToken = default);

        Task<TagDefinition?> GetTagAsync(int tagSequence, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<DefineTagDefinition>> GetDefineTagsAsync(CancellationToken cancellationToken = default);

        Task<DefineTagDefinition?> GetDefineTagAsync(int defineTagSequence, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<CompositeTypeDefinition>> GetCompositeTypesAsync(CancellationToken cancellationToken = default);

        Task<CompositeTypeDefinition?> GetCompositeTypeAsync(int compositeTypeId, CancellationToken cancellationToken = default);

        Task<CompositeTypeDefinition?> GetCompositeTypeAsync(string compositeTypeName, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ProcessDefinition>> GetProcessesAsync(CancellationToken cancellationToken = default);

        Task<ProcessDefinition?> GetProcessAsync(int processId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<SequenceDefinition>> GetSequencesAsync(CancellationToken cancellationToken = default);

        Task<SequenceDefinition?> GetSequenceAsync(int sequenceId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<SignalTowerDefinition>> GetSignalTowersAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EventKeyDefinition>> GetEventKeysAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TrendItemDefinition>> GetTrendItemsAsync(CancellationToken cancellationToken = default);
    }
}
