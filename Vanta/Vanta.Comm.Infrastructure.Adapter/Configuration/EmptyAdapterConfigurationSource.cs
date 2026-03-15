namespace Vanta.Comm.Infrastructure.Adapter.Configuration
{
    public sealed class EmptyAdapterConfigurationSource : IAdapterConfigurationSource
    {
        public Task<AdapterConfigurationSnapshot> LoadAsync(CancellationToken cancellationToken = default)
        {
            AdapterConfigurationSnapshot snapshot = new AdapterConfigurationSnapshot();
            return Task.FromResult(snapshot);
        }
    }
}
