namespace Vanta.Comm.Infrastructure.Adapter.Configuration
{
    public interface IAdapterConfigurationSource
    {
        Task<AdapterConfigurationSnapshot> LoadAsync(CancellationToken cancellationToken = default);
    }
}
