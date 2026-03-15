using Vanta.Comm.Contracts.Enums;

namespace Vanta.Comm.Contracts.Models
{
    public sealed class TagDefinition
    {
        public int TagSequence { get; set; }

        public int DeviceId { get; set; }

        public int BlockSequence { get; set; }

        public MemoryKind MemoryKind { get; set; }

        public int AddressIndex { get; set; }

        public int AddressLength { get; set; }

        public int BitDigit { get; set; }

        public int DecimalPosition { get; set; }

        public int MinimumValue { get; set; }

        public int MaximumValue { get; set; }

        public int AnalogMinimum { get; set; }

        public int AnalogMaximum { get; set; }

        public int EventUnit { get; set; }

        public int ElementCount { get; set; }

        public int CompositeTypeId { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsEnabled { get; set; }

        public DataShapeKind DataShapeKind { get; set; }

        public DataShapeKind ElementShapeKind { get; set; }

        public string TagGroup { get; set; } = string.Empty;

        public string TagName { get; set; } = string.Empty;

        public string DataType { get; set; } = string.Empty;

        public string CompositeTypeName { get; set; } = string.Empty;

        public string RawValue { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public string AddressHead { get; set; } = string.Empty;

        public string TagAddress { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string EventGroup { get; set; } = string.Empty;

        public Dictionary<string, string> EventKeys { get; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
