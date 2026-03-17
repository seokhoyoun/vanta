using System;
using System.Collections.Generic;
using Vanta.Comm.Abstractions.Devices;
using Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Constants;

namespace Vanta.Comm.Device.Mitsubishi.PLC.McProtocol
{
    public static class McProtocolDeviceDriverRegistration
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

            factories[McProtocolDriverKeys.McProtocol] = CreateDriver;
            factories[McProtocolDriverKeys.Melsec] = CreateDriver;
            factories[McProtocolDriverKeys.LegacyDllName] = CreateDriver;
            factories[McProtocolDriverKeys.LegacyModuleName] = CreateDriver;
        }

        private static IDeviceDriver CreateDriver()
        {
            return new McProtocolDeviceDriver();
        }
    }
}
