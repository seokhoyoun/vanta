namespace Vanta.Comm.Contracts.Models
{
    public sealed class SequenceDefinition
    {
        public int SequenceId { get; set; }

        public int ProcessId { get; set; }

        public int ThreadNumber { get; set; }

        public bool IsEnabled { get; set; }

        public string SequenceName { get; set; } = string.Empty;
    }
}
