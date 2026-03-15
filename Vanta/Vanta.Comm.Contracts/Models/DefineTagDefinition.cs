namespace Vanta.Comm.Contracts.Models
{
    public sealed class DefineTagDefinition
    {
        public int DefineTagSequence { get; set; }

        public string DefineTagName { get; set; } = string.Empty;

        public string LinkedTagName { get; set; } = string.Empty;
    }
}
