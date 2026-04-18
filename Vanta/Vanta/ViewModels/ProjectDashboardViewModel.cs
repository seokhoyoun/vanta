using System.Collections.Generic;

namespace Vanta.ViewModels
{
    public class ProjectDashboardViewModel
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string TeamName { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string OwnerName { get; set; } = string.Empty;

        public string StatusName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Summary { get; set; } = string.Empty;

        public List<EquipmentSectionViewModel> Equipments { get; set; } = new List<EquipmentSectionViewModel>();
    }
}
