using Vanta.Comm.Contracts.Enums;

namespace Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Runtime
{
    internal sealed class MelsecTagContext
    {
        public MelsecTagContext(
            int tagSequence,
            string memoryHead,
            int startAddress,
            int wordLength,
            int bitDigit,
            MemoryKind memoryKind)
        {
            TagSequence = tagSequence;
            MemoryHead = memoryHead;
            StartAddress = startAddress;
            WordLength = wordLength;
            BitDigit = bitDigit;
            MemoryKind = memoryKind;
        }

        public int TagSequence { get; }

        public string MemoryHead { get; }

        public int StartAddress { get; }

        public int WordLength { get; }

        public int BitDigit { get; }

        public MemoryKind MemoryKind { get; }
    }
}
