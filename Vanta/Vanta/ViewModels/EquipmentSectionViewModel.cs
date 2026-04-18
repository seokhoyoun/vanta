using System.Collections.Generic;

namespace Vanta.ViewModels
{
    public class EquipmentSectionViewModel
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string StageName { get; set; } = string.Empty;

        public string PlatformName { get; set; } = string.Empty;

        public List<ModuleItemViewModel> Modules { get; set; } = new List<ModuleItemViewModel>();
    }
}
