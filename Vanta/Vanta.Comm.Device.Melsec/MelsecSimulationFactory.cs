using System;
using Vanta.Comm.Abstractions.Devices;
using Vanta.Comm.Device.Melsec.Addressing;
using Vanta.Comm.Device.Melsec.Communication;
using Vanta.Comm.Simulation.Profiles;

namespace Vanta.Comm.Device.Melsec
{
    public static class MelsecSimulationFactory
    {
        public static IDeviceDriver CreateDriver()
        {
            return CreateDriver(new DeviceSimulationProfile());
        }

        public static IDeviceDriver CreateDriver(DeviceSimulationProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            SimulatedMelsecCommunicationClient communicationClient =
                new SimulatedMelsecCommunicationClient(profile);

            return new MelsecDeviceDriver(communicationClient, new MelsecAddressParser());
        }
    }
}
