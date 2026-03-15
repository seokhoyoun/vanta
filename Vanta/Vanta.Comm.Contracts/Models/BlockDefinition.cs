using Vanta.Comm.Contracts.Enums;

namespace Vanta.Comm.Contracts.Models
{
    public sealed class BlockDefinition
    {
        public int BlockSequence { get; set; }

        public int DeviceId { get; set; }

        public MemoryKind MemoryKind { get; set; }

        public AddressFormat AddressFormat { get; set; }

        public int BlockLength { get; set; }

        public int TagCount { get; set; }

        public bool IsBlockUsed { get; set; }

        public bool IsEnabled { get; set; }

        public string BlockName { get; set; } = string.Empty;

        public string BlockHead { get; set; } = string.Empty;

        public string BaseAddress { get; set; } = string.Empty;
    }
}
