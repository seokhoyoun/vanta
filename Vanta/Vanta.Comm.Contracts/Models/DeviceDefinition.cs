using Vanta.Comm.Contracts.Enums;

namespace Vanta.Comm.Contracts.Models
{
    public sealed class DeviceDefinition
    {
        public int DeviceId { get; set; }

        public CommunicationType CommunicationType { get; set; }

        public int ScanTime { get; set; }

        public int Timeout { get; set; }

        public ThreadEnableFlags ThreadFlags { get; set; } = ThreadEnableFlags.Double;

        public int NetworkNumber { get; set; }

        public int StationNumber { get; set; }

        public int Channel { get; set; }

        public int DevicePort { get; set; }

        public int ComPort { get; set; }

        public int BaudRate { get; set; }

        public int Parity { get; set; }

        public int DataBits { get; set; }

        public int StopBits { get; set; }

        public int TagCount { get; set; }

        public bool IsInitialized { get; set; }

        public bool IsEnabled { get; set; }

        public bool PingCheckEnabled { get; set; }

        public string DeviceName { get; set; } = string.Empty;

        public string DriverModule { get; set; } = string.Empty;

        public string DeviceIpAddress { get; set; } = string.Empty;
    }
}
