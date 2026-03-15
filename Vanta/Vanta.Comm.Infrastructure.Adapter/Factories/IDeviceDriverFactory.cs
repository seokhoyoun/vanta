using Vanta.Comm.Abstractions.Devices;

namespace Vanta.Comm.Infrastructure.Adapter.Factories
{
    public interface IDeviceDriverFactory
    {
        bool TryCreate(string driverKey, out IDeviceDriver? driver);
    }
}
