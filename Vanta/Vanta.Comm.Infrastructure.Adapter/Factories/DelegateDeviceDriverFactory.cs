using System.Collections.Generic;
using Vanta.Comm.Abstractions.Devices;

namespace Vanta.Comm.Infrastructure.Adapter.Factories
{
    public sealed class DelegateDeviceDriverFactory : IDeviceDriverFactory
    {
        private readonly Dictionary<string, Func<IDeviceDriver>> _factories;

        public DelegateDeviceDriverFactory(IDictionary<string, Func<IDeviceDriver>> factories)
        {
            _factories = new Dictionary<string, Func<IDeviceDriver>>(factories, StringComparer.OrdinalIgnoreCase);
        }

        public bool TryCreate(string driverKey, out IDeviceDriver? driver)
        {
            Func<IDeviceDriver>? factory;

            if (_factories.TryGetValue(driverKey, out factory))
            {
                driver = factory();
                return true;
            }

            driver = null;
            return false;
        }
    }
}
