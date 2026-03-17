using System;
using Vanta.Comm.Abstractions.Devices;
using Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Addressing;
using Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Communication;
using Vanta.Comm.Simulation.Profiles;

namespace Vanta.Comm.Device.Mitsubishi.PLC.McProtocol
{
    public static class McProtocolSimulationFactory
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

            return new McProtocolDeviceDriver(communicationClient, new MelsecAddressParser());
        }
    }
}
