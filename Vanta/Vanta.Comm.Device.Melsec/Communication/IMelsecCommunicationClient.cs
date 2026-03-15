using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.Device.Melsec.Communication
{
    public interface IMelsecCommunicationClient
    {
        Task OpenAsync(DeviceDefinition device, CancellationToken cancellationToken = default);

        Task CloseAsync(CancellationToken cancellationToken = default);

        Task<int[]> ReadAsync(
            string memoryHead,
            int startAddress,
            int length,
            CancellationToken cancellationToken = default);

        Task<bool> WriteAsync(
            string memoryHead,
            int startAddress,
            IReadOnlyList<int> values,
            CancellationToken cancellationToken = default);
    }
}
