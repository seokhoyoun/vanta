namespace Vanta.Comm.Device.Melsec.Communication
{
    internal sealed class MelsecDeviceAccessSpec
    {
        public MelsecDeviceAccessSpec(string memoryHead, byte deviceCode, bool isBitDevice)
        {
            MemoryHead = memoryHead;
            DeviceCode = deviceCode;
            IsBitDevice = isBitDevice;
        }

        public string MemoryHead { get; }

        public byte DeviceCode { get; }

        public bool IsBitDevice { get; }
    }
}
