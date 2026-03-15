using Vanta.Comm.Contracts.Enums;

namespace Vanta.Comm.Contracts.Models
{
    public sealed class CompositeFieldDefinition
    {
        public int FieldSequence { get; set; }

        public int CompositeTypeId { get; set; }

        public int DisplayOrder { get; set; }

        public DataShapeKind DataShapeKind { get; set; }

        public DataShapeKind ElementShapeKind { get; set; }

        public int WordOffset { get; set; }

        public int WordLength { get; set; }

        public int BitOffset { get; set; }

        public int BitLength { get; set; }

        public int ElementCount { get; set; }

        public int DecimalPosition { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsEnabled { get; set; }

        public string FieldName { get; set; } = string.Empty;

        public string DataType { get; set; } = string.Empty;

        public string CompositeTypeName { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
