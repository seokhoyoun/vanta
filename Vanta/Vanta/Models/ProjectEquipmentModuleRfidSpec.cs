namespace Vanta.Models
{
    public class ProjectEquipmentModuleRfidSpec
    {
        public int ReaderCount { get; set; }

        public int AntennaCount { get; set; }

        public string TagStandard { get; set; } = string.Empty;

        public string InterfaceType { get; set; } = string.Empty;

        public string MiddlewareName { get; set; } = string.Empty;
    }
}
