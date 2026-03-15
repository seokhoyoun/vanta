using System.Collections.Generic;

namespace Vanta.Comm.Contracts.Models
{
    public sealed class CompositeTypeDefinition
    {
        public int CompositeTypeId { get; set; }

        public int Version { get; set; }

        public int WordLength { get; set; }

        public bool IsEnabled { get; set; }

        public string CompositeTypeName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<CompositeFieldDefinition> Fields { get; } =
            new List<CompositeFieldDefinition>();
    }
}
