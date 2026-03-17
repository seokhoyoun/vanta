using System;
using System.Collections.Generic;
using Vanta.Comm.Abstractions.Devices;
using Vanta.Comm.Device.Mitsubishi.PLC.CCLinkIE.Constants;

namespace Vanta.Comm.Device.Mitsubishi.PLC.CCLinkIE
{
    public static class CCLinkIeDeviceDriverRegistration
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

            factories[CCLinkIeDriverKeys.CCLinkIe] = CreateDriver;
        }

        private static IDeviceDriver CreateDriver()
        {
            return new CCLinkIeDeviceDriver();
        }
    }
}
