using System.Collections.Generic;

namespace Vanta.Models
{
    public class ProjectEquipmentModule
    {
        public string Id { get; set; } = string.Empty;

        public string ProjectId { get; set; } = string.Empty;

        public string ProjectEquipmentId { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public EProjectEquipmentModuleType ModuleType { get; set; } = EProjectEquipmentModuleType.Other;

        public string ManufacturerName { get; set; } = string.Empty;

        public string ModelName { get; set; } = string.Empty;

        public List<string> SerialNumbers { get; set; } = new List<string>();

        public List<ProjectEquipmentModuleDriver> Drivers { get; set; } = new List<ProjectEquipmentModuleDriver>();

        public string PlatformSummary { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public ProjectEquipmentModulePcSpec? PcSpec { get; set; }

        public ProjectEquipmentModuleMotionSpec? MotionSpec { get; set; }

        public ProjectEquipmentModuleIoSpec? IoSpec { get; set; }

        public ProjectEquipmentModulePlcSpec? PlcSpec { get; set; }

        public ProjectEquipmentModuleRfidSpec? RfidSpec { get; set; }

        public ProjectEquipmentModuleSoftwarePlatformSpec? SoftwarePlatformSpec { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
