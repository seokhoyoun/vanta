namespace Vanta.Comm.Infrastructure.Adapter.Memory
{
    public interface IAdapterTagStore
    {
        Task<string?> GetTagValueAsync(string tagName, bool direct = false, CancellationToken cancellationToken = default);

        Task<bool> SetTagValueAsync(string tagName, string tagValue, bool direct = true, CancellationToken cancellationToken = default);

        Task<int[]> GetBlockMemoryAsync(
            string commandKey,
            string memoryHead,
            int startAddress,
            int length,
            CancellationToken cancellationToken = default);

        Task<bool> SetBlockMemoryAsync(
            string commandKey,
            string memoryHead,
            int startAddress,
            IReadOnlyList<int> values,
            CancellationToken cancellationToken = default);
    }
}
