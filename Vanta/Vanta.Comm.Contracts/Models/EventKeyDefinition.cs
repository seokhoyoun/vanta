namespace Vanta.Comm.Contracts.Models
{
    public sealed class EventKeyDefinition
    {
        public string EventGroup { get; set; } = string.Empty;

        public string EventKey { get; set; } = string.Empty;

        public int EventUnit { get; set; }

        public int EventCeid { get; set; }
    }
}
