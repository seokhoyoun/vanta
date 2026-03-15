using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.Abstractions.Services
{
    public interface ITraceService
    {
        Task WriteAsync(TraceEntry entry, CancellationToken cancellationToken = default);
    }
}
