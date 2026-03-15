namespace Vanta.Comm.Abstractions.Services
{
    public interface ITagValueService
    {
        Task<string?> GetLinkedTagValueAsync(
            string tagName,
            bool direct = false,
            CancellationToken cancellationToken = default);

        Task<bool> SetLinkedTagValueAsync(
            string tagName,
            string tagValue,
            bool direct = true,
            CancellationToken cancellationToken = default);

        Task<bool> IsLinkedTagValueAsync(
            string tagName,
            string expectedValue,
            bool direct = false,
            CancellationToken cancellationToken = default);

        Task<string?> GetDefineTagValueAsync(
            int defineTagSequence,
            int index1 = -1,
            int index2 = -1,
            bool direct = false,
            CancellationToken cancellationToken = default);

        Task<bool> SetDefineTagValueAsync(
            int defineTagSequence,
            string tagValue,
            int index1 = -1,
            int index2 = -1,
            bool direct = true,
            CancellationToken cancellationToken = default);

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
