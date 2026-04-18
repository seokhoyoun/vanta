namespace Vanta.Models
{
    public static class ProjectEquipmentModuleTypeExtensions
    {
        public static string ToDisplayName(this EProjectEquipmentModuleType moduleType)
        {
            return moduleType switch
            {
                EProjectEquipmentModuleType.Pc => "PC",
                EProjectEquipmentModuleType.LoadPort => "LoadPort",
                EProjectEquipmentModuleType.Fims => "FIMS",
                EProjectEquipmentModuleType.Wtr => "WTR",
                EProjectEquipmentModuleType.Ftr => "FTR",
                EProjectEquipmentModuleType.Motor => "Motor",
                EProjectEquipmentModuleType.Io => "I/O",
                EProjectEquipmentModuleType.Plc => "PLC",
                EProjectEquipmentModuleType.Rfid => "RFID",
                EProjectEquipmentModuleType.SoftwarePlatform => "S/W Platform",
                _ => "Other",
            };
        }
    }
}
