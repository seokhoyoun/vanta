namespace Vanta.Comm.Device.Melsec.Addressing
{
    public sealed class MelsecAddress
    {
        public MelsecAddress(string memoryHead, int address)
        {
            MemoryHead = memoryHead;
            Address = address;
        }

        public string MemoryHead { get; }

        public int Address { get; }
    }
}
