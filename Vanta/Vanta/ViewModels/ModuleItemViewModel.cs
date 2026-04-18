using Vanta.Models;
using System.Collections.Generic;

namespace Vanta.ViewModels
{
    public class ModuleItemViewModel
    {
        public string Code { get; set; } = string.Empty;

        public EProjectEquipmentModuleType ModuleType { get; set; } = EProjectEquipmentModuleType.Other;

        public string TypeName => ModuleType.ToDisplayName();

        public string Name { get; set; } = string.Empty;

        public string ManufacturerName { get; set; } = string.Empty;

        public string ModelName { get; set; } = string.Empty;

        public List<string> SerialNumbers { get; set; } = new List<string>();

        public List<ModuleDriverViewModel> Drivers { get; set; } = new List<ModuleDriverViewModel>();

        public string PlatformSummary { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public string PcRoleName { get; set; } = string.Empty;

        public string PcCpuModel { get; set; } = string.Empty;

        public string PcMemorySpec { get; set; } = string.Empty;

        public string PcStorageSpec { get; set; } = string.Empty;

        public string PcGpuModel { get; set; } = string.Empty;

        public string PcOsName { get; set; } = string.Empty;

        public string PcOsVersion { get; set; } = string.Empty;

        public string PcMainApplicationName { get; set; } = string.Empty;

        public string PcNetworkNotes { get; set; } = string.Empty;
    }
}
