using Vanta.Models;

namespace Vanta.Repositories.ProjectEquipments
{
    public interface IProjectEquipmentRepository
    {
        Task<List<ProjectEquipment>> GetByProjectId(string projectId, CancellationToken cancellationToken = default);

        Task<ProjectEquipment?> GetByIdOrNull(string id, CancellationToken cancellationToken = default);

        Task<ProjectEquipment?> GetByProjectIdAndCodeOrNull(string projectId, string code, CancellationToken cancellationToken = default);

        Task Create(ProjectEquipment projectEquipment, CancellationToken cancellationToken = default);

        Task Replace(ProjectEquipment projectEquipment, CancellationToken cancellationToken = default);

        Task Delete(string id, CancellationToken cancellationToken = default);
    }
}
