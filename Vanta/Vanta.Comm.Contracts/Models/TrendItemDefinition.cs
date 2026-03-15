namespace Vanta.Comm.Contracts.Models
{
    public sealed class TrendItemDefinition
    {
        public int ItemSequence { get; set; }

        public string TrendGroup { get; set; } = string.Empty;

        public string ItemName { get; set; } = string.Empty;

        public string LinkedTag { get; set; } = string.Empty;

        public bool IsAnalog { get; set; }

        public bool IsEnabled { get; set; }
    }
}
