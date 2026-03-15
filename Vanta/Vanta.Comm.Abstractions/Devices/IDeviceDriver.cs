using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.Abstractions.Devices
{
    public interface IDeviceDriver
    {
        string DriverKey { get; }

        Task InitializeAsync(DeviceDefinition device, CancellationToken cancellationToken = default);

        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);

        Task ApplyBlockAsync(BlockDefinition block, CancellationToken cancellationToken = default);

        Task RemoveBlockAsync(int blockSequence, CancellationToken cancellationToken = default);

        Task ApplyTagAsync(TagDefinition tag, CancellationToken cancellationToken = default);

        Task RemoveTagAsync(int tagSequence, CancellationToken cancellationToken = default);

        Task<string?> GetDirectTagValueAsync(TagDefinition tag, CancellationToken cancellationToken = default);

        Task<bool> SetDirectTagValueAsync(TagDefinition tag, string value, CancellationToken cancellationToken = default);

        Task<int[]> ReadBlockMemoryAsync(
            string memoryHead,
            int startAddress,
            int length,
            CancellationToken cancellationToken = default);

        Task<bool> WriteBlockMemoryAsync(
            string memoryHead,
            int startAddress,
            IReadOnlyList<int> values,
            CancellationToken cancellationToken = default);
    }
}
