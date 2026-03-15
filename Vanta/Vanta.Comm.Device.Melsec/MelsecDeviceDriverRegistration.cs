using System;
using System.Collections.Generic;
using Vanta.Comm.Abstractions.Devices;
using Vanta.Comm.Device.Melsec.Constants;

namespace Vanta.Comm.Device.Melsec
{
    public static class MelsecDeviceDriverRegistration
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

            factories[MelsecDriverKeys.Melsec] = CreateDriver;
            factories[MelsecDriverKeys.LegacyDllName] = CreateDriver;
            factories[MelsecDriverKeys.LegacyModuleName] = CreateDriver;
        }

        private static IDeviceDriver CreateDriver()
        {
            return new MelsecDeviceDriver();
        }
    }
}
