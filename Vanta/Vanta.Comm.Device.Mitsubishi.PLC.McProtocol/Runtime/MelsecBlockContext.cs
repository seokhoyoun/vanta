using Vanta.Comm.Contracts.Enums;

namespace Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Runtime
{
    internal sealed class MelsecBlockContext
    {
        public MelsecBlockContext(
            int blockSequence,
            string memoryHead,
            int startAddress,
            int length,
            MemoryKind memoryKind,
            AddressFormat addressFormat)
        {
            BlockSequence = blockSequence;
            MemoryHead = memoryHead;
            StartAddress = startAddress;
            Length = length;
            MemoryKind = memoryKind;
            AddressFormat = addressFormat;
        }

        public int BlockSequence { get; }

        public string MemoryHead { get; }

        public int StartAddress { get; }

        public int Length { get; }

        public MemoryKind MemoryKind { get; }

        public AddressFormat AddressFormat { get; }
    }
}
