using Vanta.Models;
using Vanta.ViewModels;

namespace Vanta.Pages.Projects
{
    public class ModuleEditorFieldsViewModel
    {
#region Constructors

        public ModuleEditorFieldsViewModel()
        {
        }

        public ModuleEditorFieldsViewModel(ModuleItemViewModel module, string serialNumbersText)
        {
            ModuleCode = module.Code;
            ModuleType = module.ModuleType;
            ModuleName = module.Name;
            ModuleManufacturerName = module.ManufacturerName;
            ModuleModelName = module.ModelName;
            ModuleSerialNumbersText = serialNumbersText;
            Drivers = new List<ModuleDriverViewModel>(module.Drivers);
            ModulePlatformSummary = module.PlatformSummary;
            ModuleNotes = module.Notes;
            ModulePcRoleName = module.PcRoleName;
            ModulePcCpuModel = module.PcCpuModel;
            ModulePcMemorySpec = module.PcMemorySpec;
            ModulePcStorageSpec = module.PcStorageSpec;
            ModulePcGpuModel = module.PcGpuModel;
            ModulePcOsName = module.PcOsName;
            ModulePcOsVersion = module.PcOsVersion;
            ModulePcMainApplicationName = module.PcMainApplicationName;
            ModulePcNetworkNotes = module.PcNetworkNotes;
        }

#endregion

#region Public Properties

        public string ModuleCode { get; set; } = string.Empty;

        public EProjectEquipmentModuleType ModuleType { get; set; } = EProjectEquipmentModuleType.Pc;

        public string ModuleName { get; set; } = string.Empty;

        public string ModuleManufacturerName { get; set; } = string.Empty;

        public string ModuleModelName { get; set; } = string.Empty;

        public string ModuleSerialNumbersText { get; set; } = string.Empty;

        public List<ModuleDriverViewModel> Drivers { get; set; } = new List<ModuleDriverViewModel>();

        public string ModulePlatformSummary { get; set; } = string.Empty;

        public string ModuleNotes { get; set; } = string.Empty;

        public string ModulePcRoleName { get; set; } = string.Empty;

        public string ModulePcCpuModel { get; set; } = string.Empty;

        public string ModulePcMemorySpec { get; set; } = string.Empty;

        public string ModulePcStorageSpec { get; set; } = string.Empty;

        public string ModulePcGpuModel { get; set; } = string.Empty;

        public string ModulePcOsName { get; set; } = string.Empty;

        public string ModulePcOsVersion { get; set; } = string.Empty;

        public string ModulePcMainApplicationName { get; set; } = string.Empty;

        public string ModulePcNetworkNotes { get; set; } = string.Empty;

#endregion

#region Public Methods

        public List<ModuleDriverViewModel> GetDriverRows()
        {
            List<ModuleDriverViewModel> rows = new List<ModuleDriverViewModel>(Drivers);
            int rowCount = Math.Max(3, Drivers.Count + 1);

            while (rows.Count < rowCount)
            {
                rows.Add(new ModuleDriverViewModel());
            }

            return rows;
        }

#endregion
    }
}
