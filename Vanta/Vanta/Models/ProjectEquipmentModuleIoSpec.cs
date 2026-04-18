namespace Vanta.Models
{
    public class ProjectEquipmentModuleIoSpec
    {
        public string ProtocolName { get; set; } = string.Empty;

        public string MasterModuleName { get; set; } = string.Empty;

        public int NodeCount { get; set; }

        public int ChannelCount { get; set; }

        public string AddressingNotes { get; set; } = string.Empty;
    }
}
