using Vanta.Comm.Contracts.Enums;

namespace Vanta.Comm.Contracts.Models
{
    public sealed class ProcessDefinition
    {
        public int ProcessId { get; set; }

        public int ScanTime { get; set; }

        public int Timeout { get; set; }

        public ThreadEnableFlags ThreadFlags { get; set; } = ThreadEnableFlags.One;

        public bool IsInitialized { get; set; }

        public bool IsEnabled { get; set; }

        public string ProcessName { get; set; } = string.Empty;

        public string ProcessModule { get; set; } = string.Empty;
    }
}
