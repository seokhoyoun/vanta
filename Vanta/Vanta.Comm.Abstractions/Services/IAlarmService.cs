using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.Abstractions.Services
{
    public interface IAlarmService
    {
        Task RaiseAsync(CurrentAlarm alarm, CancellationToken cancellationToken = default);

        Task ClearAsync(int alarmId, CancellationToken cancellationToken = default);
    }
}
