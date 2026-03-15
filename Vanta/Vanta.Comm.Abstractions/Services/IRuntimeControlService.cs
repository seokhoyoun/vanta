namespace Vanta.Comm.Abstractions.Services
{
    public interface IRuntimeControlService
    {
        Task StartAllAsync(CancellationToken cancellationToken = default);

        Task StopAllAsync(CancellationToken cancellationToken = default);

        Task StartDeviceAsync(int deviceId, CancellationToken cancellationToken = default);

        Task StopDeviceAsync(int deviceId, CancellationToken cancellationToken = default);

        Task StartProcessAsync(int processId, CancellationToken cancellationToken = default);

        Task StopProcessAsync(int processId, CancellationToken cancellationToken = default);

        Task RefreshDeviceConfigurationAsync(int deviceId, CancellationToken cancellationToken = default);

        Task RefreshBlockConfigurationAsync(int blockSequence, CancellationToken cancellationToken = default);

        Task RefreshTagConfigurationAsync(int tagSequence, CancellationToken cancellationToken = default);

        Task RefreshProcessConfigurationAsync(int processId, CancellationToken cancellationToken = default);

        Task RefreshSequenceConfigurationAsync(int sequenceId, CancellationToken cancellationToken = default);
    }
}
