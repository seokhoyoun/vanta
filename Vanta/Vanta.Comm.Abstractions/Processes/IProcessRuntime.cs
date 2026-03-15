using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.Abstractions.Processes
{
    public interface IProcessRuntime
    {
        string ProcessKey { get; }

        Task InitializeAsync(ProcessDefinition process, CancellationToken cancellationToken = default);

        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);

        Task ApplySequenceAsync(SequenceDefinition sequence, CancellationToken cancellationToken = default);

        Task RemoveSequenceAsync(int sequenceId, CancellationToken cancellationToken = default);
    }
}
