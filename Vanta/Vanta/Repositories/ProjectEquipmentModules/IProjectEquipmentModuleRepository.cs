using Vanta.Models;

namespace Vanta.Repositories.ProjectEquipmentModules
{
    public interface IProjectEquipmentModuleRepository
    {
        Task<List<ProjectEquipmentModule>> GetByProjectEquipmentId(string projectEquipmentId, CancellationToken cancellationToken = default);

        Task<ProjectEquipmentModule?> GetByIdOrNull(string id, CancellationToken cancellationToken = default);

        Task<ProjectEquipmentModule?> GetByProjectEquipmentIdAndCodeOrNull(string projectEquipmentId, string code, CancellationToken cancellationToken = default);

        Task Create(ProjectEquipmentModule projectEquipmentModule, CancellationToken cancellationToken = default);

        Task Replace(ProjectEquipmentModule projectEquipmentModule, CancellationToken cancellationToken = default);

        Task Delete(string id, CancellationToken cancellationToken = default);
    }
}
