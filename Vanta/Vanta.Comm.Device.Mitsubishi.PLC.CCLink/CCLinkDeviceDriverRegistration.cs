using System;
using System.Collections.Generic;
using Vanta.Comm.Abstractions.Devices;
using Vanta.Comm.Device.Mitsubishi.PLC.CCLink.Constants;

namespace Vanta.Comm.Device.Mitsubishi.PLC.CCLink
{
    public static class CCLinkDeviceDriverRegistration
    {
        public static IDictionary<string, Func<IDeviceDriver>> CreateFactoryMap()
        {
            Dictionary<string, Func<IDeviceDriver>> factories =
                new Dictionary<string, Func<IDeviceDriver>>(StringComparer.OrdinalIgnoreCase);

            Register(factories);

            return factories;
        }

        public static void Register(IDictionary<string, Func<IDeviceDriver>> factories)
        {
            if (factories == null)
            {
                throw new ArgumentNullException(nameof(factories));
            }

            factories[CCLinkDriverKeys.CCLink] = CreateDriver;
        }

        private static IDeviceDriver CreateDriver()
        {
            return new CCLinkDeviceDriver();
        }
    }
}
