using Vanta.ViewModels;

namespace Vanta.Services.Projects
{
    public interface IProjectCatalogService
    {
        Task<ProjectListPageViewModel> GetProjectListPage(CancellationToken cancellationToken = default);

        Task<ProjectDashboardViewModel?> GetProjectDashboardOrNull(string code, CancellationToken cancellationToken = default);

        Task<bool> CreateProject(ProjectDashboardViewModel projectDashboard, CancellationToken cancellationToken = default);

        Task<bool> UpdateProject(string originalCode, ProjectDashboardViewModel projectDashboard, CancellationToken cancellationToken = default);

        Task<bool> DeleteProject(string code, CancellationToken cancellationToken = default);

        Task<bool> CreateEquipment(string projectCode, EquipmentSectionViewModel equipment, CancellationToken cancellationToken = default);

        Task<bool> UpdateEquipment(
            string projectCode,
            string originalEquipmentCode,
            EquipmentSectionViewModel equipment,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteEquipment(string projectCode, string equipmentCode, CancellationToken cancellationToken = default);

        Task<bool> CreateModule(
            string projectCode,
            string equipmentCode,
            ModuleItemViewModel module,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateModule(
            string projectCode,
            string equipmentCode,
            string originalModuleCode,
            ModuleItemViewModel module,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteModule(
            string projectCode,
            string equipmentCode,
            string moduleCode,
            CancellationToken cancellationToken = default);

        Task<bool> CanEditProject(
            string code,
            string userId,
            bool isAdmin,
            CancellationToken cancellationToken = default);
    }
}
