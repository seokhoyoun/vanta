namespace Vanta.Comm.Contracts.Models
{
    public sealed class TraceEntry
    {
        public int Type { get; set; }

        public string SourceName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
